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
using HotelManagementSystem.Models;
using HotelManagementSystem.Interface;

namespace HotelManagementSystem
{
    public partial class GuestDashboard : Form
    {
        SqlConnection con = new SqlConnection(
            @"Data Source=DESKTOP-DKEC0GM\SQLEXPRESS;Initial Catalog=HotelDB2;Integrated Security=True");

        int currentGuestId;
        public GuestDashboard(int guestId)
        {
            InitializeComponent();
            currentGuestId = guestId;
            cmbRoomType.Items.Add("Single");
            cmbRoomType.Items.Add("Double");
            cmbRoomType.Items.Add("Deluxe");
            cmbRoomType.Items.Add("Suite");

            cmbServiceName.Items.Add("Breakfast");
            cmbServiceName.Items.Add("Laundry");
            cmbServiceName.Items.Add("Spa");
            cmbServiceName.Items.Add("Room Service");
            cmbServiceName.Items.Add("Transport");
            HideTabHeader();

            LoadDashboard();
            LoadMyBookings();
            LoadMyBill();
            LoadMyService();
            LoadBookingIds();
            cmbPaymentMethod.Items.Add("Cash");
            cmbPaymentMethod.Items.Add("bKash");
            cmbPaymentMethod.Items.Add("Nagad");
            cmbPaymentMethod.Items.Add("Bank Card");

            cmbPaymentStatus.Items.Add("Unpaid");
            cmbPaymentStatus.Items.Add("Paid");
        }
        void HideTabHeader()
        {
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
        }



        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lblAvailableRooms_Click(object sender, EventArgs e)
        {

        }

        private void lblMyServices_Click(object sender, EventArgs e)
        {

        }

        private void tabSearchRoom_Click(object sender, EventArgs e)
        {

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabDashboard;
            LoadDashboard();
        }

        private void btnSearchRoom_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabSearchRoom;

        }

        private void btnMyBookings_Click(object sender, EventArgs e)
        {

            tabControl1.SelectedTab = tabMyBooking;
            LoadMyBookings();
        }

        private void btnMyBills_Click(object sender, EventArgs e)
        {

            tabControl1.SelectedTab = tabMyBill;
            LoadMyBill();
            LoadBillBookingIds();
        }


        private void btnService_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabService;
            LoadBookingIds();
            LoadMyService();
        }


        private void btnLogout_Click(object sender, EventArgs e)
        {
            Form1 login = new Form1();
            login.Show();
            this.Hide();
        }
        void LoadDashboard()
        {
            lblAvailableRooms.Text = CountData("SELECT COUNT(*) FROM Rooms WHERE status='Available'").ToString();
            lblMyBookings.Text = CountData("SELECT COUNT(*) FROM Bookings WHERE guestId='" + currentGuestId + "'").ToString();
            lblMyBills.Text = CountData("SELECT COUNT(*) FROM Invoices i INNER JOIN Bookings b ON i.bookingId=b.bookingId WHERE b.guestId='" + currentGuestId + "'").ToString();
            lblMyServices.Text = CountData("SELECT COUNT(*) FROM Services s INNER JOIN Bookings b ON s.bookingId=b.bookingId WHERE b.guestId='" + currentGuestId + "'").ToString();
        }
        int CountData(string query)
        {
            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();

            return count;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM Rooms WHERE status='Available'";

            if (cmbRoomType.Text != "")
            {
                query += " AND roomType=@type";
            }

            if (txtMaxPrice.Text != "")
            {
                query += " AND pricePerNight<=@price";
            }

            SqlCommand cmd = new SqlCommand(query, con);

            if (cmbRoomType.Text != "")
            {
                cmd.Parameters.AddWithValue("@type", cmbRoomType.Text);
            }

            if (txtMaxPrice.Text != "")
            {
                cmd.Parameters.AddWithValue("@price", txtMaxPrice.Text);
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvAvailableRooms.DataSource = dt;
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
        private void btnBookRoom_Click(object sender, EventArgs e)
        {
            if (dgvAvailableRooms.CurrentRow == null)
            {
                MessageBox.Show("First search room and select a room from the table!");
                return;
            }

            if (dgvAvailableRooms.CurrentRow.Cells["roomId"].Value == null)
            {
                MessageBox.Show("Invalid room selected!");
                return;
            }

            int roomId = Convert.ToInt32(
                dgvAvailableRooms.CurrentRow.Cells["roomId"].Value
            );
           try
            {
              

                string query = @"INSERT INTO Bookings
        (guestId,roomId,checkIn,checkOut,status)
        VALUES(@guestId,@roomId,@checkIn,@checkOut,@status)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@guestId", currentGuestId);
                cmd.Parameters.AddWithValue("@roomId", roomId);
                cmd.Parameters.AddWithValue("@checkIn", dptCheckIn.Value);
                cmd.Parameters.AddWithValue("@checkOut", dptCheckOut.Value);
                cmd.Parameters.AddWithValue("@status", "Pending");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Booking Successful!");

                LoadMyBookings();
                LoadDashboard();
                LoadBillBookingIds();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        
        LoadMyBookings();
            LoadDashboard();

        }

        void LoadMyBookings()
        {
            string query = "SELECT * FROM Bookings WHERE guestId=@guestId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@guestId", currentGuestId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvMyBookings.DataSource = dt;
        }

        private void btnRefreshBooking_Click(object sender, EventArgs e)
        {
            LoadMyBookings();
            LoadDashboard();
        }

        private void btnCancelBooking_Click(object sender, EventArgs e)
        {
            if (dgvMyBookings.CurrentRow == null)
            {
                MessageBox.Show("Please select a booking!");
                return;
            }

            string bookingId = dgvMyBookings.CurrentRow.Cells["bookingId"].Value.ToString();
            string status = dgvMyBookings.CurrentRow.Cells["status"].Value.ToString();

            if (status != "Pending")
            {
                MessageBox.Show("Only pending booking can be cancelled!");
                return;
            }

            string query = "UPDATE Bookings SET status='Cancelled' WHERE bookingId=@bookingId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@bookingId", bookingId);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Booking Cancelled");
            LoadMyBookings();
            LoadDashboard();

        }


        void LoadMyBill()
        {
            string query = @"SELECT i.invoiceId, i.bookingId, i.totalAmount,
                            i.paymentDate, i.paymentMethod, i.paymentStatus
                     FROM Invoices i
                     INNER JOIN Bookings b ON i.bookingId = b.bookingId
                     WHERE b.guestId=@guestId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@guestId", currentGuestId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dgvMyBills.DataSource = dt;
        }

        void LoadBookingIds()
        {
            cmbBookingId.Items.Clear();

            string query = @"SELECT bookingId
                     FROM Bookings
                     WHERE guestId=@guestId";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@guestId", currentGuestId);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                cmbBookingId.Items.Add(dr["bookingId"].ToString());
            }

            dr.Close();
            con.Close();
        }
        void LoadMyService()
        {
            string query = "SELECT s.* FROM Services s INNER JOIN Bookings b ON s.bookingId=b.bookingId WHERE b.guestId=@guestId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@guestId", currentGuestId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvMyServices.DataSource = dt;
        }

        private void btnRequestService_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"INSERT INTO Services
        (serviceName,cost,bookingId)
        VALUES(@name,@cost,@bookingId)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@name", cmbServiceName.Text);

                double cost = 0;

                if (cmbServiceName.Text == "Laundry")
                    cost = 200;

                else if (cmbServiceName.Text == "Cleaning")
                    cost = 500;

                else if (cmbServiceName.Text == "Breakfast")
                    cost = 700;

                else if (cmbServiceName.Text == "Spa")
                    cost = 2000;

                cmd.Parameters.AddWithValue("@cost", cost);

                cmd.Parameters.AddWithValue("@bookingId",
                Convert.ToInt32(cmbBookingId.Text));

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Service Requested Successfully!");

                LoadMyService();
                LoadDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        
        private void dgvMyBills_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtInvoiceId.Text = dgvMyBills.Rows[e.RowIndex].Cells["invoiceId"].Value.ToString();
                cmbBooking.Text = dgvMyBills.Rows[e.RowIndex].Cells["bookingId"].Value.ToString();
                txtPayAmount.Text = dgvMyBills.Rows[e.RowIndex].Cells["totalAmount"].Value.ToString();
                cmbPaymentMethod.Text = dgvMyBills.Rows[e.RowIndex].Cells["paymentMethod"].Value.ToString();
                cmbPaymentStatus.Text = dgvMyBills.Rows[e.RowIndex].Cells["paymentStatus"].Value.ToString();
            }
        }

        private void btnPayNow_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (cmbBooking.Text == "")
                {
                    MessageBox.Show("Select Booking ID!");
                    return;
                }

                if (cmbPaymentMethod.Text == "")
                {
                    MessageBox.Show("Select Payment Method!");
                    return;
                }

                string query = @"UPDATE Invoices
                         SET paymentDate=@date,
                             paymentMethod=@method,
                             paymentStatus=@status
                         WHERE bookingId=@bookingId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@date", DateTime.Now.Date);

                cmd.Parameters.AddWithValue("@method",
                cmbPaymentMethod.Text);

                cmd.Parameters.AddWithValue("@status", "Paid");

                cmd.Parameters.AddWithValue("@bookingId",
                Convert.ToInt32(cmbBooking.Text));

                con.Open();

                int row = cmd.ExecuteNonQuery();

                con.Close();

                if (row > 0)
                {
                    MessageBox.Show("Payment Successful!");

                    cmbPaymentStatus.Text = "Paid";

                    LoadMyBill();
                }
                else
                {
                    MessageBox.Show("Bill not found!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        

        

        private void tabDashboard_Click(object sender, EventArgs e)
        {

        }

        private void tabMyBill_Click(object sender, EventArgs e)
        {

        }

        private void btnGenerateBill_Click(object sender, EventArgs e)
        {

      
            try
            {
                if (cmbBooking.Text == "")
                {
                    MessageBox.Show("Please select Booking ID!");
                    return;
                }

                int bookingId = Convert.ToInt32(cmbBooking.Text);

                double roomCost = 0;
                double serviceCost = 0;
                double totalAmount = 0;

                con.Open();

                string roomQuery = @"SELECT r.pricePerNight
                             FROM Bookings b
                             INNER JOIN Rooms r ON b.roomId = r.roomId
                             WHERE b.bookingId=@bookingId";

                SqlCommand roomCmd = new SqlCommand(roomQuery, con);
                roomCmd.Parameters.AddWithValue("@bookingId", bookingId);

                roomCost = Convert.ToDouble(roomCmd.ExecuteScalar());

                string serviceQuery = @"SELECT ISNULL(SUM(cost),0)
                                FROM Services
                                WHERE bookingId=@bookingId";

                SqlCommand serviceCmd = new SqlCommand(serviceQuery, con);
                serviceCmd.Parameters.AddWithValue("@bookingId", bookingId);

                serviceCost = Convert.ToDouble(serviceCmd.ExecuteScalar());

                totalAmount = roomCost + serviceCost;

                string insertQuery = @"INSERT INTO Invoices
        (bookingId,totalAmount,paymentDate,paymentMethod,paymentStatus)
        VALUES(@bookingId,@totalAmount,NULL,NULL,'Unpaid')";

                SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                insertCmd.Parameters.AddWithValue("@bookingId", bookingId);
                insertCmd.Parameters.AddWithValue("@totalAmount", totalAmount);

                insertCmd.ExecuteNonQuery();
                con.Close();

                txtRoomCost.Text = roomCost.ToString();
                txtServiceCost.Text = serviceCost.ToString();
                txtPayAmount.Text = totalAmount.ToString();
                cmbPaymentStatus.Text = "Unpaid";

                MessageBox.Show("Bill Generated Successfully!");

                LoadMyBill();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        void LoadBillBookingIds()
        {
            cmbBooking.Items.Clear();

            string query = @"SELECT bookingId
                     FROM Bookings
                     WHERE guestId=@guestId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@guestId", currentGuestId);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                cmbBooking.Items.Add(dr["bookingId"].ToString());
            }

            dr.Close();
            con.Close();
        }
        private void btnLoadBills_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void tabService_Click(object sender, EventArgs e)
        {

        }

        private void dgvMyServices_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cmbServiceName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbBookingId_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void lblBookingId_Click(object sender, EventArgs e)
        {

        }

        private void txtAccountNumber_TextChanged(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void cmbBooking_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbPaymentStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtPayAmount_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtServiceCost_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtRoomCost_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtInvoiceId_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void tabMyBooking_Click(object sender, EventArgs e)
        {

        }

        private void dgvMyBookings_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvAvailableRooms_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dptCheckOut_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dptCheckIn_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtMaxPrice_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblCheckOut_Click(object sender, EventArgs e)
        {

        }

        private void lblCheckIn_Click(object sender, EventArgs e)
        {

        }

        private void lblMaxPrice_Click(object sender, EventArgs e)
        {

        }

        private void lblRoomType_Click(object sender, EventArgs e)
        {

        }

        private void cmbRoomType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblMyBills_Click(object sender, EventArgs e)
        {

        }

        private void lblMyBookings_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
    


