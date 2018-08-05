using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DCasaPizzas.Models
{
    public class ItemPedido
    {
        public double ID_ITEMPEDIDO { get; set; }
        public string CD_PRODUTO { get; set; }
        public string DS_PRODUTO { get; set; }
        public double QT_PRODUTO { get; set; }
        public double VL_UNITARIO { get; set; }
        public double VL_TOTAL { get; set; }

        public string DS_VLTOTAL { get { return string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", VL_TOTAL); } }
    }
}