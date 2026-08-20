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
    public partial class AdminDashboard : Form
    {
        SqlConnection con = new SqlConnection(
          @"Data Source=DESKTOP-DKEC0GM\SQLEXPRESS;Initial Catalog=HotelDB2;Integrated Security=True");

        public AdminDashboard()
        {
            InitializeComponent();
            HideTabHeader();
            cmbUserRole.Items.Add("Admin");
            cmbUserRole.Items.Add("Staff");
            cmbUserRole.Items.Add("Guest");

            cmbUserStatus.Items.Add("Active");
            cmbUserStatus.Items.Add("Inactive");

            cmbRoomType.Items.Add("Single");
            cmbRoomType.Items.Add("Double");
            cmbRoomType.Items.Add("Deluxe");
            cmbRoomType.Items.Add("Suite");

            cmbRoomStatus.Items.Add("Available");
            cmbRoomStatus.Items.Add("Booked");
            cmbRoomStatus.Items.Add("Maintenance");

            LoadDashboard();
            LoadUsers();
            LoadRooms();
            LoadBookings();
            LoadPayments();
            LoadServices();
        }
        void OpenConnection()
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
        }

        void CloseConnection()
        {
            if (con.State == ConnectionState.Open)
                con.Close();
        }
        int CountData(string query)
        {
            try
            {
                CloseConnection();
                SqlCommand cmd = new SqlCommand(query, con);
                OpenConnection();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            finally
            {
                CloseConnection();
            }
        }

        void LoadDashboard()
        {
            lblTotalRooms.Text = CountData("SELECT COUNT(*) FROM Rooms").ToString();
            lblAvailableRooms.Text = CountData("SELECT COUNT(*) FROM Rooms WHERE status='Available'").ToString();
            lblTotalBookings.Text = CountData("SELECT COUNT(*) FROM Bookings").ToString();
            lblTotalRevenue.Text = CountData("SELECT ISNULL(SUM(totalAmount),0) FROM Invoices WHERE paymentStatus='Paid'").ToString();
            lblUser.Text = CountData("SELECT COUNT(*) FROM Users").ToString();
        }
        void HideTabHeader()
        {
            tabAdmin.Appearance = TabAppearance.FlatButtons;
            tabAdmin.ItemSize = new Size(0, 1);
            tabAdmin.SizeMode = TabSizeMode.Fixed;
        }

        void LoadUsers()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Users", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvUsers.DataSource = dt;
        }

        void LoadRooms()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Rooms", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvRooms.DataSource = dt;
        }

        void LoadBookings()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Bookings", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvBookings.DataSource = dt;
        }

        void LoadPayments()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Invoices", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvPayments.DataSource = dt;
        }

        void LoadServices()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Services", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvServices.DataSource = dt;
        }
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabDashboard_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lblMyBills_Click(object sender, EventArgs e)
        {

        }

        private void lblMyBookings_Click(object sender, EventArgs e)
        {

        }

        private void lblAvailableRooms_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

        }

        private void tabPayments_Click(object sender, EventArgs e)
        {

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabDashboard;
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabUsers;
        }

        private void btnRooms_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabRooms;
        }

        private void btnBookings_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabBookings;

        }

        private void btnServices_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabServices;
        }

        private void btnPayments_Click(object sender, EventArgs e)
        {
         tabAdmin.SelectedTab = tabPayments;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Form1 login = new Form1();
            login.Show();
            this.Hide();
        }
        void ClearUser()
        {
            txtUserId.Clear();
            txtUserName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            cmbUserRole.Text = "";
            cmbUserStatus.Text = "";
        }
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            

  
            if (txtUserName.Text == "" || txtEmail.Text == "" || txtPhone.Text == "" ||
                cmbUserRole.Text == "" || cmbUserStatus.Text == "")
            {
                MessageBox.Show("Please fill all fields!");
                return;
            }

            string userQuery = @"INSERT INTO Users(userName,email,phoneNumber,role,status)
                         VALUES(@name,@email,@phone,@role,@status);
                         SELECT SCOPE_IDENTITY();";

            try
            {
                CloseConnection();
                OpenConnection();

                SqlCommand cmdUser = new SqlCommand(userQuery, con);
                cmdUser.Parameters.AddWithValue("@name", txtUserName.Text);
                cmdUser.Parameters.AddWithValue("@email", txtEmail.Text);
                cmdUser.Parameters.AddWithValue("@phone", txtPhone.Text);
                cmdUser.Parameters.AddWithValue("@role", cmbUserRole.Text);
                cmdUser.Parameters.AddWithValue("@status", cmbUserStatus.Text);

                int newUserId = Convert.ToInt32(cmdUser.ExecuteScalar());

                string loginQuery = @"INSERT INTO Login(userId,password,role,status)
                              VALUES(@id,@pass,@role,@status)";

                SqlCommand cmdLogin = new SqlCommand(loginQuery, con);
                cmdLogin.Parameters.AddWithValue("@id", newUserId);
                cmdLogin.Parameters.AddWithValue("@pass", "Password123@");
                cmdLogin.Parameters.AddWithValue("@role", cmbUserRole.Text);
                cmdLogin.Parameters.AddWithValue("@status", cmbUserStatus.Text == "Active" ? 1 : 0);

                cmdLogin.ExecuteNonQuery();

                MessageBox.Show("User Added Successfully!\nUser Name: " + txtUserName.Text + "\nPassword: Password123@");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add User Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            LoadUsers();
            ClearUser();
        }
        private void btnUpdateUser_Click(object sender, EventArgs e)
        {
            if (txtUserId.Text == "")
            {
                MessageBox.Show("Select a user first!");
                return;
            }

            string userQuery = @"UPDATE Users SET userName=@name, email=@email, phoneNumber=@phone,
                             role=@role,
                             status=@status
                         WHERE userId=@id";

            string loginQuery = @"UPDATE Login
                          SET role=@role,
                              status=@loginStatus
                          WHERE userId=@id";

            try
            {
                CloseConnection();
                OpenConnection();


                SqlCommand cmdUser = new SqlCommand(userQuery, con);
                cmdUser.Parameters.AddWithValue("@id", Convert.ToInt32(txtUserId.Text));
                cmdUser.Parameters.AddWithValue("@name", txtUserName.Text);
                cmdUser.Parameters.AddWithValue("@email", txtEmail.Text);
                cmdUser.Parameters.AddWithValue("@phone", txtPhone.Text);
                cmdUser.Parameters.AddWithValue("@role", cmbUserRole.Text);
                cmdUser.Parameters.AddWithValue("@status", cmbUserStatus.Text);
                cmdUser.ExecuteNonQuery();

                SqlCommand cmdLogin = new SqlCommand(loginQuery, con);
                cmdLogin.Parameters.AddWithValue("@id", Convert.ToInt32(txtUserId.Text));
                cmdLogin.Parameters.AddWithValue("@role", cmbUserRole.Text);
                cmdLogin.Parameters.AddWithValue("@loginStatus", cmbUserStatus.Text == "Active" ? 1 : 0);
                cmdLogin.ExecuteNonQuery();

                MessageBox.Show("User Updated Successfully!");
            }

            catch (Exception ex)

            {
                MessageBox.Show("Update User Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            LoadUsers();
            ClearUser();
        }


        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (txtUserId.Text == "")
            {
                MessageBox.Show("Select a user first!");
                return;
            }

            int userId = Convert.ToInt32(txtUserId.Text);

            try
            {
                CloseConnection();
                OpenConnection();

                SqlCommand cmdLogin = new SqlCommand("DELETE FROM Login WHERE userId=@id", con);
                cmdLogin.Parameters.AddWithValue("@id", userId);
                cmdLogin.ExecuteNonQuery();

                SqlCommand cmdUser = new SqlCommand("DELETE FROM Users WHERE userId=@id", con);
                cmdUser.Parameters.AddWithValue("@id", userId);
                cmdUser.ExecuteNonQuery();

                MessageBox.Show("User Deleted Successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete User Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            LoadUsers();
            ClearUser();
        }

        

        private void btnClearUser_Click(object sender, EventArgs e)
        {
            ClearUser();
        }

        private void btnLoadUsers_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtUserId.Text = dgvUsers.Rows[e.RowIndex].Cells["userId"].Value.ToString();
                txtUserName.Text = dgvUsers.Rows[e.RowIndex].Cells["userName"].Value.ToString();
                txtEmail.Text = dgvUsers.Rows[e.RowIndex].Cells["email"].Value.ToString();
                txtPhone.Text = dgvUsers.Rows[e.RowIndex].Cells["phoneNumber"].Value.ToString();
                cmbUserRole.Text = dgvUsers.Rows[e.RowIndex].Cells["role"].Value.ToString();
                cmbUserStatus.Text = dgvUsers.Rows[e.RowIndex].Cells["status"].Value.ToString();
            }
        }

        private void dgvRooms_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtRoomId.Text =
                dgvRooms.Rows[e.RowIndex].Cells["roomId"].Value.ToString();

                cmbRoomType.Text =
                dgvRooms.Rows[e.RowIndex].Cells["roomType"].Value.ToString();

                txtPrice.Text =
                dgvRooms.Rows[e.RowIndex].Cells["pricePerNight"].Value.ToString();

                cmbRoomStatus.Text =
                dgvRooms.Rows[e.RowIndex].Cells["status"].Value.ToString();
            }
        }
        private void dgvRooms_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtRoomId.Text =
                dgvRooms.Rows[e.RowIndex].Cells["roomId"].Value.ToString();

                cmbRoomType.Text =
                dgvRooms.Rows[e.RowIndex].Cells["roomType"].Value.ToString();

                txtPrice.Text =
                dgvRooms.Rows[e.RowIndex].Cells["pricePerNight"].Value.ToString();

                cmbRoomStatus.Text =
                dgvRooms.Rows[e.RowIndex].Cells["status"].Value.ToString();
            }
        }

        private void btnAddRoom_Click(object sender, EventArgs e)
        {
            if (cmbRoomType.Text == "" ||
       txtPrice.Text == "" ||
       cmbRoomStatus.Text == "")
            {
                MessageBox.Show("Please fill all fields!");
                return;
            }

            string query = @"INSERT INTO Rooms
                    (roomType,pricePerNight,status)
                    VALUES(@type,@price,@status)";

            try
            {
                CloseConnection();
                OpenConnection();

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@type", cmbRoomType.Text);

                cmd.Parameters.AddWithValue("@price",
                Convert.ToDouble(txtPrice.Text));

                cmd.Parameters.AddWithValue("@status",
                cmbRoomStatus.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Room Added Successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add Room Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            LoadRooms();
            ClearRoom();
        }

        private void btnUpdateRoom_Click(object sender, EventArgs e)
        {
            if (txtRoomId.Text == "")
            {
                MessageBox.Show("Select room first!");
                return;
            }

            string query = @"UPDATE Rooms
                     SET roomType=@type,
                         pricePerNight=@price,
                         status=@status
                     WHERE roomId=@id";

            try
            {
                CloseConnection();
                OpenConnection();

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@id",
                Convert.ToInt32(txtRoomId.Text));

                cmd.Parameters.AddWithValue("@type",
                cmbRoomType.Text);

                cmd.Parameters.AddWithValue("@price",
                Convert.ToDouble(txtPrice.Text));

                cmd.Parameters.AddWithValue("@status",
                cmbRoomStatus.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Room Updated Successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update Room Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            LoadRooms();
            ClearRoom();
        }

        private void btnDeleteRoom_Click(object sender, EventArgs e)
        {
            if (txtRoomId.Text == "")
            {
                MessageBox.Show("Select room first!");
                return;
            }

            try
            {
                CloseConnection();
                OpenConnection();

                SqlCommand cmd = new SqlCommand(
                "DELETE FROM Rooms WHERE roomId=@id", con);

                cmd.Parameters.AddWithValue("@id",
                Convert.ToInt32(txtRoomId.Text));

                cmd.ExecuteNonQuery();

                MessageBox.Show("Room Deleted Successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete Room Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            LoadRooms();
            ClearRoom();
        }

        private void btnClearRoom_Click(object sender, EventArgs e)
        {
            ClearRoom();
        }

        private void btnLoadRooms_Click(object sender, EventArgs e)
        {
            LoadRooms();
        }
        void ClearRoom()
        {
            txtRoomId.Clear();
            cmbRoomType.Text = "";
            txtPrice.Clear();
            cmbRoomStatus.Text = "";
        }

        private void dgvBookings_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnLoadBookings_Click(object sender, EventArgs e)
        {

            LoadBookings();
        }

        private void btnConfirmBooking_Click(object sender, EventArgs e)
        {
            if (dgvBookings.CurrentRow == null)
            {
                MessageBox.Show("Select booking first!");
                return;
            }

            int bookingId = Convert.ToInt32(dgvBookings.CurrentRow.Cells["bookingId"].Value);
            int roomId = Convert.ToInt32(dgvBookings.CurrentRow.Cells["roomId"].Value);

            try
            {
                CloseConnection();
                OpenConnection();

                SqlCommand cmdBooking = new SqlCommand(
                    "UPDATE Bookings SET status='Confirmed' WHERE bookingId=@bid", con);
                cmdBooking.Parameters.AddWithValue("@bid", bookingId);
                cmdBooking.ExecuteNonQuery();

                SqlCommand cmdRoom = new SqlCommand(
                    "UPDATE Rooms SET status='Booked' WHERE roomId=@rid", con);
                cmdRoom.Parameters.AddWithValue("@rid", roomId);
                cmdRoom.ExecuteNonQuery();

                MessageBox.Show("Booking Confirmed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Confirm Booking Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            LoadBookings();
            LoadRooms();
            LoadDashboard();

        }

        private void btnCancelBooking_Click(object sender, EventArgs e)
        {
            if (dgvBookings.CurrentRow == null)
            {
                MessageBox.Show("Select booking first!");
                return;
            }

            int bookingId = Convert.ToInt32(dgvBookings.CurrentRow.Cells["bookingId"].Value);
            int roomId = Convert.ToInt32(dgvBookings.CurrentRow.Cells["roomId"].Value);

            try
            {
                CloseConnection();
                OpenConnection();

                SqlCommand cmdBooking = new SqlCommand(
                    "UPDATE Bookings SET status='Cancelled' WHERE bookingId=@bid", con);
                cmdBooking.Parameters.AddWithValue("@bid", bookingId);
                cmdBooking.ExecuteNonQuery();

                SqlCommand cmdRoom = new SqlCommand(
                    "UPDATE Rooms SET status='Available' WHERE roomId=@rid", con);
                cmdRoom.Parameters.AddWithValue("@rid", roomId);
                cmdRoom.ExecuteNonQuery();

                MessageBox.Show("Booking Cancelled!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cancel Booking Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            LoadBookings();
            LoadRooms();
            LoadDashboard();

        }

        private void btnLoadPayments_Click(object sender, EventArgs e)
        {
            LoadPayments();
        }

        private void btnLoadServices_Click(object sender, EventArgs e)
        {
            LoadServices();
        }

        private void btnRefreshDashboard_Click(object sender, EventArgs e)
        {
            LoadDashboard();

            LoadUsers();
            LoadRooms();
            LoadBookings();
            LoadPayments();
            LoadServices();

            MessageBox.Show("Dashboard Refreshed Successfully!");
        }

        private void paneltotalBookings_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void tabUsers_Click(object sender, EventArgs e)
        {

        }

        private void panalRooms_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvUsers_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtUserId.Text = dgvUsers.Rows[e.RowIndex].Cells["userId"].Value.ToString();
                txtUserName.Text = dgvUsers.Rows[e.RowIndex].Cells["userName"].Value.ToString();
                txtEmail.Text = dgvUsers.Rows[e.RowIndex].Cells["email"].Value.ToString();
                txtPhone.Text = dgvUsers.Rows[e.RowIndex].Cells["phoneNumber"].Value.ToString();
                cmbUserRole.Text = dgvUsers.Rows[e.RowIndex].Cells["role"].Value.ToString();
                cmbUserStatus.Text = dgvUsers.Rows[e.RowIndex].Cells["status"].Value.ToString();
            }
        }
    }
}
