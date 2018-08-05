using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppRoma.Models
{
    public class ConteudoEmail
    {
        public string titulo { get; set; }
        public string conteudo { get; set; }
        public string destino { get; set; }
        public bool isHTML { get; set; }
    }
}