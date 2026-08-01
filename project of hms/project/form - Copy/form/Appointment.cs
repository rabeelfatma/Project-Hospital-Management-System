using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace form
{
    public partial class Appointment : Form
    {
        private readonly SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=olx;Integrated Security=True");

        public Appointment()
        {
            InitializeComponent();
            LoadPatientIds();
            LoadDoctorIds();
        }

        private void Appointment_Load(object sender, EventArgs e)
        {
            textBox4.Text = "Pending";
            textBox4.Enabled = false;

            // Display both date and time in DateTimePicker
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
        }

        private void LoadPatientIds()
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT patientid FROM patients", con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["patientid"].ToString());
                }
            }
            con.Close();
        }

        private void LoadDoctorIds()
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT doctorid FROM doctors", con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    comboBox2.Items.Add(reader["doctorid"].ToString());
                }
            }
            con.Close();
        }

        private void BtnInsert_Click(object sender, EventArgs e)
        {
            // Validation: Check if any field is empty
            if (string.IsNullOrWhiteSpace(comboBox1.Text) || string.IsNullOrWhiteSpace(comboBox2.Text))
            {
                MessageBox.Show("Please select both Patient ID and Doctor ID. Empty fields are not allowed.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            con.Open();
            using (SqlCommand cmd = new SqlCommand("INSERT INTO appointments (patientid, doctorid, appointmentdatetime, status) VALUES (@patientid, @doctorid, @appointmentdatetime, @status)", con))
            {
                cmd.Parameters.AddWithValue("@patientid", comboBox1.Text);
                cmd.Parameters.AddWithValue("@doctorid", comboBox2.Text);
                cmd.Parameters.AddWithValue("@appointmentdatetime", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@status", textBox4.Text);
                cmd.ExecuteNonQuery();
            }
            con.Close();
            MessageBox.Show("Appointment Inserted Successfully");
            ShowAppointments();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("UPDATE appointments SET doctorid=@doctorid, appointmentdatetime=@appointmentdatetime WHERE patientid=@patientid", con))
            {
                cmd.Parameters.AddWithValue("@patientid", comboBox1.Text);
                cmd.Parameters.AddWithValue("@doctorid", comboBox2.Text);
                cmd.Parameters.AddWithValue("@appointmentdatetime", dateTimePicker1.Value);
                cmd.ExecuteNonQuery();
            }
            con.Close();
            MessageBox.Show("Appointment Updated Successfully");
            ShowAppointments();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            con.Open();
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM appointments WHERE patientid=@patientid", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@patientid", textBox5.Text);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            con.Close();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("DELETE FROM appointments WHERE patientid=@patientid", con))
            {
                cmd.Parameters.AddWithValue("@patientid", comboBox1.Text);
                cmd.ExecuteNonQuery();
            }
            con.Close();
            MessageBox.Show("Appointment Deleted Successfully");
            ShowAppointments();
        }

        private void BtnShowAppointmentData_Click(object sender, EventArgs e)
        {
            ShowAppointments();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            comboBox1.Text = "";
            comboBox2.Text = "";
            textBox4.Text = "Pending";
            textBox5.Text = "";
            ShowAppointments();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
            this.Hide();
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("UPDATE appointments SET status='Accepted' WHERE patientid=@patientid", con))
            {
                cmd.Parameters.AddWithValue("@patientid", comboBox1.Text);
                cmd.ExecuteNonQuery();
            }
            con.Close();
            MessageBox.Show("Appointment Accepted");
            ShowAppointments();
        }

        private void BtnReject_Click(object sender, EventArgs e)
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("UPDATE appointments SET status='Rejected' WHERE patientid=@patientid", con))
            {
                cmd.Parameters.AddWithValue("@patientid", comboBox1.Text);
                cmd.ExecuteNonQuery();
            }
            con.Close();
            MessageBox.Show("Appointment Rejected");
            ShowAppointments();
        }

        private void ShowAppointments()
        {
            con.Open();
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM appointments", con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            con.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // This method can remain empty if no action is needed
        }
    }
}