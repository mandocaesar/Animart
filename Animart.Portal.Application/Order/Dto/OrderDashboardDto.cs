using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animart.Portal.Order.Dto
{
    public class OrderDashboardDto
    {
        public int BDO { get; set; }
        public int Delivered { get; set; }
        public int Waiting { get; set; }
    }
}
