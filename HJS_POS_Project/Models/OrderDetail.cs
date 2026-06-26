using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJS_POS_Project.Models
{
    public class OrderDetail
    {
        public int DetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
