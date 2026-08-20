using HotelManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementSystem.Interface
{
    internal interface IBookable
    {
        void BookRoom(Booking booking);
        void CancelBooking(string bookingId);
    }
}
