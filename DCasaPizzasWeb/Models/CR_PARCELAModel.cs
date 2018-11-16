using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCasaPizzasWeb.Models
{
    public class CR_PARCELAModel
    {
        public long ID_PARCELA { get; set; }
        public long ID_DOCUM { get; set; }
        public double VL_PARCELA { get; set; }
        public double VL_PAGO { get; set; }
        public string DT_VENCIMENTO { get; set; }
    }
}
