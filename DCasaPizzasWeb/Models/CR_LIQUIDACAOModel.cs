using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCasaPizzasWeb.Models
{
    class CR_LIQUIDACAOModel
    {
        public long ID_LIQUIDACAO { get; set; }
        public long ID_PARCELA { get; set; }
        public double VL_LIQUIDACAO { get; set; }
        public string DT_LIQUIDACAO { get; set; }
    }
}
