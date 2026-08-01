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
    public partial class Diagnosis : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=olx;Integrated Security=True");

        public Diagnosis()
        {
            InitializeComponent();
        }

        private void Diagnosis_Load(object sender, EventArgs e)
        {
            LoadPatientIds();
            LoadDoctorIds();
            LoadConditions();
            LoadDiagnosisIds();
            dateTimePicker1.Value = DateTime.Now;
        }

        private void LoadPatientIds()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT patientid FROM patients", con);
                SqlDataReader rdr = cmd.ExecuteReader();
                comboBox1.Items.Clear();
                while (rdr.Read())
                {
                    comboBox1.Items.Add(rdr["patientid"].ToString());
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private void LoadDoctorIds()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT doctorid FROM doctors", con);
                SqlDataReader rdr = cmd.ExecuteReader();
                comboBox2.Items.Clear();
                while (rdr.Read())
                {
                    comboBox2.Items.Add(rdr["doctorid"].ToString());
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private void LoadConditions()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT condition FROM medicalrecords", con);
                SqlDataReader rdr = cmd.ExecuteReader();
                comboBox4.Items.Clear();
                while (rdr.Read())
                {
                    comboBox4.Items.Add(rdr["condition"].ToString());
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private void LoadDiagnosisIds()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT diagnosisid FROM diagnosis", con);
                SqlDataReader rdr = cmd.ExecuteReader();
                comboBox3.Items.Clear();
                while (rdr.Read())
                {
                    comboBox3.Items.Add(rdr["diagnosisid"].ToString());
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private void LoadDiagnosisData()
        {
            try
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM diagnosis", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(comboBox1.Text) ||
                string.IsNullOrWhiteSpace(comboBox2.Text) ||
                string.IsNullOrWhiteSpace(comboBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text))
            {
                return false;
            }
            return true;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
            {
                MessageBox.Show("Please fill in all fields before adding the diagnosis record.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO diagnosis (patientid, doctorid, diagnosisdatetime, condition, medication, surgeries) VALUES (@patientid, @doctorid, @diagnosisdatetime, @condition, @medication, @surgeries)", con);
                cmd.Parameters.AddWithValue("@patientid", comboBox1.Text);
                cmd.Parameters.AddWithValue("@doctorid", comboBox2.Text);
                cmd.Parameters.AddWithValue("@diagnosisdatetime", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@condition", comboBox4.Text);
                cmd.Parameters.AddWithValue("@medication", textBox5.Text);
                cmd.Parameters.AddWithValue("@surgeries", textBox6.Text);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Diagnosis record added successfully.");
                LoadDiagnosisIds();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text == "")
            {
                MessageBox.Show("Please select a Diagnosis ID to update.");
                return;
            }

            if (!ValidateFields())
            {
                MessageBox.Show("Please fill in all fields before updating the diagnosis record.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE diagnosis SET patientid=@patientid, doctorid=@doctorid, diagnosisdatetime=@diagnosisdatetime, condition=@condition, medication=@medication, surgeries=@surgeries WHERE diagnosisid=@diagnosisid", con);
                cmd.Parameters.AddWithValue("@diagnosisid", comboBox3.Text);
                cmd.Parameters.AddWithValue("@patientid", comboBox1.Text);
                cmd.Parameters.AddWithValue("@doctorid", comboBox2.Text);
                cmd.Parameters.AddWithValue("@diagnosisdatetime", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@condition", comboBox4.Text);
                cmd.Parameters.AddWithValue("@medication", textBox5.Text);
                cmd.Parameters.AddWithValue("@surgeries", textBox6.Text);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Diagnosis record updated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text == "")
            {
                MessageBox.Show("Please select a Diagnosis ID to delete.");
                return;
            }

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM diagnosis WHERE diagnosisid=@diagnosisid", con);
                cmd.Parameters.AddWithValue("@diagnosisid", comboBox3.Text);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Diagnosis record deleted successfully.");
                LoadDiagnosisIds();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            LoadPatientIds();
            LoadDoctorIds();
            LoadConditions();
            LoadDiagnosisIds();
            dateTimePicker1.Value = DateTime.Now;
        }

        private void buttonShowDiagnosisData_Click(object sender, EventArgs e)
        {
            LoadDiagnosisData();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM diagnosis WHERE patientid LIKE @patientid", con);
                da.SelectCommand.Parameters.AddWithValue("@patientid", "%" + textBox7.Text + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // Optional
        }

        private void label5_Click(object sender, EventArgs e)
        {
            // Optional
        }
    }
}
