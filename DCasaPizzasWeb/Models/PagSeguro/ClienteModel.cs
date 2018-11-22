using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uol.PagSeguro.Domain;

namespace DCasaPizzasWeb.Models.PagSeguro
{
    public class ClienteModel
    {
        public string documento { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string ddd { get; set; }
        public string telefone { get; set; }
    }
}