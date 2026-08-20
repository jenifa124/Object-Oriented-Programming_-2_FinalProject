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

namespace HotelManagementSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection con = new SqlConnection(
                     @"Data Source=DESKTOP-DKEC0GM\SQLEXPRESS;Initial Catalog=HotelDB2;Integrated Security=True");
        private void cbloginShowpassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = cbloginShowpassword.Checked ? '\0' : '*';
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("Please fill all fields!");
                return;
            }

            string query = @"SELECT l.userId, l.role 
                     FROM Login l
                     INNER JOIN Users u ON l.userId = u.userId
                     WHERE u.userName = @username 
                     AND l.password = @pass 
                     AND l.status = 1";

            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                cmd.Parameters.AddWithValue("@pass", txtPassword.Text);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string role = reader["role"].ToString();
                    int userId = Convert.ToInt32(reader["userId"]);

                    MessageBox.Show("Login Successful");

                    if (role == "Admin")
                    {
                        new AdminDashboard().Show();
                        this.Hide();
                    }
                    else if (role == "Staff")
                    {
                        new StaffDashboard().Show();
                        this.Hide();
                    }
                    else if (role == "Guest")
                    {
                        GuestDashboard guest = new GuestDashboard(userId);
                        guest.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Role not recognized!");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Username or Password!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SignupForm signup = new SignupForm();
            signup.Show();
            this.Hide();
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnForgetPassword_Click(object sender, EventArgs e)
        {
            ForgetPassword f = new ForgetPassword();
            f.Show();
            this.Hide();
        }
    }
}
