using AppRomagnole.Logic;
using AppRomagnole.Menu;
using AppRomagnole.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRomagnole.Grafico
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Graf : INotifyPropertyChanged
    {
        List<GraficoURL> lstGraficos = null;
        public static Graf Current;

        public static float posXBarra = 1;
        public static float posXLinha = 1;

        StackLayout layout = new StackLayout();
        DatePicker dtFiltro = new DatePicker()
        {
            Format = "MM/yyyy"
        };

        public Graf()
        {
            InitializeComponent();
            lstGraficos = Menu.Menu.item.lstGraficos;
            MontaLayout();
            dtFiltro.Date = DateTime.Now;
            RecuperarGraficos();
            Current = this;
        }

        public Graf(List<GraficoURL> lst)
        {
            InitializeComponent();
            lstGraficos = lst;
            MontaLayout();
            dtFiltro.Date = DateTime.Now;
            RecuperarGraficos();
            Current = this;
        }

        public async Task AbrirGraficoSegundoNivel(List<GraficoURL> lst)
        {
            await Navigation.PushAsync(new GrafSegundoNivel(lst),true);
        }

        public void MostraLoading()
        {
            this.IsBusy = true;
        }

        public void EscondeLoading()
        {
            this.IsBusy = false;
        }

        private async Task<BarraChart> GetDados(string url, double nidMenuAPP)
        {
            GraficoController grafC = new GraficoController();
            return await grafC.GetDadosGrafico(url, nidMenuAPP);
        }

        private async Task<LinhaChart> GetDadosLinha(string url, double nidMenuAPP)
        {
            GraficoController grafC = new GraficoController();
            return await grafC.GetDadosGraficoLinha(url, nidMenuAPP);
        }

        private async Task<PizzaChart> GetDadosPizza(string url, double nidMenuAPP)
        {
            GraficoController grafC = new GraficoController();
            return await grafC.GetDadosGraficoPizza(url, nidMenuAPP);
        }

        private void MontaLayout()
        {
            try
            {
                StackLayout stkLoading = new StackLayout();

                StackLayout stkFiltro = new StackLayout();
                Frame frameFiltro = new Frame() {
                    OutlineColor = Color.FromHex("#e4e5e6"),
                    Padding = 5,
                   // Margin = new Thickness(5, 5, 5, 5),
                    CornerRadius = 10
                };
                StackLayout stkFiltroInterno = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };

                Label lblFiltro = new Label()
                {
                    Text = "Mês/Ano Referência",
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize=10
                };
                Button btAtt = new Button()
                {
                    Text = "Atualizar",
                    BackgroundColor = Color.White,
                    TextColor = Color.FromHex("#a80c1b"),
                    BorderWidth = 1,
                    CornerRadius = 15,
                    BorderColor = Color.FromHex("#a80c1b"),
                    WidthRequest=100,
                    HorizontalOptions = LayoutOptions.EndAndExpand
                };
                btAtt.Clicked += BtAtt_Clicked;

                frameFiltro.Content = stkFiltroInterno;
                stkFiltroInterno.Children.Add(lblFiltro);
                stkFiltroInterno.Children.Add(dtFiltro);
                stkFiltroInterno.Children.Add(btAtt);
                stkFiltro.Children.Add(frameFiltro);
               
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

                layout.Children.Add(stkFiltro);
                layout.Children.Add(stkLoading);

                scrollGraf.Content = layout;
            }
            catch (Exception ex)
            {
                MessageToast.LongMessage(ex.Message);
            }

        }

        private void BtAtt_Clicked(object sender, EventArgs e)
        {
            layout.Children.Clear();
            MontaLayout();
            RecuperarGraficos();
        }

        private async void RecuperarGraficos()
        {
            try
            {
                MostraLoading();
                posXBarra = 1;
                posXLinha = 1;

                var mes = dtFiltro.Date.Month.ToString();
                var ano = dtFiltro.Date.Year.ToString();

                foreach (GraficoURL graf in lstGraficos)
                {
                    string url = MainPage.apiURI + graf.DS_URL;

                    url = url.Replace("[MES]", mes).Replace("[ANO]", ano);

                    switch (graf.FL_TIPOGRAFICO)
                    {
                        case 1: //Barra 
                        case 5: //Barra Horizontal
                            var BarraChart = await GetDados(url, graf.ID_MENUAPP);
                            if (graf.FL_TIPOGRAFICO == 5)
                                BarraChart.isHorizontal = true;
                            var viewGB = new ViewGrafico();
                            layout.Children.Add(await viewGB.CriaGrafico(BarraChart, graf.ID_MENUAPP, graf.lstGraficos));
                            break;
                        case 2: //Linha
                            var linhaChart = await GetDadosLinha(url, graf.ID_MENUAPP);
                            var viewGL = new ViewGrafico();
                            layout.Children.Add(await viewGL.CriaGrafico(linhaChart, graf.ID_MENUAPP, graf.lstGraficos));
                            break;
                        case 3: //Pizza
                            var pizzaChart = await GetDadosPizza(url, graf.ID_MENUAPP);
                            var viewGP = new ViewGrafico();
                            layout.Children.Add(await viewGP.CriaGrafico(pizzaChart, graf.ID_MENUAPP, graf.lstGraficos));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageToast.LongMessage(ex.Message);
            }
            finally
            {
                EscondeLoading();
            }
        }
    }
}