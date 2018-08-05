using AppRomagnole.Grafico;
using AppRomagnole.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppRomagnole.Logic
{
    class GraficoController
    {

        public GraficoController()
        {
        }

        public async Task<BarraChart> GetDadosGrafico(string sdsUrl, double nidMenuAPP)
        {
            int pos = sdsUrl.IndexOf("?");
            string url = sdsUrl.Substring(0, pos);
            url = url + "?nidMenuAPP=" + nidMenuAPP;
            string json = sdsUrl.Substring(pos + 1, sdsUrl.Length-pos-1);

            json = "[\"" + json + "\"]";
            json = json.Replace(",", "\",\"");

            var response = await RequestWS.RequestPOST(url, json);
            var retornoJson = await response.Content.ReadAsStringAsync();
            var lstDados = JsonConvert.DeserializeObject<Models.Grafico>(retornoJson);

            List<BarraSerieDados> series = new List<BarraSerieDados>();

            float pos_x = 1;
            foreach (DadosGrafico dados in lstDados.Dados)
            {
                List<BarraChartDadosEntry> entries = new List<BarraChartDadosEntry>();
                pos_x = Graf.posXBarra;
                int legenda = 0;
                foreach (double valor in dados.Entries.Value)
                {                    
                    entries.Add(new BarraChartDadosEntry(pos_x, (float)valor, lstDados.Legendas[legenda]));
                    pos_x++;
                    legenda++;
                }
                series.Add(new BarraSerieDados(entries, dados.Label, Color.FromRgba(dados.cor.R,dados.cor.G,dados.cor.B,dados.cor.A)));
            }
            Graf.posXBarra = pos_x;

            BarraChart chart = new BarraChart(series) { sdsTitulo = lstDados.sdsTitulo };

            return chart;
        }

        internal async Task<LinhaChart> GetDadosGrafComparar(string url)
        {
            var response = await RequestWS.RequestGET(url);
            var retornoJson = await response.Content.ReadAsStringAsync();
            var lstDados = JsonConvert.DeserializeObject<Models.Grafico>(retornoJson);

            List<LinhaChartDados> series = new List<LinhaChartDados>();

            float pos_x = 1;
            foreach (DadosGrafico dados in lstDados.Dados)
            {
                List<LinhaChartDadosEntry> entries = new List<LinhaChartDadosEntry>();
                pos_x = Graf.posXLinha;
                int legenda = 0;
                foreach (double valor in dados.Entries.Value)
                {
                    entries.Add(new LinhaChartDadosEntry(pos_x, (float)valor, lstDados.Legendas[legenda]));
                    pos_x++;
                    legenda++;
                }
                series.Add(new LinhaChartDados(entries, dados.Label, Color.FromRgba(dados.cor.R, dados.cor.G, dados.cor.B, dados.cor.A)));
            }
            Graf.posXLinha = pos_x;

            LinhaChart chart = new LinhaChart(series) { sdsTitulo = lstDados.sdsTitulo };

            return chart;
        }

        public async Task<LinhaChart> GetDadosGraficoLinha(string sdsUrl, double nidMenuAPP)
        {
            int pos = sdsUrl.IndexOf("?");
            string url = sdsUrl.Substring(0, pos);
            url = url + "?nidMenuAPP=" + nidMenuAPP;
            string json = sdsUrl.Substring(pos + 1, sdsUrl.Length - pos - 1);

            json = "[\"" + json + "\"]";
            json = json.Replace(",", "\",\"");

            var response = await RequestWS.RequestPOST(url, json);
            var retornoJson = await response.Content.ReadAsStringAsync();
            var lstDados = JsonConvert.DeserializeObject<Models.Grafico>(retornoJson);

            List<LinhaChartDados> series = new List<LinhaChartDados>();

            float pos_x = 1;
            foreach (DadosGrafico dados in lstDados.Dados)
            {
                List<LinhaChartDadosEntry> entries = new List<LinhaChartDadosEntry>();
                pos_x = Graf.posXLinha;
                int legenda = 0;
                foreach (double valor in dados.Entries.Value)
                {
                    entries.Add(new LinhaChartDadosEntry(pos_x, (float)valor, lstDados.Legendas[legenda]));
                    pos_x++;
                    legenda++;
                }
                series.Add(new LinhaChartDados(entries, dados.Label, Color.FromRgba(dados.cor.R, dados.cor.G, dados.cor.B, dados.cor.A)));
            }
            Graf.posXLinha = pos_x;

            LinhaChart chart = new LinhaChart(series) { sdsTitulo = lstDados.sdsTitulo };

            return chart;
        }

        public async Task<PizzaChart> GetDadosGraficoPizza(string sdsUrl, double nidMenuAPP)
        {
            int pos = sdsUrl.IndexOf("?");
            string url = sdsUrl.Substring(0, pos);
            url = url + "?nidMenuAPP=" + nidMenuAPP;
            string json = sdsUrl.Substring(pos + 1, sdsUrl.Length - pos - 1);

            json = "[\"" + json + "\"]";
            json = json.Replace(",", "\",\"");

            var response = await RequestWS.RequestPOST(url, json);
            var retornoJson = await response.Content.ReadAsStringAsync();
            var lstDados = JsonConvert.DeserializeObject<Models.Grafico>(retornoJson);

            PizzaDados serie = null;

            foreach (DadosGrafico dados in lstDados.Dados)
            {
                List<PizzaChartDadosEntry> entries = new List<PizzaChartDadosEntry>();
                int legenda = 0;
                foreach (double valor in dados.Entries.Value)
                {
                    var cor = dados.Entries.coresPizza[legenda];                    
                    entries.Add(new PizzaChartDadosEntry((float)valor, lstDados.Legendas[legenda], Color.FromRgba(cor.R,cor.G,cor.B,cor.A))); // pos_x, (float)valor, lstDados.Legendas[legenda]));
                    legenda++;
                }

                serie = new PizzaDados(entries);
            }

            PizzaChart chart = new PizzaChart(serie, lstDados.sdsTitulo);

            return chart;
        }

    }
}
