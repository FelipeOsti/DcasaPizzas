using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebAppRoma.Models;

namespace WebAppRoma.Controllers
{
    public class GraficoController : Controller
    {
        //https://localhost:44360/grafico/grafico?tipoGrafico=grafCarteira&paramIN=Consolidado,09,2017
        /// <summary>
        /// Impressão de graficos 
        /// </summary>
        /// <param name="tipoGrafico">
        /// -> grafCarteira = para Graficos da Carteira separados
        /// -> grafCarteiraGlobal = Gráficos da Carteira unificados
        /// -> FaturadoAnual = Faturamento Mês a Mês
        /// -> FaturadoMercado = Acumulado do Mês por Mercado
        /// -> CotacaoMoeda = Cotação das Moedas
        /// </param>
        /// <param name="paramIN">
        /// ----- Cotação Moeda
        /// [0] = Moeda
        /// [1] = Titulo 
        /// [2] = null
        /// [3] = null
        /// [4] = SIGE -> T ou F
        /// 
        /// -----
        /// [0] = Consolidado ou Artefatos ou Onix ou Transformares ou Ferragens;
        /// [1] = Titulo; 
        /// [2] = Mês; 
        /// [3] = Ano
        /// [4] = SIGE -> T ou F
        /// </param>
        /// <returns></returns>
        public ActionResult Grafico(string tipoGrafico,string paramIN)
        {
            try
            {
                List<string> args = null;
                if (paramIN != null)
                {
                    paramIN = paramIN.Replace("\"", "");
                    args = paramIN.Split(new char[] { ',' }).ToList();                    
                }

                if (tipoGrafico == null) tipoGrafico = "grafico";
                if (args == null) args = new List<string> { "Default" };

                ViewBag.Legenda = GetLegenda(tipoGrafico, args);

                var lstDados = GetDados(tipoGrafico, args);
                if (tipoGrafico == "FaturadoMercado") {
                    ViewBag.Dados = JsonConvert.DeserializeObject(lstDados, typeof(List<GrafDataSetPizza>));
                }
                else if(tipoGrafico == "AtingimentoMeta")
                {
                    ViewBag.Dados = lstDados;
                }
                else
                {
                    ViewBag.Dados = JsonConvert.DeserializeObject(lstDados, typeof(List<GrafDataSet>));
                };
                ViewBag.Titulo = args[1]; //Titulo 
                ViewBag.SIGE = args[4];

                if (tipoGrafico == "grafCarteiraGlobal") tipoGrafico = "grafCarteira";
                return View(tipoGrafico);
            }
            catch { throw; }
        }

        private String GetDados(string tipoGrafico, List<string> args)
        {
            GraficoDadosController grafDados = new GraficoDadosController();
            try
            {
                String lstRetorno = null;

                if (tipoGrafico == "grafCarteira")
                {
                    List<Double> lstFatura = null;
                    List<Double> lstPedido = null;
                    List<Double> lstOrcam = null;

                    lstPedido = grafDados.RecuperaPedidoCarteira(args);
                    lstFatura = grafDados.RecuperaFaturamentoCarteira(args);
                    lstOrcam = grafDados.RecuperaOrcamCarteira(args);

                    var lstDados = new List<GrafDataSet> {
                        new GrafDataSet()
                        {
                            data = lstOrcam,
                            backgroundColor = "rgba(0,112,192,0.8)",
                            borderColor = "rgba(0,112,192,1)",
                            label="Orçam",
                            stack=1,
                            stackLabel="Orçam"
                        },
                        new GrafDataSet()
                        {
                            data = lstPedido,
                            backgroundColor = "rgba(0,176,80,0.8)",
                            borderColor = "rgba(0,176,80,1)",
                            label="Carte",
                            stack=2,
                            stackLabel="Carte"
                        },
                        new GrafDataSet()
                        {
                            data = lstFatura,
                            backgroundColor = "rgba(142,94,162,0.8)",
                            borderColor = "rgba(142,94,162,1)",
                            label="Fatur",
                            stack=3,
                            stackLabel="Fatur"
                        }
                    };
                    lstRetorno = JsonConvert.SerializeObject(lstDados);
                }
                else if (tipoGrafico == "grafCarteiraGlobal")
                {
                    List<Double> lstFatura = null;
                    List<Double> lstPedido = null;
                    List<Double> lstOrcam = null;

                    List<string> aux = new List<string>() { "Ferragens", "", args[2], args[3] };
                    var lstFerPedido = grafDados.RecuperaPedidoCarteira(aux);
                    var lstFerFatura = grafDados.RecuperaFaturamentoCarteira(aux);
                    var lstFerOrcam = grafDados.RecuperaOrcamCarteira(aux);

                    aux[0] = "Artefatos";
                    var lstArtPedido = grafDados.RecuperaPedidoCarteira(aux);
                    var lstArtFatura = grafDados.RecuperaFaturamentoCarteira(aux);
                    var lstArtOrcam = grafDados.RecuperaOrcamCarteira(aux);

                    aux[0] = "Transformadores";
                    var lstTraPedido = grafDados.RecuperaPedidoCarteira(aux);
                    var lstTraFatura = grafDados.RecuperaFaturamentoCarteira(aux);
                    var lstTraOrcam = grafDados.RecuperaOrcamCarteira(aux);

                    aux[0] = "Onix";
                    var lstOniPedido = grafDados.RecuperaPedidoCarteira(aux);
                    var lstOniFatura = grafDados.RecuperaFaturamentoCarteira(aux);
                    var lstOniOrcam = grafDados.RecuperaOrcamCarteira(aux);

                    var nvlTotRomaOrcam = lstFerOrcam[3] + lstArtOrcam[3] + lstTraOrcam[3];
                    var nvlTotGloablOrcam = lstFerOrcam[3] + lstArtOrcam[3] + lstTraOrcam[3] + lstOniOrcam[3];
                    lstOrcam = new List<double>() { lstFerOrcam[3], lstArtOrcam[3], lstTraOrcam[3], nvlTotRomaOrcam, lstOniOrcam[3], nvlTotGloablOrcam };
                    var nvlTotRomaPedido = lstFerPedido[3] + lstArtPedido[3] + lstTraPedido[3];
                    var nvlTotGloablPedido = lstFerPedido[3] + lstArtPedido[3] + lstTraPedido[3] + lstOniPedido[3];
                    lstPedido = new List<double>() { lstFerPedido[3], lstArtPedido[3], lstTraPedido[3], nvlTotRomaPedido, lstOniPedido[3], nvlTotGloablPedido };
                    var nvlTotRomaFatura = lstFerFatura[3] + lstArtFatura[3] + lstTraFatura[3];
                    var nvlTotGloablFatura = lstFerFatura[3] + lstArtFatura[3] + lstTraFatura[3] + lstOniFatura[3];
                    lstFatura = new List<double>() { lstFerFatura[3], lstArtFatura[3], lstTraFatura[3], nvlTotRomaFatura, lstOniFatura[3], nvlTotGloablFatura };

                    var lstDados = new List<GrafDataSet> {
                        new GrafDataSet
                        {
                            data = lstOrcam,
                            label = "Orçam",
                            backgroundColor = "rgba(0,112,192,0.8)",
                            borderColor = "rgba(0,112,192,1)",
                            stack=1,
                            stackLabel="Orçam"
                        },
                        new GrafDataSet
                        {
                            data = lstPedido,
                            label = "Carte",
                            backgroundColor = "rgba(0,176,80,0.8)",
                            borderColor = "rgba(0,176,80,1)",
                            stack=2,
                            stackLabel="Carte"
                        },
                        new GrafDataSet
                        {
                            data = lstFatura,
                            label = "Fatur",
                            backgroundColor = "rgba(142,94,162,0.8)",
                            borderColor = "rgba(142,94,162,1)",
                            stack=3,
                            stackLabel="Fatur"
                        }
                    };
                    lstRetorno = JsonConvert.SerializeObject(lstDados);

                }
                else if (tipoGrafico == "FaturadoAnual")
                {
                    List<Double> lstFatura = null;
                    List<Double> lstOrcam = null;

                    lstFatura = grafDados.RecuperaFaturamentoMesAMes(args);
                    lstOrcam = grafDados.RecuperaOrcamMesAMes(args);

                    var lstDados = new List<GrafDataSet> {
                        new GrafDataSet()
                        {
                            data = lstOrcam,
                            backgroundColor = "rgba(0,112,192,0.8)",
                            borderColor = "rgba(0,112,192,1)",
                            label="Orçam",
                            stack=1,
                            stackLabel="Orçam"
                        },
                        new GrafDataSet()
                        {
                            data = lstFatura,
                            backgroundColor = "rgba(142,94,162,0.8)",
                            borderColor = "rgba(142,94,162,1)",
                            label="Fatur",
                            stack=3,
                            stackLabel="Fatur"
                        }
                    };

                    lstRetorno = JsonConvert.SerializeObject(lstDados);
                }
                else if (tipoGrafico == "FaturadoMercado")
                {
                    List<Double> lstMercado = new List<Double>();

                    lstMercado = grafDados.RecuperaFaturamentoMercado(args);

                    var lstDadosPizza = new List<GrafDataSetPizza>() {
                        new GrafDataSetPizza()
                        {
                            data = lstMercado,
                            backgroundColor = new List<string> {"#8e5ea2", "#3e95cd", "#3cba9f"},
                            label= new List<string>{"Concessionária","Privado","Exportação"},
                        }
                    };

                    lstRetorno = JsonConvert.SerializeObject(lstDadosPizza);
                }
                else if (tipoGrafico == "AtingimentoMeta") //Google
                {
                    List<Double> lstFatura = new List<Double>();
                    List<Double> lstOrcam = new List<Double>();

                    lstFatura = grafDados.RecuperaFaturamentoCarteira(args);
                    lstOrcam = grafDados.RecuperaOrcamCarteira(args);

                    var nvlMeta = lstFatura[3] / lstOrcam[3] * 100;

                    lstRetorno = Convert.ToInt32(Math.Floor(nvlMeta)) + "";
                }
                else if (tipoGrafico == "CotacaoMoeda")
                {
                    CotacaoMoedaController cotac = new CotacaoMoedaController();
                    int ncdMoeda = int.Parse(args[0]);
                    var lstCotac = cotac.GetCotacao(ncdMoeda);

                    List<Double> lstData = new List<Double>();

                    foreach (MoedaCotacao cotacMoeda in lstCotac)
                    {
                        lstData.Add(cotacMoeda.VL_COTACAO);
                    }

                    var lstDados = new List<GrafDataSet>() {
                        new GrafDataSet()
                        {
                            data = lstData,
                            backgroundColor = "rgba(0,112,192,0.8)",
                            borderColor = "rgba(0,112,192,1)",
                            label="Cotação Moeda",
                            stack=1,
                            stackLabel="Cotação Moeda"
                        }
                    };
                    lstRetorno = JsonConvert.SerializeObject(lstDados);
                }
                else
                {
                    var lstDados = new List<GrafDataSet> {
                        new GrafDataSet
                        {
                            data = new List<double> { 65, 59, 80, 81, 56, 55, 40 },
                            label = "Teste",
                            backgroundColor = "rgba(151,187,205,0.8)",
                            borderColor = "rgba(151,187,205,1)",
                            fill = false
                        },
                        new GrafDataSet
                        {
                            data = new List<double> { 28, 48, 40, 19, 86, 27, 90 },
                            label = "Dados",
                            backgroundColor="rgba(220,220,220,0.8)",
                            borderColor="rgba(220,220,220,1)",
                            fill = false
                        }
                    };
                    lstRetorno = JsonConvert.SerializeObject(lstDados);
                }
                
                return lstRetorno;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        private IEnumerable<string> GetLegenda(string tipoGrafico, List<string> args)
        {
            IEnumerable<string> lstLegenda = null;
            if (tipoGrafico == "grafCarteira")
            {
                lstLegenda = new[] { "Conces.", "Privado", "Export.", "Total" };
            }
            else if (tipoGrafico == "grafCarteiraGlobal")
            {
                lstLegenda = new[] { "Ferr.", "Artef.", "Transf.", "Roma", "Onix", "Global" };
            }
            else if (tipoGrafico == "FaturadoAnual")
            {
                lstLegenda = new[] { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez" };
            }
            else if (tipoGrafico == "FaturadoMercado")
            {
                lstLegenda = new[] { "Concessionária", "Privado", "Exportação" };
            }
            else if (tipoGrafico == "CotacaoMoeda")
            {
                CotacaoMoedaController cotac = new CotacaoMoedaController();
                int ncdMoeda = int.Parse(args[0]);
                var lstCotac = cotac.GetCotacao(ncdMoeda);

                List<String> lstData = new List<String>();

                foreach (MoedaCotacao cotacMoeda in lstCotac)
                {
                    lstData.Add(cotacMoeda.DT_COTACAO.Day + "/" + cotacMoeda.DT_COTACAO.Month);
                }
                lstLegenda = lstData;
            }
            return lstLegenda;
        }
    }
}
