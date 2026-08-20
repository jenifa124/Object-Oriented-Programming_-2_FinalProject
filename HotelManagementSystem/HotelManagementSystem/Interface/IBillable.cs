using HotelManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementSystem.Interface
{
    internal interface IBillable
    {
        double CalculateBill(string bookingId);
        void GenerateInvoice(Invoice invoice);
    }
}
