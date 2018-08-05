using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AppRomagnole.Models
{
    
    public class Grafico
    {
        public string sdsTitulo { get; set; }
        public List<DadosGrafico> Dados { get; set; }
        public List<string> Legendas { get; set; }
        public double ID_MENUAPP { get; set; }
        public int sdsTipoGrafico { get; set; }
    }

    public class DadosGrafico
    {
        public Entry Entries { get; set; }
        public string Label { get; set; }
        public CorGraf cor { get; set; }
    }

    public class Entry
    {
        public List<double> Value { get; set; }
        public List<CorGraf> coresPizza { get; set; }
    }

    public class CorGraf
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int A { get; set; }

        public CorGraf(int _r, int _g, int _b, int _a)
        {
            R = _r;
            G = _g;
            B = _b;
            A = _a;
        }
    } 
}
