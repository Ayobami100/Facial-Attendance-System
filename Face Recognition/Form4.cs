using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Data.SqlClient;
namespace Face_Recognition
{
    public partial class Form4 : MetroFramework.Forms.MetroForm
    {

        SqlCommand cmd;
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-18J3HJ4;Initial Catalog=FaceRecognition;Integrated Security=True");
        SqlDataAdapter da;
        SqlDataReader dr;

        public Form4()
        {
            InitializeComponent();
           
            LoadCombo();
            BindFirstGrid();

        }

        private void BindGrid()
        {
            
            var pt = comboBox1.SelectedItem.ToString();
            string[] pt1 = pt.Split(' ');
            string StaffId = pt1[0];
            con.Open();
            using (cmd = new SqlCommand("SelectallAttRecordForChart", con))
                {
                cmd.Parameters.Add("@StaffId", SqlDbType.NVarChar, 50).Value = StaffId;

                cmd.CommandType = CommandType.StoredProcedure;
                    using (da = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);
                            dataGridView1.DataSource = ds.Tables[0];
                        }
                    }
                }
            con.Close();


         
        }

        private void BindFirstGrid()
        {

            con.Open();
            using (cmd = new SqlCommand("SelectallStaffWithoutCon", con))
            {

                cmd.CommandType = CommandType.StoredProcedure;
                using (da = new SqlDataAdapter(cmd))
                {
                    using (DataSet ds = new DataSet())
                    {
                        da.Fill(ds);
                        dataGridView1.DataSource = ds.Tables[0];
                    }
                }
            }
            con.Close();



        }
        void BindChart()
        {
            
            chart1.Series["Present"].Points.Clear();
            var period = comboPeriod.SelectedItem.ToString();
            if(period == "FOR THE WEEK")
            {
                con.Open();


                using (cmd = new SqlCommand("SelectForWeekChart", con))
                {
                    cmd.Parameters.Add("@StaffId", SqlDbType.BigInt, 50).Value = dataGridView1.Rows[0].Cells[0].Value;
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (dr = cmd.ExecuteReader())
                    {
                        
                        while (dr.Read())
                        {
                           
                            this.chart1.Series["Present"].Points.AddXY("Present", dr["Present"].ToString());
                            this.chart1.Series["Present"].Points[0].Color = Color.Green;
                            ///////////////////////////////////////////////////////////////////////////////////////

                            int resultAbsent = 7 - Convert.ToInt32(dr["Present"]);
                            this.chart1.Series["Present"].Points.AddXY("Absent", resultAbsent);
                            this.chart1.Series["Present"].Points[1].Color = Color.Red;
                            //chart1.Titles.Add("Days");
                        }
                    }
                }
                con.Close();
            }
           else if (period == "FOR THE MONTH")
            {
                chart1.Series["Present"].Points.Clear();
                con.Open();


                using (cmd = new SqlCommand("SelectForMonthChart", con))
                {
                    cmd.Parameters.Add("@StaffId", SqlDbType.BigInt, 50).Value = dataGridView1.Rows[0].Cells[0].Value;
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                           
                            this.chart1.Series["Present"].Points.AddXY("Present", dr["Present"].ToString());
                            this.chart1.Series["Present"].Points[0].Color = Color.Green;

                            ///////////////////////////////////////////////////////////////////////////////////////


                            int resultAbsent = 31 - Convert.ToInt32(dr["Present"]);
                            this.chart1.Series["Present"].Points.AddXY("Absent", resultAbsent);
                            this.chart1.Series["Present"].Points[1].Color = Color.Red;
                            //chart1.Titles.Add("Days");
                        }
                    }
                }
                con.Close();
            }

            else if (period == "FOR THE YEAR")
            {
                chart1.Series["Present"].Points.Clear();

                con.Open();


                using (cmd = new SqlCommand("SelectForYearChart", con))
                {
                    cmd.Parameters.Add("@StaffId", SqlDbType.BigInt, 50).Value = dataGridView1.Rows[0].Cells[0].Value;
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {

                            this.chart1.Series["Present"].Points.AddXY("Present", dr["Present"].ToString());
                           // this.chart1.Series["Present"].Points[0].SetValueXY("Present", dr["Present"].ToString());
                            this.chart1.Series["Present"].Points[0].Color = Color.Green;

                            ///////////////////////////////////////////////////////////////////////////////////////

                            int resultAbsent = 365 - Convert.ToInt32(dr["Present"]);
                           // this.chart1.Series["Present"].Points[1].SetValueXY("Absent", resultAbsent.ToString());
                            this.chart1.Series["Present"].Points.AddXY("Absent", resultAbsent);
                            this.chart1.Series["Present"].Points[1].Color = Color.Red;
                            //chart1.Titles.Add("Days");
                        }
                    }
                }
                con.Close();
            }


        }
        void LoadCombo()
        {
            //Fill the combobox with the Staff Names 
            con.Open();
            using (cmd = new SqlCommand("SelectAllStaffForChat", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {

                        comboBox1.Items.Add(dr["StaffId"]+" "+dr["FirstName"] + " " + dr["LastName"] + " " + dr["OtherName"]);

                    }
                }
            }
            con.Close();

        }
        private void comboBox1_Leave(object sender, EventArgs e)
        {
                    //this.chart1.Series["Present"].Points.AddXY("name", 40);
                    //this.chart1.Series["Abscent"].Points.AddXY("name", 10);

                    //this.chart1.Series["Present"].Points.AddXY("name1", 50);
                    //this.chart1.Series["Abscent"].Points.AddXY("name1", 20);

        }

        private void Form4_Load(object sender, EventArgs e)
        {
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
           
           
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            try
            {
                BindGrid();
                BindChart();
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.ToString());
            }

        }
    }
}
