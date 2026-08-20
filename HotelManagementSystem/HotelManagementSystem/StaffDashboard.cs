using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManagementSystem
{
    public partial class StaffDashboard : Form
    {
        SqlConnection con = new SqlConnection(
          @"Data Source=DESKTOP-DKEC0GM\SQLEXPRESS;Initial Catalog=HotelDB2;Integrated Security=True");
        public StaffDashboard()
        {
            InitializeComponent();
            tabStaff.Appearance = TabAppearance.FlatButtons;
            tabStaff.ItemSize = new Size(0, 1);
            tabStaff.SizeMode = TabSizeMode.Fixed;

            cmbRoomStatus.Items.Add("Available");
            cmbRoomStatus.Items.Add("Booked");
            cmbRoomStatus.Items.Add("Maintenance");

            LoadDashboard();
            LoadRooms();
            LoadBookings();
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
        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void lblCheckIn_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabCheckInOut_Click(object sender, EventArgs e)
        {

        }
        private void btnLoadBills_Click(object sender, EventArgs e)
        {
            if (txtBookingId.Text == "")
            {
                MessageBox.Show("Please select a booking from the table first!");
                return;
            }

            try
            {
                CloseConnection();
                OpenConnection();

                SqlCommand checkCmd = new SqlCommand(
                    "SELECT status FROM Bookings WHERE bookingId=@id", con);
                checkCmd.Parameters.AddWithValue("@id",
                    Convert.ToInt32(txtBookingId.Text));

                string status = checkCmd.ExecuteScalar()?.ToString();

                if (status != "Checked In")
                {
                    MessageBox.Show("Guest must be Checked In before Checking Out!");
                    return;
                }

                SqlCommand cmd = new SqlCommand(
                    "UPDATE Bookings SET status='Checked Out' WHERE bookingId=@id", con);
                cmd.Parameters.AddWithValue("@id",
                    Convert.ToInt32(txtBookingId.Text));
                cmd.ExecuteNonQuery();

                MessageBox.Show("Guest Checked Out Successfully!");

                btnCheckIn.Enabled = false;
                btnCheckOut.Enabled = false;
                txtBookingId.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            LoadBookings();
        }
        
        private void dgvCurrentBookings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtBookingId.Text =
                    dgvCurrentBookings.Rows[e.RowIndex]
                    .Cells["bookingId"].Value.ToString();

                string currentStatus =
                    dgvCurrentBookings.Rows[e.RowIndex]
                    .Cells["status"].Value.ToString();

                if (currentStatus == "Pending" || currentStatus == "Booked")
                {
                    btnCheckIn.Enabled = true;
                    btnCheckOut.Enabled = false;
                }
                else if (currentStatus == "Checked In")
                {
                    btnCheckIn.Enabled = false;
                    btnCheckOut.Enabled = true;
                }
                else
                {
                    btnCheckIn.Enabled = false;
                    btnCheckOut.Enabled = false;
                }
            }
        }

        private void dgvCurrentBookings_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnStaffDashboard_Click(object sender, EventArgs e)
        {
            tabStaff.SelectedTab = tabDashboard;
        }

        private void btnRoomStatus_Click(object sender, EventArgs e)
        {
            tabStaff.SelectedTab = tabRoomStatus;

        }

        private void btnStaffBookings_Click(object sender, EventArgs e)
        {
            tabStaff.SelectedTab = tabBookings;

        }

        private void btnCheckInOut_Click(object sender, EventArgs e)
        {
            tabStaff.SelectedTab = tabCheckInOut;
        }

        private void btnStaffServices_Click(object sender, EventArgs e)
        {
            tabStaff.SelectedTab = tabServices;

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
            lblTotalRooms.Text =
            CountData("SELECT COUNT(*) FROM Rooms").ToString();

            lblAvailableRooms.Text =
            CountData("SELECT COUNT(*) FROM Rooms WHERE status='Available'")
            .ToString();

            lblTodayBookings.Text =
             CountData("SELECT COUNT(*) FROM Bookings WHERE status='Pending'")
             .ToString();

            lblPendingServices.Text =
            CountData("SELECT COUNT(*) FROM Services").ToString();
            lblTotalRevenue.Text = CountData("SELECT ISNULL(SUM(totalAmount),0) FROM Invoices WHERE paymentStatus='Paid'").ToString();
            lblUser.Text = CountData("SELECT COUNT(*) FROM Users").ToString();
        }

        private void btnLoadStaffServices_Click(object sender, EventArgs e)
        {
            LoadServices();
        }

        private void btnMarkServiceDone_Click(object sender, EventArgs e)
        {
            if (selectedServiceId == 0)
            {
                MessageBox.Show("Please select a service from the table first!");
                return;
            }

            try
            {
                CloseConnection();
                OpenConnection();

                SqlCommand checkCmd = new SqlCommand(
                    "SELECT status FROM Services WHERE serviceId=@id", con);
                checkCmd.Parameters.AddWithValue("@id", selectedServiceId);

                string status = checkCmd.ExecuteScalar()?.ToString();

                if (status == "Done")
                {
                    MessageBox.Show("This service is already marked as Done!");
                    return;
                }

                SqlCommand cmd = new SqlCommand(
                    "UPDATE Services SET status='Done' WHERE serviceId=@id", con);
                cmd.Parameters.AddWithValue("@id", selectedServiceId);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Service Marked as Done!");

                selectedServiceId = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            LoadServices();
            LoadDashboard();
        }

        private void btnRefreshStaffDashboard_Click(object sender, EventArgs e)
        {
            LoadDashboard();
            LoadRooms();
            LoadBookings();
            LoadServices();
            LoadUsers();
        }
        void LoadRooms()
        {
            SqlDataAdapter da =
            new SqlDataAdapter("SELECT * FROM Rooms", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            dgvStaffRooms.DataSource = dt;
        }
        private void btnLoadRoomsStaff_Click(object sender, EventArgs e)
        {
            LoadRooms();
        }
        void LoadBookings()
        {
            SqlDataAdapter da =
            new SqlDataAdapter("SELECT * FROM Bookings", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            dgvStaffBookings.DataSource = dt;

            dgvCurrentBookings.DataSource = dt;
        }

        private void btnBookRoom_Click(object sender, EventArgs e)
        {

        }
        void LoadServices()
        {
            SqlDataAdapter da =
            new SqlDataAdapter("SELECT * FROM Services", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            dgvStaffServices.DataSource = dt;
        }

        private void btnLoadStaffBookings_Click(object sender, EventArgs e)
        {
            LoadBookings();
        }

        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            if (txtBookingId.Text == "")
            {
                MessageBox.Show("Please select a booking from the table first!");
                return;
            }

            try
            {
                CloseConnection();
                OpenConnection();

                SqlCommand checkCmd = new SqlCommand(
                    "SELECT status FROM Bookings WHERE bookingId=@id", con);
                checkCmd.Parameters.AddWithValue("@id",
                    Convert.ToInt32(txtBookingId.Text));

                string status = checkCmd.ExecuteScalar()?.ToString();

                if (status == "Checked In")
                {
                    MessageBox.Show("Guest is already Checked In!");
                    return;
                }
                if (status == "Checked Out")
                {
                    MessageBox.Show("This booking is already Checked Out!");
                    return;
                }

                SqlCommand cmd = new SqlCommand(
                    "UPDATE Bookings SET status='Checked In' WHERE bookingId=@id", con);
                cmd.Parameters.AddWithValue("@id",
                    Convert.ToInt32(txtBookingId.Text));
                cmd.ExecuteNonQuery();

                MessageBox.Show("Guest Checked In Successfully!");

                btnCheckIn.Enabled = false;
                btnCheckOut.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            LoadBookings();
        }

        private void dgvStaffRooms_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtRoomId.Text =
                dgvStaffRooms.Rows[e.RowIndex]
                .Cells["roomId"].Value.ToString();

                cmbRoomStatus.Text =
                dgvStaffRooms.Rows[e.RowIndex]
                .Cells["status"].Value.ToString();
            }
        }

        private void dgvStaffRooms_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtRoomId.Text =
                    dgvStaffRooms.Rows[e.RowIndex]
                    .Cells["roomId"].Value.ToString();

                cmbRoomStatus.Text =
                    dgvStaffRooms.Rows[e.RowIndex]
                    .Cells["status"].Value.ToString();
            }
        }

        private void btnUpdateRoomStatus_Click(object sender, EventArgs e)
        {
            if (txtRoomId.Text == "")
            {
                MessageBox.Show("Select Room First!");
                return;
            }

            string query =
            "UPDATE Rooms SET status=@status WHERE roomId=@id";

            try
            {
                CloseConnection();
                OpenConnection();

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@id",
                Convert.ToInt32(txtRoomId.Text));

                cmd.Parameters.AddWithValue("@status",
                cmbRoomStatus.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Room Status Updated!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            LoadRooms();
            LoadDashboard();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnStaffLogout_Click(object sender, EventArgs e)
        {
            Form1 login = new Form1();
            login.Show();
            this.Hide();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void txtRoomId_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvStaffServices_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        int selectedServiceId = 0;

        private void dgvStaffServices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedServiceId =
                    Convert.ToInt32(dgvStaffServices.Rows[e.RowIndex]
                    .Cells["serviceId"].Value.ToString());
            }
        }

        private void dgvPayments_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedInvoiceId = Convert.ToInt32(dgvPayments.Rows[e.RowIndex].Cells["Invoice ID"].Value);
                selectedStaffBookingId = Convert.ToInt32(dgvPayments.Rows[e.RowIndex].Cells["Booking ID"].Value);

                txtInvoiceId.Text = selectedInvoiceId.ToString();
                txtPayAmount.Text = dgvPayments.Rows[e.RowIndex].Cells["Total Amount"].Value.ToString();
                cmbPaymentMethod.Text = dgvPayments.Rows[e.RowIndex].Cells["Payment Method"].Value?.ToString() ?? "";
                cmbPaymentStatus.Text = dgvPayments.Rows[e.RowIndex].Cells["Payment Status"].Value.ToString();

                txtRoomCost.Text = "";
                txtServiceCost.Text = "";
                txtAccountNumber.Text = "";

                // Match combobox item
                foreach (var item in cmbBooking.Items)
                {
                    if (item.ToString().StartsWith(selectedStaffBookingId.ToString() + " -"))
                    {
                        cmbBooking.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabStaff.SelectedTab = tabPayment;
            LoadAllPayments();
            LoadAllBookingIds();

            if (cmbPaymentMethod.Items.Count == 0)
            {
                cmbPaymentMethod.Items.Add("Cash");
                cmbPaymentMethod.Items.Add("bKash");
                cmbPaymentMethod.Items.Add("Nagad");
                cmbPaymentMethod.Items.Add("Bank Card");
            }

            if (cmbPaymentStatus.Items.Count == 0)
            {
                cmbPaymentStatus.Items.Add("Unpaid");
                cmbPaymentStatus.Items.Add("Paid");
            }
        }

        

        private void cmbBooking_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        int selectedInvoiceId = 0;
        int selectedStaffBookingId = 0;

        void LoadAllPayments()
        {
            try
            {
                string query = @"SELECT 
                            i.invoiceId         AS [Invoice ID],
                            i.bookingId         AS [Booking ID],
                            u.userName          AS [Guest Name],
                            r.roomType          AS [Room Type],
                            i.totalAmount       AS [Total Amount],
                            i.paymentDate       AS [Payment Date],
                            i.paymentMethod     AS [Payment Method],
                            i.paymentStatus     AS [Payment Status]
                        FROM Invoices i
                        INNER JOIN Bookings b ON i.bookingId = b.bookingId
                        INNER JOIN Users u    ON b.guestId   = u.userId
                        INNER JOIN Rooms r    ON b.roomId    = r.roomId
                        ORDER BY i.invoiceId DESC";

                CloseConnection();
                OpenConnection();

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvPayments.DataSource = dt;

                // Color rows by status
                foreach (DataGridViewRow row in dgvPayments.Rows)
                {
                    if (row.Cells["Payment Status"].Value != null)
                    {
                        row.DefaultCellStyle.BackColor =
                            row.Cells["Payment Status"].Value.ToString() == "Paid"
                            ? Color.LightGreen
                            : Color.LightCoral;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

        // Load All Booking IDs into ComboBox
        void LoadAllBookingIds()
        {
            try
            {
                cmbBooking.Items.Clear();

                string query = @"SELECT b.bookingId, u.userName 
                         FROM Bookings b
                         INNER JOIN Users u ON b.guestId = u.userId
                         ORDER BY b.bookingId DESC";

                CloseConnection();
                OpenConnection();

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    cmbBooking.Items.Add(dr["bookingId"].ToString() + " - " + dr["userName"].ToString());
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

        private void dgvPayments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedInvoiceId = Convert.ToInt32(dgvPayments.Rows[e.RowIndex].Cells["Invoice ID"].Value);
                selectedStaffBookingId = Convert.ToInt32(dgvPayments.Rows[e.RowIndex].Cells["Booking ID"].Value);

                txtInvoiceId.Text = selectedInvoiceId.ToString();
                txtPayAmount.Text = dgvPayments.Rows[e.RowIndex].Cells["Total Amount"].Value.ToString();
                cmbPaymentMethod.Text = dgvPayments.Rows[e.RowIndex].Cells["Payment Method"].Value?.ToString() ?? "";
                cmbPaymentStatus.Text = dgvPayments.Rows[e.RowIndex].Cells["Payment Status"].Value.ToString();

                txtRoomCost.Text = "";
                txtServiceCost.Text = "";
                txtAccountNumber.Text = "";

                // Match combobox item
                foreach (var item in cmbBooking.Items)
                {
                    if (item.ToString().StartsWith(selectedStaffBookingId.ToString() + " -"))
                    {
                        cmbBooking.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void btnGenerateBill_Click_1(object sender, EventArgs e)
        {
            if (cmbBooking.SelectedItem == null)
            {
                MessageBox.Show("Please select a Booking!");
                return;
            }

            int bookingId = Convert.ToInt32(cmbBooking.SelectedItem.ToString().Split('-')[0].Trim());

            try
            {
                CloseConnection();
                OpenConnection();

                // Check if invoice already exists
                SqlCommand checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Invoices WHERE bookingId=@bookingId", con);
                checkCmd.Parameters.AddWithValue("@bookingId", bookingId);
                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (exists > 0)
                {
                    MessageBox.Show("Bill already generated for this Booking!");
                    return;
                }

                // Get room cost
                SqlCommand roomCmd = new SqlCommand(@"
            SELECT r.pricePerNight
            FROM Bookings b
            INNER JOIN Rooms r ON b.roomId = r.roomId
            WHERE b.bookingId=@bookingId", con);
                roomCmd.Parameters.AddWithValue("@bookingId", bookingId);
                double roomCost = Convert.ToDouble(roomCmd.ExecuteScalar());

                // Get service cost
                SqlCommand serviceCmd = new SqlCommand(@"
            SELECT ISNULL(SUM(cost),0)
            FROM Services
            WHERE bookingId=@bookingId", con);
                serviceCmd.Parameters.AddWithValue("@bookingId", bookingId);
                double serviceCost = Convert.ToDouble(serviceCmd.ExecuteScalar());

                double totalAmount = roomCost + serviceCost;

                // Insert invoice
                SqlCommand insertCmd = new SqlCommand(@"
            INSERT INTO Invoices(bookingId,totalAmount,paymentDate,paymentMethod,paymentStatus)
            VALUES(@bookingId,@totalAmount,NULL,NULL,'Unpaid')", con);
                insertCmd.Parameters.AddWithValue("@bookingId", bookingId);
                insertCmd.Parameters.AddWithValue("@totalAmount", totalAmount);
                insertCmd.ExecuteNonQuery();

                txtRoomCost.Text = roomCost.ToString();
                txtServiceCost.Text = serviceCost.ToString();
                txtPayAmount.Text = totalAmount.ToString();
                cmbPaymentStatus.Text = "Unpaid";

                MessageBox.Show("Bill Generated Successfully!");
                LoadAllPayments();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

        private void btnPayNow_Click(object sender, EventArgs e)
        {
            if (cmbBooking.SelectedItem == null)
            {
                MessageBox.Show("Please select a Booking!");
                return;
            }

            if (cmbPaymentMethod.Text == "")
            {
                MessageBox.Show("Please select Payment Method!");
                return;
            }

            int bookingId = Convert.ToInt32(cmbBooking.SelectedItem.ToString().Split('-')[0].Trim());

            try
            {
                CloseConnection();
                OpenConnection();

                // Check invoice exists
                SqlCommand checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Invoices WHERE bookingId=@bookingId", con);
                checkCmd.Parameters.AddWithValue("@bookingId", bookingId);
                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (exists == 0)
                {
                    MessageBox.Show("Please generate bill first!");
                    return;
                }

                // Check already paid
                SqlCommand statusCmd = new SqlCommand(
                    "SELECT paymentStatus FROM Invoices WHERE bookingId=@bookingId", con);
                statusCmd.Parameters.AddWithValue("@bookingId", bookingId);
                string status = statusCmd.ExecuteScalar()?.ToString();

                if (status == "Paid")
                {
                    MessageBox.Show("This booking is already Paid!");
                    return;
                }

                SqlCommand cmd = new SqlCommand(@"
            UPDATE Invoices
            SET paymentDate=@date, paymentMethod=@method, paymentStatus='Paid'
            WHERE bookingId=@bookingId", con);
                cmd.Parameters.AddWithValue("@date", DateTime.Now.Date);
                cmd.Parameters.AddWithValue("@method", cmbPaymentMethod.Text);
                cmd.Parameters.AddWithValue("@bookingId", bookingId);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    MessageBox.Show("Payment Successful!");
                    cmbPaymentStatus.Text = "Paid";
                    LoadAllPayments();
                    LoadDashboard();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            cmbBooking.SelectedIndex = -1;
            cmbPaymentMethod.Text = "";
            cmbPaymentStatus.Text = "";
            txtInvoiceId.Text = "";
            txtRoomCost.Text = "";
            txtServiceCost.Text = "";
            txtPayAmount.Text = "";
            txtAccountNumber.Text = "";
            selectedInvoiceId = 0;
            selectedStaffBookingId = 0;
        }

        private void btnLoadBills_Click_1(object sender, EventArgs e)
        {
            LoadAllPayments();
        }

        void LoadUsers()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Users", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvUsers.DataSource = dt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabStaff.SelectedTab = tabUser;
            LoadUsers();
        }

        private void btnLoadUsers_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void lblPendingServices_Click(object sender, EventArgs e)
        {

        }

        private void lblTotalRooms_Click(object sender, EventArgs e)
        {

        }
    }
}

