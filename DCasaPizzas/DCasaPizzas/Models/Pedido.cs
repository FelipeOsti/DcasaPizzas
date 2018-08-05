using System;
using System.Collections.Generic;
using System.Globalization;

namespace DCasaPizzas.Models
{
    public class Pedido
    {
        public double ID_PEDIDO { get; set; }
        public DateTime DT_PEDIDO { get; set; }
        public double ID_USUARIO { get; set; }
        public string DS_CLIENTE { get; set; }
        public double VL_PEDIDO { get; set; }
        public List<ItemPedido> itens { get; set; }

        public string DS_VLPEDIDO { get { return string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", VL_PEDIDO); } }
    }

    public class PedidoTela : List<ItemPedido>
    {
        public string Title { get; set; }
        public double ID_PEDIDO { get; set; }
        public DateTime DT_PEDIDO { get; set; }
        public double ID_USUARIO { get; set; }
        public string DS_CLIENTE { get; set; }
        public double VL_PEDIDO { get; set; }
        //public List<ItemPedido> itens { get; set; }

        public string DS_VLPEDIDO { get { return string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", VL_PEDIDO); } }

        public string DS_IDPEDIDO { get { return "N. " + ID_PEDIDO; } }

        public int DtPedidoDia
        {
            get
            {
                return DT_PEDIDO.Day;
            }
        }

        public string DtPedidoMesAno
        {
            get
            {
                return DT_PEDIDO.Month + "/" + DT_PEDIDO.Year;
            }
        }
    }
}