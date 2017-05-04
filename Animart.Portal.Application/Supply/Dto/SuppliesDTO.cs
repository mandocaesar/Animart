using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animart.Portal.Supply.Dto
{
    public class SuppliesDTO
    {
        public List<SupplyItemDto> Supply { get; set; }

        public List<SupplyItemDto> PoSupply { get; set; }
    }
}
