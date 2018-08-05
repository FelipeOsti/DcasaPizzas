using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppRoma.Models
{
    public class GraficoUsuario
    {
        public float ID_GRAFAPPUSU { get; set; }
        public float ID_MENUAPP{ get; set; }
        public string DS_EMAIL { get; set; }
        public bool BO_FAVORITO { get; set; }
    }
}