using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCasaPizzasWeb.Models.PagSeguro
{
    public class EnderecoModel
    {
        public string pais { get; set; }
        public string estado { get; set; }
        public string cidade { get; set; }
        public string cep { get; set; }
        public string endereco { get; set; }
        public string bairro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
    }
}