using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppRoma.Models
{
    public class Usuario
    {
        public string CD_USUARIO { get; set; }
        public int CD_MATR { get; set; }
        public string NM_NOME { get; set; }
        public string DS_EMAIL { get; set; }
    }
}