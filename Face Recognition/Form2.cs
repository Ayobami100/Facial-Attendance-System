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

using System.Data.SqlClient;
namespace Face_Recognition
{
    public partial class Form2 : MetroFramework.Forms.MetroForm
    {

        #region variables
        Image<Bgr, Byte> currentFrame; //current image aquired from webcam for display
        Image<Gray, byte> result, TrainedFace = null; //used to store the result image and trained face
        Image<Gray, byte> gray_frame = null; //grayscale current image aquired from webcam for processing

        Capture grabber; //This is our capture variable

        public CascadeClassifier Face = new CascadeClassifier(Application.StartupPath + "/Cascades/haarcascade_frontalface_default.xml");//Our face detection method 

        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_COMPLEX, 0.5, 0.5); //Our fount for writing within the frame

        int NumLabels;

        //Classifier with default training location
        Classifier_Train Eigen_Recog = new Classifier_Train();

        SqlCommand cmd;
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-18J3HJ4;Initial Catalog=FaceRecognition;Integrated Security=True");
        SqlDataAdapter da;
        SqlDataReader dr;

        #endregion
        int i = 1;
        public Form2()
        {
            InitializeComponent();

            timer1.Start();

            if (Eigen_Recog.IsTrained)
            {
                message_bar.Text = "Training Data loaded";
            }
            else
            {
                message_bar.Text = "No training data found, please train program using Train menu option";
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] StaffName = txtName.Text.Split(' ');           
            string StaffId = "";
            try
            {
                 if(txtName.Text != "")
                 {
                    con.Open();
                    cmd = new SqlCommand("SelectallStaff", con);
                    cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = StaffName[0];
                    cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = StaffName[1];
                    cmd.Parameters.Add("@OtherName", SqlDbType.NVarChar, 50).Value = StaffName[2];
                    cmd.CommandType = CommandType.StoredProcedure;
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        StaffId = dr["StaffId"].ToString();

                    }
                    con.Close();
                 }            
            
           
                string[] rowToAdd = { i.ToString(), txtName.Text, DateTime.Now.ToString("hh: mm:ss tt") };
                for (int j = 0; j < i; j++)
                {
                        for (int k = 0; k < i; k++)
                        {
                            if (dataGridView1.Rows[k].Cells[1].Value != null && txtName.Text != "" && txtName.Text == dataGridView1.Rows[k].Cells[1].Value.ToString())
                            {
                                //MessageBox.Show(dataGridView1.Rows[k].Cells[1].Value.ToString());
                                dataGridView1.Rows[k].Cells[3].Value = DateTime.Now.ToString("hh: mm:ss tt");
                                

                                //UPDATE TIMEOUT
                                cmd = new SqlCommand("UpdateStaffAttendance", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@StaffId", SqlDbType.NVarChar, 50).Value = StaffId;
                                cmd.Parameters.Add("@TimeOutt", SqlDbType.NVarChar, 50).Value = DateTime.Now.ToString("hh: mm:ss tt");
                               // cmd.Parameters.Add("@StaffId", SqlDbType.NVarChar, 50).Value = txtName.Text;

                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                                txtName.Text = "";


                        }
                    }
                    if (txtName.Text != "")
                    {
                        dataGridView1.Rows.Add(rowToAdd);
                        
                        i++;
                        //INSERT DATAGRIDVIEW ITEMS TO DATABASE
                        cmd = new SqlCommand("InsertStaffAttendance", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@StaffId", SqlDbType.NVarChar, 50).Value = StaffId;
                        cmd.Parameters.Add("@TimeIn", SqlDbType.NVarChar, 50).Value = rowToAdd[2];
                        cmd.Parameters.Add("@TimeOut", SqlDbType.NVarChar, 50).Value = "0";
                        cmd.Parameters.Add("@Dt", SqlDbType.Date, 50).Value = DateTime.Now.ToString("dddd, dd MMMM yyyy");

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        MessageBox.Show("SUCCESSFULL", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtName.Text = "";

                    }
                    break;
                }
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                con.Close();
            }
        }
      

        private void btnTake_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
            //this.Dispose();
        }

        private void trainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();
        }

        private void Form2_Activated(object sender, EventArgs e)
        {
            if(Form3.SetValueForText1 != "")
            {
                txtName.Text = Form3.SetValueForText1;
            }
        }

        private void attendanceRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.Show();
        }

        private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Training_Form t1 = new Training_Form(this);
            t1.Show();
           
        }
    }
}
