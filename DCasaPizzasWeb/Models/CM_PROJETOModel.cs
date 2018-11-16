using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCasaPizzasWeb.Models
{
    class CM_PROJETOModel
    {
        public long ID_PROJETO { get; set; }
        public string DS_PROJETO { get; set; }
        public long ID_CLIENTEINTERNO { get; set; }
        public int NR_MESES { get; set; }
        public double VL_PROJETO { get; set; }
        public int NR_PARCELAS { get; set; }
        public string DS_SITUACAO { get; set; }
    }
}
