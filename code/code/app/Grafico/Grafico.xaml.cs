using AppRomagnole.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRomagnole.Grafico
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Grafico : ContentPage
	{
        ItemMenu item = null;

        public Grafico ()
		{
			InitializeComponent();
            dtFiltro.Date = DateTime.Now;

            item = Menu.Menu.item;

            CarregaGraf();
            btAtt.Clicked += BtAtt_Clicked;

        }

        private void BtAtt_Clicked(object sender, EventArgs e)
        {
            CarregaGraf();
        }

        private async void CarregaGraf()
        {
            try
            {
                var mes = dtFiltro.Date.Month;
                var ano = dtFiltro.Date.Year;

                string height = "90%";
                if (Device.RuntimePlatform == Device.Android) height = "490px";

                string grafico = null;
                string html = null;
                html = @"<html><head></head><body style='background-color:#e4e5e6'>";

                foreach (GraficoURL graf in item.lstGraficos)
                {
                    grafico = MainPage.uriGraf+graf.DS_URL;
                    grafico = grafico.Replace("[TITULO]", graf.DS_TITULO);
                    grafico = grafico.Replace("[MES]", mes.ToString());
                    grafico = grafico.Replace("[ANO]", ano.ToString());
                    html += @"<iframe width='100%' height='"+ height + "' style='border:none;' scrolling='no' src='" + grafico + "'></iframe>";
                };

                html += @"</body></html>";
                //var htmlSource = new HtmlWebViewSource();
                //htmlSource.Html = html;
                browser.Source = html;// htmlSource;
            }
            catch
            {
                //await DisplayAlert("Aviso", "Falha ao exibir os gráficos!", "Ok");
            }
        }

        /*protected override bool OnBackButtonPressed()
        {
            var page = HomePage.Instance;// (Page)Activator.CreateInstance(typeof(MenuDetail));
            Menu.Menu.menu.Detail = new NavigationPage(page);
            return true;
        }*/

    }
}