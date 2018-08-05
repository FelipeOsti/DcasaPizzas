using System;

namespace WebAppRoma.Models
{
    public class PlanilhaFinanceira
    {
        public long NR_PLANILHA { get; set; }
        public string DS_CLIENTE { get; set; }
        public double VL_VALOR { get; set; }
        public string DT_PLANPFC { get; set; }
        public int NR_ATRASO { get; set; }
        public string DS_PARECER { get; set; }
        public string FL_TIPOPLAN { get; set; }
        public int ID_NIVEL { get; set; }
        public double CD_CLIENTE { get; set; }
        public string DS_EMAIL { get; set; }
        public double VL_PEDIDO { get; set; }
    }
}