using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppRomagnole.Grafico
{
    public class PizzaChart : View
    {
        public PizzaDados SeriesDados;
        public string Title { get; set; }

        public PizzaChart(PizzaDados _dadosAux, string _title)
        {
            SeriesDados = _dadosAux;
            Title = _title;
        }
    }

    public class PizzaDados
    {
        public List<PizzaChartDadosEntry> Entries;

        public PizzaDados(List<PizzaChartDadosEntry> _entries)
        {
            Entries = _entries;
        }
    }

    public class PizzaChartDadosEntry
    {
        public float x { get; set; }
        public string Label { get; set; }
        public double r;
        public double g;
        public double b;
        public double a;

        public PizzaChartDadosEntry(float _x, string _label, Color cor)
        {
            x = _x;
            Label = _label;
            r = cor.R;
            g = cor.G;
            b = cor.B;
            a = cor.A;
        }
    }
}
