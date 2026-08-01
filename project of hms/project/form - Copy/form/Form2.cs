// Namespaces
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace form
{
    public partial class Form2 : Form
    {
        private readonly string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=olx;Integrated Security=True";
        private string currentTable = "";
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;

        public Form2()
        {
            InitializeComponent();
            LoadDashboardCounts();
            dataGridView1.RowValidated += dataGridView1_RowValidated;
        }

        private void Form2_Load(object sender, EventArgs e) { }

        private void LoadDashboardCounts()
        {
            LoadTotal("patients", label1, "Total Patients: ");
            LoadTotal("doctors", label2, "Total Doctors: ");
            LoadTotal("appointments", label4, "Total Appointments: ");
            LoadTotal("medicalrecords", label5, "Total Medical Records: ");
            LoadTotal("bills", label7, "Total Bills: ");
            LoadTotal("users", label8, "Total Users: ");
            LoadTotal("diagnosis", label9, "Total Diagnosis: ");

            UpdateBarChart();
        }

        private void LoadTotal(string tableName, Label label, string labelPrefix)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM {tableName}", conn);
                    int total = (int)cmd.ExecuteScalar();
                    label.Text = labelPrefix + total;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading {tableName}: {ex.Message}");
            }
        }

        private int GetTotal(string tableName)
        {
            int count = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM {tableName}", conn);
                    count = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error counting {tableName}: {ex.Message}");
            }
            return count;
        }

        private void UpdateBarChart()
        {
            chartDashboard.Series.Clear();

            Series series = new Series("Totals");
            series.ChartType = SeriesChartType.Bar;

            int totalPatients = GetTotal("patients");
            int totalDoctors = GetTotal("doctors");
            int totalAppointments = GetTotal("appointments");
            int totalMedicalRecords = GetTotal("medicalrecords");
            int totalBills = GetTotal("bills");
            int totalUsers = GetTotal("users");
            int totalDiagnosis = GetTotal("diagnosis");

            series.Points.AddXY("Patients", totalPatients);
            series.Points.AddXY("Doctors", totalDoctors);
            series.Points.AddXY("Appointments", totalAppointments);
            series.Points.AddXY("Records", totalMedicalRecords);
            series.Points.AddXY("Bills", totalBills);
            series.Points.AddXY("Users", totalUsers);
            series.Points.AddXY("Diagnosis", totalDiagnosis);

            chartDashboard.Series.Add(series);
        }

        private void LoadTableData(string tableName)
        {
            try
            {
                currentTable = tableName;

                SqlConnection conn = new SqlConnection(connectionString);
                dataAdapter = new SqlDataAdapter($"SELECT * FROM {tableName}", conn);
                SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(dataAdapter);

                dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void buttonShowPatients_Click(object sender, EventArgs e) => LoadTableData("patients");
        private void buttonShowDoctors_Click(object sender, EventArgs e) => LoadTableData("doctors");
        private void buttonShowAppointments_Click(object sender, EventArgs e) => LoadTableData("appointments");
        private void buttonShowMedicalRecords_Click(object sender, EventArgs e) => LoadTableData("medicalrecords");
        private void buttonShowBills_Click(object sender, EventArgs e) => LoadTableData("bills");
        private void buttonShowUsersData_Click(object sender, EventArgs e) => LoadTableData("users");
        private void buttonUsers_Click(object sender, EventArgs e) => LoadTableData("users");

        // Show Diagnosis Data button functionality
        private void buttonShowDiagnosisData_Click(object sender, EventArgs e) => LoadTableData("diagnosis");

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentTable))
            {
                MessageBox.Show("Please load a table first.");
                return;
            }

            if (dataTable == null)
            {
                MessageBox.Show("No data loaded.");
                return;
            }

            DataRow newRow = dataTable.NewRow();
            dataTable.Rows.Add(newRow);

            int newRowIndex = dataGridView1.Rows.Count - 1;
            int firstEditableColIndex = 1;

            if (dataGridView1.ColumnCount > 1)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[newRowIndex].Cells[firstEditableColIndex];
                dataGridView1.BeginEdit(true);
            }
        }

        private void dataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (string.IsNullOrEmpty(currentTable) || dataTable == null)
                return;

            if (e.RowIndex < 0 || e.RowIndex >= dataTable.Rows.Count)
                return;

            DataRow row = dataTable.Rows[e.RowIndex];

            try
            {
                if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
                {
                    for (int i = 1; i < dataTable.Columns.Count; i++)
                    {
                        if (string.IsNullOrWhiteSpace(row[i].ToString()))
                        {
                            MessageBox.Show($"Column '{dataTable.Columns[i].ColumnName}' cannot be empty.");
                            return;
                        }
                    }

                    for (int i = 1; i < dataTable.Columns.Count; i++)
                    {
                        if (dataTable.Columns[i].ColumnName.ToLower().Contains("email"))
                        {
                            if (!IsValidEmail(row[i].ToString()))
                            {
                                MessageBox.Show($"Invalid email format in column '{dataTable.Columns[i].ColumnName}'.");
                                return;
                            }
                        }
                    }

                    dataAdapter.Update(dataTable);
                    dataTable.AcceptChanges();
                    LoadDashboardCounts();

                    if (row.RowState == DataRowState.Added)
                        MessageBox.Show("Row added successfully.");
                    else
                        MessageBox.Show("Row updated successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving changes: " + ex.Message);
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentTable) || dataTable == null)
            {
                MessageBox.Show("Load a table first.");
                return;
            }

            try
            {
                DataTable changes = dataTable.GetChanges(DataRowState.Modified);

                if (changes == null || changes.Rows.Count == 0)
                {
                    MessageBox.Show("No changes to update.");
                    return;
                }

                dataAdapter.Update(changes);
                dataTable.AcceptChanges();
                LoadDashboardCounts();
                MessageBox.Show("Update successful.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating: " + ex.Message);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentTable) || dataTable == null)
            {
                MessageBox.Show("Load a table first.");
                return;
            }

            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a row to delete.");
                return;
            }

            try
            {
                foreach (DataGridViewRow dgvRow in dataGridView1.SelectedRows)
                {
                    if (!dgvRow.IsNewRow)
                    {
                        DataRow row = ((DataRowView)dgvRow.DataBoundItem).Row;
                        row.Delete();
                    }
                }

                dataAdapter.Update(dataTable);
                dataTable.AcceptChanges();

                LoadDashboardCounts();
                MessageBox.Show("Row(s) deleted successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting row(s): " + ex.Message);
            }
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            Form1 home = new Form1();
            home.Show();
            this.Hide();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonPatients_Click(object sender, EventArgs e)
        {
            Patients p = new Patients();
            p.Show();
            this.Hide();
        }

        private void buttonDoctors_Click(object sender, EventArgs e)
        {
            Doctors d = new Doctors();
            d.Show();
            this.Hide();
        }

        private void buttonAppointment_Click(object sender, EventArgs e)
        {
            Appointment a = new Appointment();
            a.Show();
            this.Hide();
        }

        private void buttonMedicalRecord_Click(object sender, EventArgs e)
        {
            medicalrecords m = new medicalrecords();
            m.Show();
            this.Hide();
        }

        private void buttonBills_Click(object sender, EventArgs e)
        {
            bills b = new bills();
            b.Show();
            this.Hide();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentTable))
            {
                MessageBox.Show("No table is currently loaded.");
                return;
            }

            DialogResult confirmResult = MessageBox.Show("Are you sure you want to delete all records from this table?",
                                                         "Confirm Delete",
                                                         MessageBoxButtons.YesNo,
                                                         MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand($"DELETE FROM {currentTable}", conn);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        MessageBox.Show($"{rowsAffected} row(s) deleted from '{currentTable}'.");

                        LoadTableData(currentTable);
                        LoadDashboardCounts();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting records: " + ex.Message);
                }
            }
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Hide();
        }

        // Diagnosis form opening
        private void buttonDiagnosis_Click(object sender, EventArgs e)
        {
            Diagnosis d = new Diagnosis();
            d.Show();
            this.Hide();
        }


        private void label3_Click(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

    }
}
