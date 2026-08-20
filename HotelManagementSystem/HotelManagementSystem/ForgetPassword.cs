using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManagementSystem
{
    public partial class ForgetPassword : Form
    {
        SqlConnection con = new SqlConnection(
         @"Data Source=DESKTOP-DKEC0GM\SQLEXPRESS;Initial Catalog=HotelDB2;Integrated Security=True");
        private string generatedOTP = "";
        private int foundUserId = -1;
        public ForgetPassword()
        {
            InitializeComponent();
            HideOTPSection();
            HidePasswordSection();
        }

        private void HideOTPSection()
        {
            txtOTP.Visible = false;
            label1.Visible = false;
            btnVerify.Visible = false;
        }

        private void ShowOTPSection()
        {
            txtOTP.Visible = true;
            label1.Visible = true;
            btnVerify.Visible = true;
        }

        private void HidePasswordSection()
        {
            txtNewPass.Visible = false;
            label3.Visible = false;
            btnSetPass.Visible = false;
        }

        private void ShowPasswordSection()
        {
            txtNewPass.Visible = true;
            label3.Visible = true;
            btnSetPass.Visible = true;
        }


        private void label2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void SendOTPEmail(string toEmail, string otp)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("your_email@gmail.com");
                mail.To.Add(toEmail);
                mail.Subject = "Hotel Management - Password Reset OTP";
                mail.Body = $"Hello {txtName.Text},\n\nYour OTP for password reset is: {otp}\n\nThis OTP is valid for one-time use only.\n\nRegards,\nHotel Management System";

                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential("nesafojilatun@gmail.com", "ptrswjwetxhhnilu");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Email sending failed: " + ex.Message);
            }
        }
        private void btnOTP_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "" || txtEmail.Text == "")
            {
                MessageBox.Show("Please enter Username and Email!");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Please enter a valid email address!");
                return;
            }

            try
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
                con.Open();

                // Check if username and email match in Users table
                string query = "SELECT userId FROM Users WHERE userName = @name AND email = @email AND status = 'Active'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);

                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    MessageBox.Show("Username or Email not found!");
                    return;
                }

                foundUserId = Convert.ToInt32(result);

                // 6-digit OTP Generate
                Random rnd = new Random();
                generatedOTP = rnd.Next(100000, 999999).ToString();

                // Send OTP via Email
                SendOTPEmail(txtEmail.Text, generatedOTP);

                MessageBox.Show("OTP sent to your email!");

                ShowOTPSection();
                HidePasswordSection();
                txtOTP.Clear();
                txtOTP.Focus();

                // Lock username and email fields
                txtName.Enabled = false;
                txtEmail.Enabled = false;
                btnOTP.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            if (txtOTP.Text == "")
            {
                MessageBox.Show("Please enter the OTP!");
                return;
            }

            if (txtOTP.Text == generatedOTP)
            {
                MessageBox.Show("OTP Verified Successfully!");

                
                ShowPasswordSection();
                HideOTPSection();
                txtNewPass.Clear();
                txtNewPass.Focus();
            }
            else
            {
                MessageBox.Show("Invalid OTP! Please try again.");
                txtOTP.Clear();
            }
        }

        private void btnSetPass_Click(object sender, EventArgs e)
        {
            if (txtNewPass.Text == "")
            {
                MessageBox.Show("Please enter a new password!");
                return;
            }

            
            if (txtNewPass.Text.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters!");
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtNewPass.Text, @"[0-9]"))
            {
                MessageBox.Show("Password must contain at least 1 number!");
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtNewPass.Text, @"[A-Z]"))
            {
                MessageBox.Show("Password must contain at least 1 uppercase letter!");
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtNewPass.Text, @"[a-z]"))
            {
                MessageBox.Show("Password must contain at least 1 lowercase letter!");
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtNewPass.Text, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
            {
                MessageBox.Show("Password must contain at least 1 special character!");
                return;
            }

            try
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
                con.Open();

                // Update password
                string query = "UPDATE Login SET password = @pass WHERE userId = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pass", txtNewPass.Text);
                cmd.Parameters.AddWithValue("@id", foundUserId);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    MessageBox.Show("Password changed successfully! Please login with your new password.");
                    new Form1().Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Failed to update password. Try again!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            this.Hide();
        }
    }
}
