using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCasaPizzasWeb.Models.PagSeguro
{
    public class ProdutoModel
    {
        public string id { get; set; }
        public string descricao { get; set; }
        public int qtde { get; set; }
        public decimal unitario { get; set; }
    }
}