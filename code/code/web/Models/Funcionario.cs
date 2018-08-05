using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppRoma.Models
{
    public class Funcionario
    {
        public int CD_ESTAB { get; set; }
        public int CD_FUNC { get; set; }
        public string NM_FUNC { get; set; }
        public string CD_CCUSTO { get; set; }
        public string CD_CARGO { get; set; }
    }
}