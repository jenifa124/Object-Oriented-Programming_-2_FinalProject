using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementSystem.Models
{
    public class Invoice
    {

        public int InvoiceId { get; set; }
        public string BookingId { get; set; }
        public double TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
