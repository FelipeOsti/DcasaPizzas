using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCasaPizzasWeb.Models
{
    public class ClientesAPP
    {
        public double ID_USUARIO { get; set; }
        public string DS_EMAIL { get; set; }
        public string DS_CPF { get; set; }
        public string NM_NOME { get; set; }
        public string DS_TELEFONE { get; set; }
        public string DS_ENDERECO { get; set; }
        public string NR_NUMERO { get; set; }
        public string DS_BAIRO { get; set; }
        public string DS_COMPLEMENTO { get; set; }
        public double NR_PONTOS { get; set; }
    }
}