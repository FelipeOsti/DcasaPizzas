using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCasaPizzasWeb.Models
{
    class IN_CHAVELICENCAModel
    {
        public long ID_CHAVELICENCA { get; set; }
        public long ID_CLIENTEINTERNO { get; set; }
        public string DS_CHAVE { get; set; }
        public string DT_VALIDADE { get; set; }
    }
}
