using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppRomagnole.Grafico
{
    public class BarraChart : View
    {
        public List<BarraSerieDados> SeriesDados;
        public string sdsTitulo { get; set; }
        public bool isHorizontal { get; set; }

        public BarraChart(List<BarraSerieDados> _dadosAux)
        {
            SeriesDados = _dadosAux;
            isHorizontal = false;
        }
    }

    public class BarraSerieDados
    {
        public List<BarraChartDadosEntry> Entries;
        public String SerieName;
        public double r;
        public double g;
        public double b;
        public double a;

        public BarraSerieDados(List<BarraChartDadosEntry> _entries, string _serieName, Color cor)
        {
            SerieName = _serieName;
            Entries = _entries;
            r = cor.R;
            g = cor.G;
            b = cor.B;
            a = cor.A;
        }
    }

    public class BarraChartDadosEntry
    {
        public float x { get; set; }
        public float y { get; set; }
        public string Label { get; set; }

        public BarraChartDadosEntry(float _x, float _y, string _label)
        {
            x = _x;
            y = _y;
            Label = _label;
        }
    }

}
