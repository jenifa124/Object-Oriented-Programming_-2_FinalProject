using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementSystem.Models
{
 public class Guest:User
    {
        public override void ShowDashboard()
        {

            GuestDashboard guest =new GuestDashboard(Convert.ToInt32(this.UserId));

            guest.Show();
            


        }
    }
}
