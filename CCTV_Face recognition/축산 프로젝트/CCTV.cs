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
    public partial class CCTV : Form
    {
        VideoCapture video;

        Mat inCvImage;
        Mat outCvImage;
        Mat tmpImage;
        Mat dst1;
        Mat dst2;

        public CCTV()
        {
            InitializeComponent();
        }

        private void CCTV_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (inCvImage != null)
            {
                inCvImage.Dispose();
                video.Release();
                // Cv2.DestroyAllWindows();
            }
        }


    
        private void button1_Click(object sender, EventArgs e)
        {
            video = new VideoCapture("D:\\project\\c#\\Day021\\vtest.avi");
            Random r = new Random();
            int cnt = 0;
            int x=0,y=0,W=0,H=0;
              
            int sleepTime = (int)Math.Round(1000 / video.Fps);
            String filenameBodyCascade = "D:\\project\\c#\\Day021\\haarcascade_fullbody.xml"; // haar-cascade 훈련 사용
            CascadeClassifier bodyCascade = new CascadeClassifier();

            if (!bodyCascade.Load(filenameBodyCascade))
            {
                Console.WriteLine("error");
                return;
            }

            inCvImage = new Mat();
            outCvImage = new Mat();

            int oH = inCvImage.Height;
            int oW = inCvImage.Width;
            bool csi = false;

            while (true)
            {
                cnt = 0;
                
                video.Read(inCvImage);
            
                if (inCvImage.Empty())
                    break;
                // detect 
                Rect[] body = bodyCascade.DetectMultiScale(inCvImage);
                Console.WriteLine(body.Length);

                foreach (var item in body)
                {
                    Scalar c = new Scalar(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));
                    Cv2.Rectangle(inCvImage, item, c); // add rectangle to the image
                    Console.WriteLine("body : " + item);
                    cnt++;

                    x = item.X;
                    y = item.Y;
                    W = item.Width;
                    H = item.Height;
                }

                if (cnt > 4)
                {
                    csi = true;
                }

                if (csi)
                {
                    outCvImage = Mat.Ones(new OpenCvSharp.Size(oW, oH), MatType.CV_8UC1);
                    Cv2.CvtColor(inCvImage, outCvImage, ColorConversionCodes.BGR2GRAY);
                    Cv2.AdaptiveThreshold(outCvImage, outCvImage, 255,
                        AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 25, 5);
                }
                else
                {
                    outCvImage = inCvImage;
                }
                
                // display        
                //Cv2.ImShow("CCTV", frame);

                picboxCow.ImageIpl = outCvImage;

                if (Cv2.WaitKey(9) == 27)
                {
                    inCvImage.Dispose(); outCvImage.Dispose(); 
                    video.Release();
                    break;
                }

                if (cnt > 7)
                {                 
                    video.Release();
                    break;
                }
               
            }


            // grayscale
            Cv2.CvtColor(inCvImage, outCvImage, ColorConversionCodes.BGR2GRAY);
            picboxCow.ImageIpl = outCvImage;

            Delay(2000); //각 변환 시각화

            //histogram equalizition을 통해서 픽셀를 고르게 분포
            Cv2.EqualizeHist(outCvImage, outCvImage);
            picboxCow.ImageIpl = outCvImage;

            Delay(2000); //각 변환 시각화

            dst1 = new Mat();//누적 image 작업을 위한 초기화
            dst2 = new Mat();

            
            Cv2.Resize(outCvImage,dst1, new OpenCvSharp.Size(1920,1280),0,0, InterpolationFlags.Lanczos4);

            //무리 내의 사람 판독을 위해 따로 배출 
            Rect rect = new Rect(x , y , W , H);
            dst1 = outCvImage.SubMat(rect);

            //image 크기와 맞게 picplcsi 설정
            picplCSi.Size = new System.Drawing.Size(W, H);
            picplCSi.ImageIpl = dst1;
            lbf.Visible = true;

            Delay(2000);//각 변환 시각화

            //보다 효율적으로 시각화 하기위한 사이즈 확대
            Cv2.Resize(dst1, dst2, new OpenCvSharp.Size(800, 600), 0, 0, InterpolationFlags.Lanczos4);

            //image 크기와 맞게 picplcsi 설정
            this.Size = new System.Drawing.Size();
            picplCSi.Size = new System.Drawing.Size(800, 600);
            
            Cv2.ImShow("dst2", dst2);//image 출력

            Cv2.WaitKey(2000);

            //파일 내로 저장 후 메모리 누수 발생 방지를 위해 사용했던 Matrix Dispose시킵니다.
            string _saveName = "C:/images/" + DateTime.Now.ToString("yyyy/MM/dd_hh_mm_ss") + ".jpeg";
            Cv2.ImWrite(_saveName, dst2);
            inCvImage.Dispose();
            outCvImage.Dispose();
            dst1.Dispose();
    
            //Check Message 
            MessageBox.Show("업로드","Checking",MessageBoxButtons.OK,MessageBoxIcon.Information);
            Close();

        }

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

        private void btnStop_Click(object sender, EventArgs e)
        {

        }
    }
}


//Bitmap downloadimage = BitmapConverter.ToBitmap(db_face);//매트릭스를 bitmap으로 한번에 바꿔주는 함수 (using.opencvsharp.Extensions);
//string path = Path.GetTempFileName();//임시 파일로 저장, 저장 경로를 반한(using.system.io)
//Path.GetTempPath();//저장한 폴더 위치 반환(using.system.io)