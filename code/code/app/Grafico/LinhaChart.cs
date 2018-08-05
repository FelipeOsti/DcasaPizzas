using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppRomagnole.Grafico
{
    public class LinhaChart : View
    {
        public int height { get; set; }
        public int width { get; set; }

        public List<LinhaChartDados> SeriesDados;
        public string sdsTitulo { get; set; }

        public LinhaChart(List<LinhaChartDados> _dadosAux)
        {
            SeriesDados = _dadosAux;
        }
    }

    public class LinhaChartDados
    {
        public List<LinhaChartDadosEntry> Entries;
        public String SerieName;
        public double r;
        public double g;
        public double b;
        public double a;

        public LinhaChartDados(List<LinhaChartDadosEntry> _entries, string _serieName, Color cor)
        {
            SerieName = _serieName;
            Entries = _entries;
            r = cor.R;
            g = cor.G;
            b = cor.B;
            a = cor.A;
        }
    }

    public class LinhaChartDadosEntry
    {
        public float x { get; set; }
        public float y { get; set; }
        public string Label { get; set; }

        public LinhaChartDadosEntry(float _x, float _y, string _label)
        {
            x = _x;
            y = _y;
            Label = _label;
        }
    }

}
