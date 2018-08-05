using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppRoma.Models
{
    public class TarefaUNIFACE
    {
        public float ID_TAREFAAPP { get; set; }
        public string DS_EMAIL { get; set; }
        public string CD_SISTEMA { get; set; }
        public string CD_TAREFA { get; set; }
        public string CD_CHAVE { get; set; }
        public int FL_STATUS { get; set; }
        public DateTime DT_RETORNO { get; set; }
        public string DS_RETORNO { get; set; }
    }
}