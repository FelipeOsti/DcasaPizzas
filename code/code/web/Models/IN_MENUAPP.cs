using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppRoma.Models
{
    public class IN_MENUAPP
    {
        public double ID_MENUAPP { get; set; }
        public string DS_MENU { get; set; }
        public string  DS_CLASSE { get; set; }
        public string DS_ICONE { get; set; }
        public double ID_GRPMENUAPP { get; set; }
        public double ID_MENUAPPSUB { get; set; }
        public string DS_URL { get; set; }
        public int FL_GRAFICO { get; set; }
        public DateTime DT_INATIVO { get; set; }
        public bool BO_FORM { get; set; }
        public int NR_SEQUEN { get; set; }
        public int FL_TIPOGRAFICO { get; set; }
    }
}