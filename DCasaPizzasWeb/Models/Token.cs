using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCasaPizzasWeb.Models
{
    public class Token
    {
        public DateTime DataCompra { get; set; }
        public DateTime DataTroca { get; set; }
        public DateTime DataValidade { get; set; }
        public int NrPontos { get; set; }
    }
}