using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WebAppRoma.Models
{
    public class modelPedidos
    {
        public double nrPedido { get; set; }
        public string dsCliente { get; set; }
        public string dsClassif { get; set; }
        private double valorPedido;
        public double vlPedido
        {
            get { return this.valorPedido; }
            set
            {
                svlPedido = String.Format(new CultureInfo("pt-BR"), "{0:C}", value);
                valorPedido = value;
            }
        }
        public string svlPedido { get; set; }
        public string dsMotivo { get; set; }    
        private DateTime ddtFatura;
        public DateTime dtFatura
        {
            get { return this.ddtFatura; }
            set
            {
                sdtFatura = value.Day + "/" + value.Month + "/" + value.Year;
                ddtFatura = value;
            }
        }
        public string sdtFatura { get; set; }
        public bool IsVisible { get; set; }
        public string dsSituacao { get; set; }
    }
}