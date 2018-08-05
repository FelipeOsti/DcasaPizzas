using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppRoma.Models
{
    public class MoedaCotacao
    {
        public int CD_MOEDA { get; set; }
        public string DS_MOEDA { get; set; }
        public string DS_SIGLA { get; set; }
        public DateTime DT_COTACAO { get; set; }
        public double VL_COTACAO { get; set; }
    }
}