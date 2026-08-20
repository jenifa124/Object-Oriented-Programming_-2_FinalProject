using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementSystem.Interface
{
    internal interface ISearchable
    {
        DataTable Search(string keyword);
    }
}
