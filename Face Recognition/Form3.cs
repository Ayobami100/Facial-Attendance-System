using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Emgu.CV.UI;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace Face_Recognition
{
    public partial class Form3 : MetroFramework.Forms.MetroForm
    {
        #region variables
        Image<Bgr, Byte> currentFrame; //current image aquired from webcam for display
        Image<Gray, byte> result, TrainedFace = null; //used to store the result image and trained face
        Image<Gray, byte> gray_frame = null; //grayscale current image aquired from webcam for processing

        Capture grabber; //This is our capture variable

        public CascadeClassifier Face = new CascadeClassifier(Application.StartupPath + "/Cascades/haarcascade_frontalface_default.xml");//Our face detection method 

        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_COMPLEX, 0.5, 0.5); //Our fount for writing within the frame

        int NumLabels;
        public static string SetValueForText1 = "";
        //Classifier with default training location
        Classifier_Train Eigen_Recog = new Classifier_Train();
        #endregion

        public Form3()
        {
            InitializeComponent();
            if (Eigen_Recog.IsTrained)
            {
                message_bar.Text = "Training Data loaded";
            }
            else
            {
                message_bar.Text = "No training data found, please train program using Train menu option";
            }
            initialise_capture();
        }
        public string TextBoxValue
        {
            get
            {
                return txtName.Text;
            }
            set
            {
                txtName.Text = value;
            }
        }

        public void initialise_capture()
        {
            grabber = new Capture();
            grabber.QueryFrame();
            //Initialize the FrameGraber event
            Application.Idle += new EventHandler(FrameGrabber_Parrellel);

        }
        private void stop_capture()
        {
            Application.Idle -= new EventHandler(FrameGrabber_Parrellel);

            if (grabber != null)
            {
                grabber.Dispose();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            SetValueForText1 = txtName.Text;
           // this.Dispose();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetValueForText1 = txtName.Text;
           this.Dispose();
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            SetValueForText1 = txtName.Text;
            this.Dispose();
        }

        void FrameGrabber_Parrellel(object sender, EventArgs e)
        {
            try
            {

                Eigen_Recog.Recognizer_Type = "EMGU.CV.LBPHFaceRecognizer";
                //Get the current frame form capture device
                currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                //Convert it to Grayscale
                //Clear_Faces_Found();

                if (currentFrame != null)
                {
                    gray_frame = currentFrame.Convert<Gray, Byte>();
                    //Face Detector
                    Rectangle[] facesDetected = Face.DetectMultiScale(gray_frame, 1.2, 10, new Size(50, 50), Size.Empty);

                    //Action for each element detected
                    Parallel.For(0, facesDetected.Length, i =>
                    {
                        try
                        {
                            facesDetected[i].X += (int)(facesDetected[i].Height * 0.15);
                            facesDetected[i].Y += (int)(facesDetected[i].Width * 0.22);
                            facesDetected[i].Height -= (int)(facesDetected[i].Height * 0.3);
                            facesDetected[i].Width -= (int)(facesDetected[i].Width * 0.35);

                            result = currentFrame.Copy(facesDetected[i]).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                            result._EqualizeHist();
                            //draw the face detected in the 0th (gray) channel with blue color
                            currentFrame.Draw(facesDetected[i], new Bgr(Color.Red), 2);

                            if (Eigen_Recog.IsTrained)
                            {
                                string name = Eigen_Recog.Recognise(result);
                                int match_value = (int)Eigen_Recog.Get_Eigen_Distance;

                                txtName.Text = name;
                                //Draw the label for each face detected and recognized
                                currentFrame.Draw(name + " ", ref font, new Point(facesDetected[i].X - 2, facesDetected[i].Y - 2), new Bgr(Color.LightGreen));
                                //ADD_Face_Found(result, name, match_value);
                            }

                        }
                        catch(Exception ex) { 
                            //do nothing as parrellel loop buggy                        {

                            //No action as the error is useless, it is simply an error in 
                            //no data being there to process and this occurss sporadically 
                        }
                    });
                    //Show the faces procesed and recognized
                    image_PICBX.Image = currentFrame.ToBitmap();
                    //parent.txtName.Text = txtName.Text;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
    }
}
