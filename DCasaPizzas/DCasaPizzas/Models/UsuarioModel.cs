using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCasaPizzas.Models
{
    public class UsuarioModel
    {
        public double ID_USUARIO { get; set; }
        public string DS_EMAIL { get; set; }
        public string DS_CPF { get; set; }
        public string DS_SENHA { get; set; }
        public string NM_NOME { get; set; }
        public string DS_TELEFONE { get; set; }
        public string DS_ENDERECO { get; set; }
        public string NR_NUMERO { get; set; }
        public string DS_BAIRRO { get; set; }
        public string DS_COMPLEMENTO { get; set; }
        public string BO_FACEBOOK { get; set; }
    }
}