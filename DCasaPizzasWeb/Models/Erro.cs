using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace DCasaPizzasWeb.Models
{
    public class Erro
    {
        public static void GerarErro(string sdsConteudo, string sdsEvento)
        {
            string sSource;
            string sLog;
            string sEvent;

            sLog = sdsEvento;
            sSource = "WebAPI DCasaPizzas";
            sEvent = sdsConteudo;

            if (!EventLog.SourceExists(sSource))            
                EventLog.CreateEventSource(sSource, sLog);

            EventLog myLog = new EventLog();
            myLog.Source = sSource;
            myLog.WriteEntry(sLog+" - "+sEvent);
        }
    }
}