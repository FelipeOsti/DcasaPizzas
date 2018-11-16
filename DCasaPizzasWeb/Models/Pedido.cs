using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCasaPizzasWeb.Models
{
    public class Pedido
    {
        public double ID_PEDIDO { get; set; }
        public string DT_PEDIDO { get; set; }
        public double ID_USUARIO { get; set; }
        public string DS_CLIENTE { get; set; }
        public double VL_PEDIDO { get; set; }
        public List<ItemPedido> itens { get; set; }
    }
}