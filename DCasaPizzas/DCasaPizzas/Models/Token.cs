using System;
using System.Collections.Generic;

namespace DCasaPizzas.Models
{
    public class Token
    {
        public DateTime DataCompra { get; set; }
        public DateTime DataTroca { get; set; }
        public DateTime DataValidade { get; set; }
        public int NrPontos { get; set; }

        public int DtCompraDia
        {
            get
            {
                return DataCompra.Day;
            }
        }

        public string DtCompraMesAno
        {
            get
            {
                return DataCompra.Month+"/"+ DataCompra.Year;
            }
        }

        public int DtValidadeDia
        {
            get
            {
                return DataValidade.Day;
            }
        }

        public string DtValidadeMesAno
        {
            get
            {
                return DataValidade.Month + "/" + DataValidade.Year;
            }
        }

        public string sNrPontos
        {
            get
            {
                return "+ " + NrPontos + " pts";
            }
        }

        public string sNrPontosNeg
        {
            get
            {
                return "- " + NrPontos + " pts";
            }
        }
    }
}