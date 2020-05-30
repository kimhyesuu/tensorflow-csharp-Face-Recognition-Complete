using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.UserInterface;
using System.Threading;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenCvSharp.Util;
using OpenCvSharp.Dnn;
using MySql.Data.MySqlClient;
using System.Drawing.Imaging;

namespace 축산_프로젝트
{
    public partial class FaceID : Form
    {
        //opencvsharp
        VideoCapture video; 
        Mat frame;
        
        //image
        private string _filename = "";
        string _saveName;
        string _saveFname;
        private int _outH, _outW; // IO 2D Video Size
        private Bitmap _paper; // Output   
        Mat inCvImage, outCvImage;
        private byte[,,] _inImage, _outImage; 
        Bitmap _image; //저장 이미
        int cnt2 = 0;

        private Double modalOpa = 0.0;

        //Mysql 
        String connStr;
        MySqlConnection conn;
        MySqlCommand cmd;

        public FaceID()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
                var model = "opencv_face_detector_uint8.pb"; //tensorflow 학습된 모델 파일
                var config = "opencv_face_detector.pbtxt";  // 구성 파일
                var confidence = 0.0;
                var x1 = 0.0;
                var x2 = 0.0;
                var y1 = 0.0;
                var y2 = 0.0;
                int cnt = 0;

                video = new VideoCapture(1); //video 1번(ManyCam) 사용

                if (!video.IsOpened())//video 초기화 안될 시 error
                {
                    Console.WriteLine("error");
                    return;
                }

                Net net = CvDnn.ReadNet(model, config);

                if (net.Empty())
                {
                    Console.WriteLine("error");
                    return;
                }

                frame = new Mat();

                while (true)
                {
                    video.Read(frame);
                    if (frame.Empty()) break;

                    //frame  blob 입력 설정된 부분을 net
                    Mat blob = CvDnn.BlobFromImage(frame, 1.0, new OpenCvSharp.Size(300, 300), new Scalar(104, 177, 123));
                    net.SetInput(blob);
                    Mat res = net.Forward();
                   
                    var detect = res.Reshape(1, res.Cols * res.Rows);
                   
                    for (int i = 0; i < detect.Rows; i++) //Face recongition
                    {
                        confidence = detect.At<float>(i, 2);
                        if (confidence < 0.5) break; //Face recongition 0.5 이하는 무시 

                        //get center and width/height
                        x1 = Math.Round(detect.At<float>(i, 3) * frame.Cols);//좌표 계산 
                        y1 = Math.Round(detect.At<float>(i, 4) * frame.Rows);
                        x2 = Math.Round(detect.At<float>(i, 5) * frame.Cols);
                        y2 = Math.Round(detect.At<float>(i, 6) * frame.Rows);

                        string Label = string.Format("Face:" + confidence);// 1. Face recongition을 시각화하는 작업 


                    //2. confidence > 0.955 greenbox 생성
                    if (confidence > 0.955)
                        {
                            Cv2.Rectangle(frame, new OpenCvSharp.Point(x1, y1), new OpenCvSharp.Point(x2, y2), new Scalar(0, 255, 0));
                            Cv2.PutText(frame, Label, new OpenCvSharp.Point(x1, y1 - 1), OpenCvSharp.HersheyFonts.Italic, 0.8, new Scalar(0, 255, 0));
                        }
                    //2. 아닐 시 redbox 생성
                    else
                    {
                            Cv2.Rectangle(frame, new OpenCvSharp.Point(x1, y1), new OpenCvSharp.Point(x2, y2), new Scalar(0, 0, 255));
                            Cv2.PutText(frame, Label, new OpenCvSharp.Point(x1, y1 - 1), OpenCvSharp.HersheyFonts.Italic, 0.8, new Scalar(0, 0, 255));
                        }
                    }

                    Cv2.ImShow("FACE ID", frame);

                    if (confidence > 0.997910)
                    {
                        cnt++;
                        if (cnt >= 30)//30번 인식시 파일 save
                        {
                            _saveName = "C:/images/" + DateTime.Now.ToString("yyyy/MM/dd_hh_mm_ss") + ".jpeg";
                            Cv2.ImWrite(_saveName, frame);
                            video.Release();
                            Cv2.DestroyAllWindows();
                            break;
                        }
                    }

                    if (Cv2.WaitKey(1) == 27)//아스키코드  27 : esc
                        break;
                }

                Delay(100);

                inCvImage = Cv2.ImRead(_saveName); //2번 
                Cv2.Transpose(inCvImage, inCvImage);

                outCvImage = new Mat();
                Cv2.CvtColor(inCvImage, outCvImage, ColorConversionCodes.BGR2GRAY);
                Cv2.EqualizeHist(outCvImage, outCvImage);

                Cv2ToOutImage();

                //사각박스 내부만 나오게 설정
                for (int rgb = 0; rgb < 3; rgb++)
                    for (int i = 0; i < _outH; i++)
                        for (int k = 0; k < _outW; k++)
                        {
                            if ((x1 <= i && i <= x2) && (y1 <= k && k <= y2))
                            {
                                _outImage[rgb, i, k] = (byte)(255 - _outImage[rgb, i, k]);
                            }
                            else
                            {
                                _outImage[rgb, i, k] = 0;
                            }
                        }

                DisplayImage();
                
                connStr = "Server=127.0.0.1; Uid=root; Pwd=1234;Database=HYDB;CHARSET=UTF8";
                conn = new MySqlConnection(connStr);
                //db로 전달
                dbUpload();
                CCTV ma = new CCTV();
                ma.ShowDialog();
                Close();

        } //1. 얼굴인식 후 저장 및 blob 업로드

        //Click Event

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (frame != null)
            {
                Cv2.DestroyAllWindows();
                frame.Dispose();
                video.Release();
            }
        }


        //공통 함수  

        void DisplayImage()
        {
            int oW = _outW, oH = _outH;
            int hop = 1;

            if (_outH > 512 || _outW > 512)
            {
                if (_outW > _outH)
                    hop = (int)(_outW / 512);
                else
                    hop = (int)(_outH / 512);

                oW = _outW / hop; oH = _outH / hop;
            }

            // 종이, 게시판, 벽 크기 조절
            _paper = new Bitmap(oH, oW);           
            this.Size = new System.Drawing.Size(oH + 20, oW + 90);

            Color pen; // 펜 (콕콕 찍을 펜)
            for (int i = 0; i < oH; i++)
                for (int k = 0; k < oW; k++)
                {
                    if (i >= oH - 1 || k >= oW - 1)
                        continue;
                    if (_outImage[0, i, k] == 0 || _outImage[1, i, k] == 0 || _outImage[2, i, k] == 0) continue;
                    byte dataR = _outImage[0, i * hop, k * hop];  // 색깔 (잉크)
                    byte dataG = _outImage[1, i * hop, k * hop];  // 색깔 (잉크)
                    byte dataB = _outImage[2, i * hop, k * hop];  // 색깔 (잉크)
                    pen = Color.FromArgb(dataR, dataG, dataB); // 펜에 잉크 묻힘
                    _paper.SetPixel(i, k, pen); // 종이에 콕 찍음.
                }          
        }//이미지 설치

        private int CheckRange(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }//overflow & underflow checking

        private void Cv2ToOutImage()
        {
            // 원래 출력 배열 메모리 확보
            _outH = outCvImage.Height;
            _outW = outCvImage.Width;
            _outImage = new byte[3, _outH, _outW];
            // outCvImage --> outImage
            for (int i = 0; i < _outH; i++)
                for (int k = 0; k < _outW; k++)
                {
                    var c = outCvImage.At<Vec3b>(i, k);
                    _outImage[0, i, k] = c.Item2;
                    _outImage[1, i, k] = c.Item1;
                    _outImage[2, i, k] = c.Item0;
                }            
        }//opencv 

        private void dbUpload()
        {
            _saveFname = "C:\\FACEID\\" + DateTime.Now.ToString("yyyy/MM/dd_hh_mm_ss") + ".jpeg";
            _paper.Save(_saveFname, ImageFormat.Jpeg);

            Delay(150);

            conn.Open();
            cmd = new MySqlCommand("", conn);
            string full_name = _saveFname;
            string[] tmpArray = full_name.Split('\\');
            string tmp1 = tmpArray[tmpArray.Length - 1];
            string[] tmp2 = tmp1.Split('.');
            string file_name = tmp2[0];
            string ext_name = tmp2[1];
            long fsize = new FileInfo(full_name).Length;
            Random rnd = new Random();

            int f_id = rnd.Next(0, 2147483647);

            // 부모 테이블(파일 테이블)에 입력

            string sql = "INSERT INTO blob_table(f_id,file_name,ext_name,fileSize,fileData)";
            sql += "VALUES(" + f_id + ",'" + file_name + "', '";
            sql += ext_name + "', " + fsize + ", @FileData)";//@FileData는 변수입니다.

            //파일을 통째로 @FileData에 넣기

            FileStream fs = new FileStream(full_name, FileMode.Open, FileAccess.Read);
            byte[] fileData = new byte[fsize]; //선언
            fs.Read(fileData, 0, (int)fsize);//하나씩 처리하는 것이 아닌 통째로 쓰는 것입니다. 
            fs.Close();

            cmd.Parameters.AddWithValue("@FileData", fileData);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            MessageBox.Show("업로드");
            conn.Close();

            frame.Dispose();
            video.Release();
        }//저장 후 업로드
   
        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        } //딜레이 함수     
    }

}
