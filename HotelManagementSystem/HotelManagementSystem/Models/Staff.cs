using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementSystem.Models
{
    public class Staff:User
    {
        public  override void ShowDashboard ()
        {

            StaffDashboard staff = new StaffDashboard();
            staff.Show();
        }
    }
}
