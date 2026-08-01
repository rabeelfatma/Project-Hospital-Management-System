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
    public partial class Patients : Form
    {
        SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=olx;Integrated Security=True");

        public Patients()
        {
            InitializeComponent();
            textBox3.PasswordChar = '*'; // Hide password initially
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dddd, dd MMMM yyyy hh:mm:ss tt";
        }

        private void patients_Load(object sender, EventArgs e)
        {
            // Optional on load
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count == 0)
            {
                comboBox1.Items.Add("Male");
                comboBox1.Items.Add("Female");
            }
        }

        private void LoadPatientsData()
        {
            try
            {
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM patients", con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                dataGridView1.DataSource = dt;
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                con.Close();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                comboBox1.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Empty Fields Are Not Allowed.");
                return;
            }

            if (textBox3.Text.Length != 6)
            {
                MessageBox.Show("Password must be exactly 6 characters.");
                return;
            }

            try
            {
                con.Open();
                string query = "INSERT INTO patients (name, gender, address, password, email, registrationdatetime) " +
                               "VALUES (@name, @gender, @address, @password, @email, @registrationdatetime)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", textBox1.Text);
                cmd.Parameters.AddWithValue("@gender", comboBox1.Text);
                cmd.Parameters.AddWithValue("@address", textBox2.Text);
                cmd.Parameters.AddWithValue("@password", textBox3.Text);
                cmd.Parameters.AddWithValue("@email", textBox4.Text);
                cmd.Parameters.AddWithValue("@registrationdatetime", dateTimePicker1.Value);
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Patient Added Successfully!");
                ClearForm();
                LoadPatientsData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                con.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string query = "UPDATE patients SET name=@name, gender=@gender, address=@address, password=@password, registrationdatetime=@registrationdatetime WHERE email=@email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", textBox1.Text);
                cmd.Parameters.AddWithValue("@gender", comboBox1.Text);
                cmd.Parameters.AddWithValue("@address", textBox2.Text);
                cmd.Parameters.AddWithValue("@password", textBox3.Text);
                cmd.Parameters.AddWithValue("@registrationdatetime", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@email", textBox4.Text);
                int result = cmd.ExecuteNonQuery();
                con.Close();

                if (result > 0)
                {
                    MessageBox.Show("Patient Updated Successfully!");
                    ClearForm();
                    LoadPatientsData();
                }
                else
                {
                    MessageBox.Show("No Patient Found with This Email.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                con.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string query = "DELETE FROM patients WHERE email=@email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", textBox4.Text);
                int result = cmd.ExecuteNonQuery();
                con.Close();

                if (result > 0)
                {
                    MessageBox.Show("Patient Deleted Successfully!");
                    ClearForm();
                    LoadPatientsData();
                }
                else
                {
                    MessageBox.Show("No Patient Found with This Email.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                con.Close();
            }
        }

        private void btnShowData_Click(object sender, EventArgs e)
        {
            LoadPatientsData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string query = "SELECT * FROM patients WHERE email=@email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", textBox5.Text);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                dataGridView1.DataSource = dt;
                con.Close();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No Patient Found with This Email.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                con.Close();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadPatientsData();
            dataGridView2.DataSource = null; // Clear Diagnosis grid when refreshing
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox3.PasswordChar = '\0';
            }
            else
            {
                textBox3.PasswordChar = '*';
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox1.Text = row.Cells["name"].Value.ToString();
                comboBox1.Text = row.Cells["gender"].Value.ToString();
                textBox2.Text = row.Cells["address"].Value.ToString();
                textBox3.Text = row.Cells["password"].Value.ToString();
                textBox4.Text = row.Cells["email"].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(row.Cells["registrationdatetime"].Value);
            }
        }

        private void ClearForm()
        {
            textBox1.Text = "";
            comboBox1.SelectedIndex = -1;
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox3.PasswordChar = '*';
            checkBox1.Checked = false;
            dateTimePicker1.Value = DateTime.Now;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // Optional
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Optional
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Optional unused
        }

        private void label5_Click(object sender, EventArgs e)
        {
            // Optional unused
        }

        // ✅ NEW: Show Diagnosis Data button for dataGridView2
        private void btnShowDiagnosisData_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM diagnosis", con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                dataGridView2.DataSource = dt;
                con.Close();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No Diagnosis Records Found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                con.Close();
            }
        }
    }
}
