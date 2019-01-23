using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCasaPizzasWeb.Models
{
    public class DocumentoFinModel
    {
        //long nidCliente, string sdsDocum, double vlDocum, double vlDesconto, int nrParcelas,int nnrDia, int nnrMes, int nnrAno
        public long nidCliente { get; set; }
        public string sdsDocum { get; set; }
        public double vlDocum { get; set; }
        public double vlDesconto { get; set; }
        public int nrParcelas { get; set; }
        public DateTime ddtPriParcela { get; set; }
        public string FL_TIPODOCUM { get; set; }
    }
}