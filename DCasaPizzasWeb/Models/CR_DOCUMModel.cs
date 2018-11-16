using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCasaPizzasWeb.Models
{
    public class CR_DOCUMModel
    {
        public long ID_DOCUM { get; set; }
        public string DS_DOCUM { get; set; }
        public long ID_CLIENTEINTERNO { get; set; }
        public double VL_DOCUM { get; set; }
        public string DT_CADASTRO { get; set; }

        public List<CR_PARCELAModel> parcelas { get; set; }
        public double VL_DESCONTO { get; set; }
    }
}
