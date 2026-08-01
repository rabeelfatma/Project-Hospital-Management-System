using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace form
{
    public partial class Doctors : Form
    {
        private readonly SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=olx;Integrated Security=True");

        public Doctors()
        {
            InitializeComponent();
            textBox4.UseSystemPasswordChar = true;
        }

        private void Doctors_Load(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count == 0)
            {
                comboBox1.Items.Add("Male");
                comboBox1.Items.Add("Female");
            }
        }

        // Add Doctor
        private void Add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                comboBox1.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            if (textBox4.Text.Trim().Length != 6)
            {
                MessageBox.Show("Password must be exactly 6 characters.");
                return;
            }

            try
            {
                con.Open();
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM doctors WHERE email=@Email", con);
                checkCmd.Parameters.AddWithValue("@Email", textBox2.Text.Trim());
                int exists = (int)checkCmd.ExecuteScalar();

                if (exists > 0)
                {
                    MessageBox.Show("Doctor already exists. You can now update or continue to manage records.");
                    return;
                }

                SqlCommand cmd = new SqlCommand("INSERT INTO doctors (name, email, gender, password, specialization, joiningdate) VALUES (@Name, @Email, @Gender, @Password, @Specialization, @JoiningDate)", con);
                cmd.Parameters.AddWithValue("@Name", textBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", textBox2.Text.Trim());
                cmd.Parameters.AddWithValue("@Gender", comboBox1.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@Password", textBox4.Text.Trim());
                cmd.Parameters.AddWithValue("@Specialization", textBox5.Text.Trim());
                cmd.Parameters.AddWithValue("@JoiningDate", dateTimePicker1.Value);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Doctor added successfully.");
                    LoadDoctorsData();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Failed to add doctor.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding doctor: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // Update Doctor (validation removed)
        private void Update_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE doctors SET name=@Name, gender=@Gender, password=@Password, specialization=@Specialization, joiningdate=@JoiningDate WHERE email=@Email", con);
                cmd.Parameters.AddWithValue("@Name", textBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@Gender", comboBox1.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@Password", textBox4.Text.Trim());
                cmd.Parameters.AddWithValue("@Specialization", textBox5.Text.Trim());
                cmd.Parameters.AddWithValue("@JoiningDate", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@Email", textBox2.Text.Trim());

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Doctor updated successfully.");
                    LoadDoctorsData();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Doctor not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating doctor: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // Delete Doctor (validation removed)
        private void Delete_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM doctors WHERE email=@Email", con);
                cmd.Parameters.AddWithValue("@Email", textBox2.Text.Trim());

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Doctor deleted successfully.");
                    LoadDoctorsData();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Doctor not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting doctor: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // View Doctors Button
        private void ViewDoctors_Click(object sender, EventArgs e)
        {
            LoadDoctorsData();
        }

        // Refresh Button
        private void Refresh_Click(object sender, EventArgs e)
        {
            LoadDoctorsData();
            ClearFields();
        }

        // Load all doctors into dataGridView1
        private void LoadDoctorsData()
        {
            try
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM doctors", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // Clear all input fields
        private void ClearFields()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox1.SelectedIndex = -1;
            textBox4.Text = "";
            textBox5.Text = "";
            dateTimePicker1.Value = DateTime.Now;
        }

        // Select row from dataGridView1 and load into textboxes
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox1.Text = row.Cells["name"].Value.ToString();
                textBox2.Text = row.Cells["email"].Value.ToString();
                comboBox1.SelectedItem = row.Cells["gender"].Value.ToString();
                textBox4.Text = row.Cells["password"].Value.ToString();
                textBox5.Text = row.Cells["specialization"].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(row.Cells["joiningdate"].Value);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Form2 f1 = new Form2();
            f1.Show();
            this.Hide();
        }

        private void Search_Click(object sender, EventArgs e)
        {
            string patientId = textBox6.Text.Trim();
            if (string.IsNullOrEmpty(patientId))
            {
                MessageBox.Show("Enter a Patient ID to search.");
                return;
            }

            try
            {
                con.Open();
                SqlDataAdapter da1 = new SqlDataAdapter("SELECT * FROM patients WHERE patientid=@pid", con);
                da1.SelectCommand.Parameters.AddWithValue("@pid", patientId);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                MessageBox.Show(dt1.Rows.Count > 0 ? "Patient found." : "Patient not found.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.UseSystemPasswordChar = !checkBox1.Checked;
        }

        // Unused events (optional to remove)
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void button6_Click(object sender, EventArgs e) { }
    }
}