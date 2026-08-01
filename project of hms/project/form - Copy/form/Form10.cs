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
    public partial class Form10 : Form
    {
        SqlConnection conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=olx;Integrated Security=True");

        public Form10()
        {
            InitializeComponent();
        }

        private void Form10_Load(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = true;
            textBox3.UseSystemPasswordChar = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox3.UseSystemPasswordChar = !checkBox2.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string newPassword = textBox2.Text;
            string confirmPassword = textBox3.Text;

            if (username == "" || newPassword == "" || confirmPassword == "")
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            try
            {
                conn.Open();

                SqlCommand checkUser = new SqlCommand("SELECT COUNT(*) FROM users WHERE username=@username", conn);
                checkUser.Parameters.AddWithValue("@username", username);
                int userExists = (int)checkUser.ExecuteScalar();

                if (userExists == 1)
                {
                    SqlCommand updatePassword = new SqlCommand("UPDATE users SET password=@password WHERE username=@username", conn);
                    updatePassword.Parameters.AddWithValue("@password", newPassword);
                    updatePassword.Parameters.AddWithValue("@username", username);

                    int result = updatePassword.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Password changed successfully. Redirecting to Login...");
                        Form1 login = new Form1();
                        login.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Password change failed.");
                    }
                }
                else
                {
                    MessageBox.Show("Username does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}