using DCasaPizzas.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace DCasaPizzas.Fidelidade
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Acumular : ContentPage
    { 
		public Acumular ()
		{
			InitializeComponent ();
            nrToken.Text = "";
            Title = "Acumular Pontos";
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            indiAcumulando.IsVisible = true;
            lblAcumulando.IsVisible = true;

            btAcumular.IsEnabled = false;
            Logic.Fidelidade fidelidade = new Logic.Fidelidade();
            var retorno = await fidelidade.ValidarToken(nrToken.Text);

            if (retorno == "T") await DisplayAlert("Parabéns", "Seu token foi acumulado com sucesso!", "OK");
            if (retorno == "V") await DisplayAlert("Expirado", "Seu token já expirou!", "Que Pena");
            if (retorno == "U") await DisplayAlert("Já utilizado", "Seu token já foi utilizado!", "Que Pena");
            if (retorno == "F") await DisplayAlert("Nada por aqui", "Seu token não foi encontrado!", "Que Pena");

            nrToken.Text = "";
            btAcumular.IsEnabled = true;
            indiAcumulando.IsVisible = false;
            lblAcumulando.IsVisible = false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void btQRCODE_Clicked(object sender, EventArgs e)
        {
            OpenScaner();
        }

        private async void OpenScaner()
        {
            var ScannerPage = new ZXingScannerPage() { Title = "Aponde para o QR-CODE" };

            ScannerPage.OnScanResult += (result) =>
            {
                // Parar de escanear
                ScannerPage.IsScanning = false;

                // Retorno com o código escaneado
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();
                    nrToken.Text = result.Text;
                    Button_Clicked(this, new EventArgs());
                });
            };

            await Navigation.PushAsync(ScannerPage);
        }
    }
}