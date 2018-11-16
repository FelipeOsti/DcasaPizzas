using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uol.PagSeguro.Constants;
using Uol.PagSeguro.Domain;

namespace DCasaPizzasWeb.Models.PagSeguro
{
    public class PagSeguroModel
    {
        public ClienteModel cliente { get; set; }
        public List<ProdutoModel> produtos { get; set; }
        public EnderecoModel enderecoEntrega { get; set; }

        public int shippingType { get; set; }
        public decimal shippingCost { get; set; }
        public string paymontReference { get; set; }
    }
}