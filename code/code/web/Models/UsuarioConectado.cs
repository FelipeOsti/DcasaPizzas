using AppRomagnole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppRoma.Models
{
    public class UsuarioConectado
    {
        public int Index { get; set; }
        public string ID { get; set; }
        public string IMEI { get; set; }
        public string  email { get; set; }
        public string Nome { get; set; }
        public string sdsDispositivo { get; set; }
        public string sdsVersao { get; set; }
    }
}