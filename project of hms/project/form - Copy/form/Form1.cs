using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace form
{
    public partial class Form1 : Form
    {
        private readonly string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=olx;Integrated Security=True;";
        private Timer animationTimer = new Timer();
        private float scale = 1.0f;
        private bool scalingUp = true;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';

            comboBox1.Items.Clear();
            comboBox1.Items.Add("Admin");
            comboBox1.Items.Add("Doctor"); // Added Doctor option
            comboBox1.SelectedIndex = -1;

            MakePictureBoxCircular();
            StartPictureAnimation();
        }

        private void MakePictureBoxCircular()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(0, 0, pictureBox1.Width - 3, pictureBox1.Height - 3);
                pictureBox1.Region = new Region(gp);
            }
        }

        private void StartPictureAnimation()
        {
            animationTimer.Interval = 50;
            animationTimer.Tick += AnimatePicture;
            animationTimer.Start();
        }

        private void AnimatePicture(object sender, EventArgs e)
        {
            if (scalingUp)
            {
                scale += 0.02f;
                if (scale >= 1.2f)
                    scalingUp = false;
            }
            else
            {
                scale -= 0.02f;
                if (scale <= 1.0f)
                    scalingUp = true;
            }

            pictureBox1.Width = (int)(100 * scale);
            pictureBox1.Height = (int)(100 * scale);
            pictureBox1.Left = (this.ClientSize.Width - pictureBox1.Width) / 2;
        }

        private bool ValidateInputs(out string message)
        {
            message = "";

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                message = "Username is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                message = "Password is required.";
                return false;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                message = "Please select a role.";
                return false;
            }

            if (textBox2.Text.Length != 6)
            {
                message = "Password must be exactly 6 characters long.";
                return false;
            }

            return true;
        }

        private void LOGIN_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out string validationMessage))
            {
                ShowAnimatedMessage(validationMessage);
                return;
            }

            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string role = comboBox1.SelectedItem?.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE Username = @username AND Password = @password AND Role = @role";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@role", role);

                    int count = (int)cmd.ExecuteScalar();

                    if (count == 1)
                    {
                        ShowAnimatedMessage("Login Successful!");
                        Form2 dashboard = new Form2();
                        dashboard.Show();
                        this.Hide();
                    }
                    else
                    {
                        ShowAnimatedMessage("Invalid credentials. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    ShowAnimatedMessage("Error: " + ex.Message);
                }
            }
        }

        private void CreateNewUser_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out string validationMessage))
            {
                ShowAnimatedMessage(validationMessage);
                return;
            }

            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string role = comboBox1.SelectedItem?.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string insertQuery = "INSERT INTO users (Username, Password, Role) VALUES (@username, @password, @role)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@username", username);
                    insertCmd.Parameters.AddWithValue("@password", password);
                    insertCmd.Parameters.AddWithValue("@role", role);

                    int rowsAffected = insertCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowAnimatedMessage("New user created successfully! You can now login.");
                    }
                    else
                    {
                        ShowAnimatedMessage("Failed to create user.");
                    }
                }
                catch (Exception ex)
                {
                    ShowAnimatedMessage("Error: " + ex.Message);
                }
            }
        }

        private void ClearFields()
        {
            textBox1.Clear();
            textBox2.Clear();
            comboBox1.SelectedIndex = -1;
            checkBox1.Checked = false;
        }

        private void ResetPassword_Click(object sender, EventArgs e)
        {
            Form10 resetPasswordForm = new Form10();
            resetPasswordForm.Show();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        [DllImport("user32.dll")]
        private static extern bool AnimateWindow(IntPtr hwnd, int time, int flags);

        private const int AW_SLIDE = 0X40000;
        private const int AW_VER_POSITIVE = 0X00000004;
        private const int AW_BLEND = 0x00080000;

        private void ShowAnimatedMessage(string message)
        {
            Form messageForm = new Form
            {
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Size = new Size(400, 160), // Slightly smaller height
                BackColor = Color.WhiteSmoke,
                TopMost = true
            };

            PictureBox iconPictureBox = new PictureBox
            {
                Image = SystemIcons.Information.ToBitmap(),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(40, 40),
                Location = new Point(20, 20) // Normal top margin
            };

            Label lblMessage = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Size = new Size(320, 50),
                Location = new Point(70, 20) // Normal top margin
            };

            Button btnOK = new Button
            {
                Text = "OK",
                BackColor = Color.MediumBlue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(80, 35),
                Location = new Point((messageForm.Width - 80) / 2, 85) // Decreased vertical distance
            };
            btnOK.Click += (s, e) => { messageForm.Close(); };

            messageForm.Controls.Add(iconPictureBox);
            messageForm.Controls.Add(lblMessage);
            messageForm.Controls.Add(btnOK);

            messageForm.Load += (s, e) =>
            {
                AnimateWindow(messageForm.Handle, 400, AW_BLEND | AW_VER_POSITIVE | AW_SLIDE);
            };

            messageForm.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Optional click functionality
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
