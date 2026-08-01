using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace form
{
    public partial class medicalrecords : Form
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=olx;Integrated Security=True";

        public medicalrecords()
        {
            InitializeComponent();
            LoadPatientIds();
            LoadDoctorIds();

            textBox5.Text = "Pending";
            textBox6.Text = "Pending";
            textBox5.Enabled = false;
            textBox6.Enabled = false;

            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dddd, yyyy-MM-dd HH:mm:ss";
            dateTimePicker1.Value = DateTime.Now;
        }

        private void LoadPatientIds()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT patientid FROM patients";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader["patientid"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading patient IDs: " + ex.Message);
                }
            }
        }

        private void LoadDoctorIds()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT doctorid FROM doctors";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        comboBox2.Items.Add(reader["doctorid"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading doctor IDs: " + ex.Message);
                }
            }
        }

        private void medicalrecords_Load(object sender, EventArgs e) { }

        private void btnExit_Click(object sender, EventArgs e)
        {
            new Form2().Show();
            this.Close();
        }

        private void clearForm()
        {
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            textBox4.Clear();
            textBox7.Clear();
            textBox5.Text = "Pending";
            textBox6.Text = "Pending";
            dateTimePicker1.Value = DateTime.Now;
        }

        private bool validateInputs()
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select Patient ID.");
                return false;
            }
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Select Doctor ID.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Enter Condition.");
                return false;
            }
            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!validateInputs()) return;

            DateTime recordDateTime = dateTimePicker1.Value;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string checkQuery = @"SELECT COUNT(*) FROM medicalrecords
                        WHERE patientid=@pid AND doctorid=@did AND recorddatetime=@datetime AND condition=@cond";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@pid", comboBox1.SelectedItem.ToString());
                    checkCmd.Parameters.AddWithValue("@did", comboBox2.SelectedItem.ToString());
                    checkCmd.Parameters.AddWithValue("@datetime", recordDateTime);
                    checkCmd.Parameters.AddWithValue("@cond", textBox4.Text.Trim());

                    if ((int)checkCmd.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Record already exists!");
                        return;
                    }

                    string insertQuery = @"INSERT INTO medicalrecords
                        (patientid, doctorid, recorddatetime, condition, medication, surgeries)
                        VALUES (@pid, @did, @datetime, @cond, @med, @surg)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@pid", comboBox1.SelectedItem.ToString());
                    insertCmd.Parameters.AddWithValue("@did", comboBox2.SelectedItem.ToString());
                    insertCmd.Parameters.AddWithValue("@datetime", recordDateTime);
                    insertCmd.Parameters.AddWithValue("@cond", textBox4.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@med", "Pending");
                    insertCmd.Parameters.AddWithValue("@surg", "Pending");

                    if (insertCmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Record added successfully!");
                        clearForm();
                        // Removed navigation to Doctors form
                    }
                    else
                    {
                        MessageBox.Show("Failed to add record.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnShowMedicalrecords_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string selectQuery = @"SELECT * FROM medicalrecords ORDER BY recorddatetime DESC";
                    SqlCommand selectCmd = new SqlCommand(selectQuery, conn);

                    SqlDataAdapter adapter = new SqlDataAdapter(selectCmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;

                    if (dt.Rows.Count == 0)
                        MessageBox.Show("No records found.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox7.Text))
            {
                MessageBox.Show("Please enter Patient ID to search.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string searchQuery = @"SELECT * FROM medicalrecords WHERE patientid=@pid ORDER BY recorddatetime DESC";
                    SqlCommand cmd = new SqlCommand(searchQuery, conn);
                    cmd.Parameters.AddWithValue("@pid", textBox7.Text.Trim());

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;

                    if (dt.Rows.Count == 0)
                        MessageBox.Show("No records found for Patient ID: " + textBox7.Text.Trim());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string selectQuery = @"SELECT * FROM medicalrecords ORDER BY recorddatetime DESC";
                    SqlCommand cmd = new SqlCommand(selectQuery, conn);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;

                    if (dt.Rows.Count == 0)
                        MessageBox.Show("No records found.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a row to update.");
                return;
            }

            DataGridViewRow row = dataGridView1.SelectedRows[0];
            string pid = row.Cells["patientid"].Value.ToString();
            string did = row.Cells["doctorid"].Value.ToString();
            string datetime = row.Cells["recorddatetime"].Value.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string updateQuery = @"UPDATE medicalrecords
                        SET condition=@cond, medication=@med, surgeries=@surg
                        WHERE patientid=@pid AND doctorid=@did AND recorddatetime=@datetime";

                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@pid", pid);
                    cmd.Parameters.AddWithValue("@did", did);
                    cmd.Parameters.AddWithValue("@datetime", Convert.ToDateTime(datetime));
                    cmd.Parameters.AddWithValue("@cond", row.Cells["condition"].Value.ToString());
                    cmd.Parameters.AddWithValue("@med", row.Cells["medication"].Value.ToString());
                    cmd.Parameters.AddWithValue("@surg", row.Cells["surgeries"].Value.ToString());

                    if (cmd.ExecuteNonQuery() > 0)
                        MessageBox.Show("Record updated.");
                    else
                        MessageBox.Show("Update failed.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a row to delete.");
                return;
            }

            DataGridViewRow row = dataGridView1.SelectedRows[0];
            string pid = row.Cells["patientid"].Value.ToString();
            string did = row.Cells["doctorid"].Value.ToString();
            string datetime = row.Cells["recorddatetime"].Value.ToString();

            DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Confirm", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string deleteQuery = @"DELETE FROM medicalrecords
                        WHERE patientid=@pid AND doctorid=@did AND recorddatetime=@datetime";
                    SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                    cmd.Parameters.AddWithValue("@pid", pid);
                    cmd.Parameters.AddWithValue("@did", did);
                    cmd.Parameters.AddWithValue("@datetime", Convert.ToDateTime(datetime));

                    if (cmd.ExecuteNonQuery() > 0)
                        MessageBox.Show("Record deleted.");
                    else
                        MessageBox.Show("Deletion failed.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // Unused Event Handlers
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void panel4_Paint(object sender, PaintEventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void button7_Click(object sender, EventArgs e) { }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
