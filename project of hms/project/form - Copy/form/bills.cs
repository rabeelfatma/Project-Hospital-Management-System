using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
namespace form
{
    public partial class bills : Form
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=olx;Integrated Security=True";

        public bills()
        {
            InitializeComponent();
        }

        private void bills_Load(object sender, EventArgs e)
        {
            textBox3.Enabled = false; // Disable Payment Status TextBox
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Patient ID is required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Amount is required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Bill date is required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(textBox2.Text.Trim(), out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid positive amount.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!DateTime.TryParse(textBox4.Text.Trim(), out DateTime billDate))
            {
                MessageBox.Show("Please enter a valid bill date.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            string patientId = textBox1.Text.Trim();
            decimal amount = decimal.Parse(textBox2.Text.Trim());
            DateTime billDate = DateTime.Parse(textBox4.Text.Trim());
            string paymentStatus = "Unpaid";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string insertQuery = @"INSERT INTO bills (patientid, amount, paymentstatus, billdate)
                                           VALUES (@pid, @amt, @status, @bdate)";

                    SqlCommand cmd = new SqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@pid", patientId);
                    cmd.Parameters.AddWithValue("@amt", amount);
                    cmd.Parameters.AddWithValue("@status", paymentStatus);
                    cmd.Parameters.AddWithValue("@bdate", billDate);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("Bill added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        LoadBillsData();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add bill.", "Insert Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            LoadBillsData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadBillsData();
        }

        private void LoadBillsData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string selectQuery = "SELECT * FROM bills";
                    SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            string patientId = textBox1.Text.Trim();
            decimal amount = decimal.Parse(textBox2.Text.Trim());
            DateTime billDate = DateTime.Parse(textBox4.Text.Trim());
            string paymentStatus = "Paid";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string updateQuery = @"UPDATE bills 
                                           SET paymentstatus = @status 
                                           WHERE patientid = @pid AND amount = @amt AND billdate = @bdate";

                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@pid", patientId);
                    cmd.Parameters.AddWithValue("@amt", amount);
                    cmd.Parameters.AddWithValue("@status", paymentStatus);
                    cmd.Parameters.AddWithValue("@bdate", billDate);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("Bill updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        LoadBillsData();
                    }
                    else
                    {
                        MessageBox.Show("No matching bill to update.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            string patientId = textBox1.Text.Trim();
            decimal amount = decimal.Parse(textBox2.Text.Trim());
            DateTime billDate = DateTime.Parse(textBox4.Text.Trim());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string deleteQuery = @"DELETE FROM bills 
                                           WHERE patientid = @pid AND amount = @amt AND billdate = @bdate";

                    SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                    cmd.Parameters.AddWithValue("@pid", patientId);
                    cmd.Parameters.AddWithValue("@amt", amount);
                    cmd.Parameters.AddWithValue("@bdate", billDate);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("Bill deleted successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        LoadBillsData();
                    }
                    else
                    {
                        MessageBox.Show("No matching bill to delete.", "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchPatientId = textBox5.Text.Trim();

            if (string.IsNullOrEmpty(searchPatientId))
            {
                MessageBox.Show("Please enter a patient ID to search.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string searchQuery = "SELECT * FROM bills WHERE patientid = @pid";
                    SqlDataAdapter adapter = new SqlDataAdapter(searchQuery, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@pid", searchPatientId);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = dt;
                    }
                    else
                    {
                        MessageBox.Show("No bills found for this patient ID.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dataGridView1.DataSource = null;
                    }

                    textBox5.Clear(); // Optional: clear search box
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Are you sure you want to go to Form2?", "Go to Form2", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                Form2 form2 = new Form2();
                form2.Show();
                this.Hide();
            }
        }


        private void ClearForm()
        {
            textBox1.Clear(); // patientid
            textBox2.Clear(); // amount
            textBox3.Clear(); // payment status
            textBox4.Clear(); // bill date
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Optional text change logic
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // Optional panel paint logic
        }
    }
}
