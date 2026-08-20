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
    public partial class SignupForm : Form
    {
        SqlConnection con = new SqlConnection(
           @"Data Source=DESKTOP-DKEC0GM\SQLEXPRESS;Initial Catalog=HotelDB2;Integrated Security=True");

        public SignupForm()
        {
            InitializeComponent();
        }

        private void btnsingUp_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "" || txtPhone.Text == "" ||
                txtEmail.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("Please fill all fields!");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtPhone.Text, @"^\d{11}$"))
            {
                MessageBox.Show("Phone number must be exactly 11 digits!");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Please enter a valid email address!");
                return;
            }

            if (txtPassword.Text.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters!");
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtPassword.Text, @"[0-9]"))
            {
                MessageBox.Show("Password must contain at least 1 number!");
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtPassword.Text, @"[A-Z]"))
            {
                MessageBox.Show("Password must contain at least 1 uppercase letter!");
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtPassword.Text, @"[a-z]"))
            {
                MessageBox.Show("Password must contain at least 1 lowercase letter!");
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtPassword.Text, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
            {
                MessageBox.Show("Password must contain at least 1 special character!");
                return;
            }

            string userQuery = @"INSERT INTO Users(userName,email,phoneNumber,role,status)
                 VALUES(@name,@email,@phone,@role,@status);
                 SELECT SCOPE_IDENTITY();";
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                con.Open();

                SqlCommand cmdUser = new SqlCommand(userQuery, con);
                cmdUser.Parameters.AddWithValue("@name", txtName.Text);
                cmdUser.Parameters.AddWithValue("@email", txtEmail.Text);
                cmdUser.Parameters.AddWithValue("@phone", txtPhone.Text);
                cmdUser.Parameters.AddWithValue("@role", "Guest");
                cmdUser.Parameters.AddWithValue("@status", "Active");

                int newUserId = Convert.ToInt32(cmdUser.ExecuteScalar());

                string loginQuery = @"INSERT INTO Login(userId,password,role,status)
                      VALUES(@id,@pass,@role,@loginStatus)";

                SqlCommand cmdLogin = new SqlCommand(loginQuery, con);
                cmdLogin.Parameters.AddWithValue("@id", newUserId);
                cmdLogin.Parameters.AddWithValue("@pass", txtPassword.Text);
                cmdLogin.Parameters.AddWithValue("@role", "Guest");
                cmdLogin.Parameters.AddWithValue("@loginStatus", 1);

                cmdLogin.ExecuteNonQuery();

                MessageBox.Show("Account Created Successfully!\nYour User Name is: " + txtName.Text);
                ClearSignup();

                new Form1().Show();
                this.Hide();
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

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            Form1 login = new Form1();
            login.Show();
            this.Hide();
        }
        void ClearSignup()
        {
            txtName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            cbloginShowpassword.Checked = false;
        }

        private void cbloginShowpassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = cbloginShowpassword.Checked ? '\0' : '*';

        }

        private void btnsignIn_Click(object sender, EventArgs e)
        {

            Form1 login = new Form1();
            login.Show();
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cmbRole_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
