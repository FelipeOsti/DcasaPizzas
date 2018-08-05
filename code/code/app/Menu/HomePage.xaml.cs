using AppRomagnole.Grafico;
using AppRomagnole.Logic;
using AppRomagnole.Models;
using AppRomagnole.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRomagnole.Menu
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : INotifyPropertyChanged
    {
        public static HomePage Instance;

		public HomePage ()
		{
            Title = "Inicio";
			InitializeComponent ();
            CarregaGraficos();
            Instance = this;
        }

        private async Task<List<Models.Grafico>> GetDados()
        {
            MenuController menu = new MenuController();
            return await menu.GetInicio();
        }

        public async Task CarregaGraficos()
        {
            try
            {
                StackLayout layout = new StackLayout();
                StackLayout stkLoading = new StackLayout();

                ActivityIndicator indiLoading = new ActivityIndicator()
                {
                    Color = Color.Gray,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    IsEnabled = true,
                    IsRunning = true,
                    BindingContext = this
                };
                indiLoading.SetBinding(ActivityIndicator.IsVisibleProperty, "IsBusy");
                stkLoading.Children.Add(indiLoading);
                Label lblLoading = new Label()
                {
                    TextColor = Color.Gray,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    BindingContext = this,
                    Text = "Aguarde, carregando gráficos"
                };
                lblLoading.SetBinding(Label.IsVisibleProperty, "IsBusy");
                stkLoading.Children.Add(lblLoading);

                layout.Children.Add(stkLoading);
                scrollGraf.Content = layout;

                this.IsBusy = true;

                Graf.posXBarra = 1;
                Graf.posXLinha = 1;

                var lstGraficos = await GetDados();

                if (lstGraficos.Count == 0)
                {
                    MessageToast.LongMessage("Nenhum gráfico favorito! Acesse o menu DashBoard e favorite seus gráficos!");
                }
                else
                {
                    foreach (Models.Grafico grafico in lstGraficos)
                    {
                        if (grafico.sdsTipoGrafico == 1 || grafico.sdsTipoGrafico == 5) //Barra e Barra Horizontal
                        {
                            List<BarraSerieDados> lstBarra = new List<BarraSerieDados>();
                            float valorX = 1;
                            foreach (DadosGrafico dados in grafico.Dados)
                            {
                                List<BarraChartDadosEntry> lstEntries = new List<BarraChartDadosEntry>();
                                int idLegenda = 0;
                                valorX = Graf.posXBarra;
                                foreach (double valorY in dados.Entries.Value)
                                {
                                    var Entry = new BarraChartDadosEntry(valorX, (float)valorY, grafico.Legendas[idLegenda]);
                                    lstEntries.Add(Entry);
                                    valorX++;
                                    idLegenda++;
                                }
                                BarraSerieDados serie = new BarraSerieDados(lstEntries, dados.Label, Color.FromRgba(dados.cor.R, dados.cor.G, dados.cor.B, dados.cor.A));
                                lstBarra.Add(serie);
                            }
                            Graf.posXBarra = valorX;

                            BarraChart barraChart = new BarraChart(lstBarra)
                            {
                                sdsTitulo = grafico.sdsTitulo
                            };
                            if (grafico.sdsTipoGrafico == 5) //Horizontal
                                barraChart.isHorizontal = true;
                            var viewGB = new ViewGrafico();
                            layout.Children.Add((await viewGB.CriaGrafico(barraChart, grafico.ID_MENUAPP, null)));
                        }
                        else if (grafico.sdsTipoGrafico == 2) //Linha
                        {
                            List<LinhaChartDados> lstBarra = new List<LinhaChartDados>();

                            float valorX = 1;
                            foreach (DadosGrafico dados in grafico.Dados)
                            {
                                List<LinhaChartDadosEntry> lstEntries = new List<LinhaChartDadosEntry>();
                                int idLegenda = 0;
                                valorX = Graf.posXLinha;
                                foreach (double valorY in dados.Entries.Value)
                                {
                                    var Entry = new LinhaChartDadosEntry(valorX, (float)valorY, grafico.Legendas[idLegenda]);
                                    lstEntries.Add(Entry);
                                    valorX++;
                                    idLegenda++;
                                }
                                LinhaChartDados serie = new LinhaChartDados(lstEntries, dados.Label, Color.FromRgba(dados.cor.R, dados.cor.G, dados.cor.B, dados.cor.A));
                                lstBarra.Add(serie);
                            }
                            Graf.posXLinha = valorX;
                            LinhaChart linhaChart = new LinhaChart(lstBarra)
                            {
                                sdsTitulo = grafico.sdsTitulo
                            };
                            var viewGL = new ViewGrafico();
                            layout.Children.Add((await viewGL.CriaGrafico(linhaChart, grafico.ID_MENUAPP,null)));
                        }
                        else if (grafico.sdsTipoGrafico == 3) //Pizza
                        {
                            List<PizzaChartDadosEntry> lstEntries = new List<PizzaChartDadosEntry>();

                            foreach (DadosGrafico dados in grafico.Dados)
                            {

                                int idLegenda = 0;
                                foreach (double valorY in dados.Entries.Value)
                                {
                                    var cor = dados.Entries.coresPizza[idLegenda];
                                    lstEntries.Add(new PizzaChartDadosEntry((float)valorY, grafico.Legendas[idLegenda], Color.FromRgba(cor.R, cor.G, cor.B, cor.A)));
                                    idLegenda++;
                                }
                            }

                            PizzaDados pizzaDados = new PizzaDados(lstEntries);
                            PizzaChart pizza = new PizzaChart(pizzaDados, grafico.sdsTitulo);
                            var viewGP = new ViewGrafico();
                            layout.Children.Add((await viewGP.CriaGrafico(pizza, grafico.ID_MENUAPP,null)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Falha", ex.Message, "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}