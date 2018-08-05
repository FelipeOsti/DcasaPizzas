using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AppRomagnole.Models
{
    public class ItemPlanilhaFin : INotifyPropertyChanged
    {
        public long nrPlanilha { get; set; }
        public string dsCliente { get; set; }
        public string vlValor { get; set; }
        public string ddtPlanfc { get; set; }
        public int nnrAtraso { get; set; }
        public string nvlPedido { get; set; }
        public string imDetalhe { get; set; }
        public string imgSeta { get; set; }
        public string imgDivisao { get; set; }
        private bool _AtivaBotao;
        public bool AtivaBotao { get { return _AtivaBotao; } set { _AtivaBotao = value; OnPropertyChanged("AtivaBotao"); } }
        private bool _inProcess;
        public bool inProcess { get { return _inProcess; } set { _inProcess = value; OnPropertyChanged("inProcess"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed(this, new PropertyChangedEventArgs(name));
        }
        public string FL_TIPOPLAN { get; set; }
        public int ID_NIVEL { get; set; }
        public double CD_CLIENTE { get; set; }
    }
}
