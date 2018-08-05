using System;
using System.Collections.Generic;
using System.Linq;

namespace DCasaPizzas.Models
{
    public class Produto
    {
        public double ID_PRODUTO { get; set; }
        public string CD_PRODUTO { get; set; }
        public string DS_PRODUTO { get; set; }
        public double NR_PONTOS { get; set; }
        public double VL_VALOR { get; set; }
        public string DS_TAMANHO { get; set; }
        public string DS_PONTOS { get { return NR_PONTOS + " pts"; } }
        public bool BO_SELECTED { get; set; }
    }
}