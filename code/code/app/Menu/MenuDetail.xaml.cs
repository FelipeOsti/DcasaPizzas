using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Input;
using Xam.Plugin.WebView.Abstractions;

namespace AppRomagnole.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuDetail : INotifyPropertyChanged
    {
        public static MenuDetail Current;

        public MenuDetail()
        {
            InitializeComponent();
            CarregaGraf();
            Current = this;
        }

        public async void CarregaGraf()
        {
            try
            {
                string html = null;

                if (MainPage.sdsEmail != "")
                {
                    var mes = DateTime.Now.Month;
                    var ano = DateTime.Now.Year;
                    string grafico = null;

                    string height = "90%";
                    if (Device.RuntimePlatform == Device.Android) height = "590px";


                    html = @"<html><body style='background-color:#e4e5e6'>";

                    grafico = MainPage.uriGraf + "?tipoGrafico=grafCarteiraGlobal&paramIN=,Consolidado Global," + mes + "," + ano + ",F";
                    html += @"<iframe width='100%' height='" + height + "' style='border:none;' scrolling='no' src='" + grafico + "'></iframe>";

                    grafico = MainPage.uriGraf + "?tipoGrafico=FaturadoMercado&paramIN=Consolidado,Faturamento%20Anual,," + ano + ",F";
                    html += @"<iframe width='100%' height='" + height + "' style='border:none;' scrolling='no' src='" + grafico + "'></iframe>";

                    grafico = MainPage.uriGraf + "?tipoGrafico=grafCarteira&paramIN=Consolidado,Detalhado Geral," + mes + "," + ano + ",F";
                    html += @"<iframe width='100%' height='" + height + "' style='border:none;' scrolling='no' src='" + grafico + "'></iframe>";

                    grafico = MainPage.uriGraf + "?tipoGrafico=grafCarteira&paramIN=Artefatos,Consolidado Artefatos," + mes + "," + ano + ",F";
                    html += @"<iframe width='100%' height='" + height + "' style='border:none;' scrolling='no' src='" + grafico + "'></iframe>";

                    grafico = MainPage.uriGraf + "?tipoGrafico=grafCarteira&paramIN=Transformadores,Consolidado Transformadores," + mes + "," + ano + ",F";
                    html += @"<iframe width='100%' height='" + height + "' style='border:none;' scrolling='no' src='" + grafico + "'></iframe>";

                    grafico = MainPage.uriGraf + "?tipoGrafico=AtingimentoMeta&paramIN=Consolidado,Atingimento%20Meta," + mes + "," + ano + ",F";
                    html += @"<iframe width='100%' height='" + height + "' style='border:none;' scrolling='no' src='" + grafico + "'></iframe>";

                    grafico = MainPage.uriGraf + "?tipoGrafico=grafCarteira&paramIN=Ferragens,Consolidado Ferragens," + mes + "," + ano + ",F";
                    html += @"<iframe width='100%' height='" + height + "' style='border:none;' scrolling='no' src='" + grafico + "'></iframe>";

                    grafico = MainPage.uriGraf + "?tipoGrafico=FaturadoMercado&paramIN=Consolidado,Faturamento Mês Atual," + mes + "," + ano + ",F";
                    html += @"<iframe width='100%' height='" + height + "' style='border:none;' scrolling='no' src='" + grafico + "'></iframe>";

                    grafico = MainPage.uriGraf + "?tipoGrafico=grafCarteira&paramIN=Onix,Consolidado Onix," + mes + "," + ano + ",F";
                    html += @"<iframe width='100%' height='" + height + "' style='border:none;' scrolling='no' src='" + grafico + "'></iframe>";

                    html += @"</body></html>";
                }
                else
                {
                    html = @"<html><body>";
                    html += "Realize login para visualizar as informações!";
                    html += @"</body></html>";
                }

                var htmlSource = new HtmlWebViewSource();
                htmlSource.Html = html;
                browser.Source = html;
            }
            catch
            {
                await DisplayAlert("Aviso", "Falha ao exibir os gráficos!", "Ok");
            }
        }
    }
}