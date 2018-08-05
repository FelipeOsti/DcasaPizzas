using AppRomagnole.Logic;
using AppRomagnole.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRomagnole.Forms
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CodigoVerificacao : ContentPage
	{
		public CodigoVerificacao ()
		{
			InitializeComponent ();
            activityIndicator.IsVisible = false;
            activityIndicator.IsEnabled = false;

            imgLogo.Source = MainPage.urlIMG + "roma.png";
            imgEmail.Source = MainPage.urlIMG + "outlook.png";

            btOk.Clicked += BtOk_Clicked;
        }

        private async void BtOk_Clicked(object sender, EventArgs e)
        {
            try
            {
                btOk.IsEnabled = false;
                activityIndicator.IsEnabled = true;
                activityIndicator.IsVisible = true;

                AuthDispositivoController authDispositivo = new AuthDispositivoController(MainPage.sdsEmail);
                var bboOk = await authDispositivo.ValidaCodigoAprovacaoAsync(edCodigo.Text);
                if (bboOk)
                {
                    if (await authDispositivo.AprovaCodigoAPP(edCodigo.Text))
                    {
                        //await Navigation.PopModalAsync();
                        MainPage.Current.RealizaLoginToken("T"); //Dispositivo Autorizado
                    }
                    else
                    {
                        MessageToast.ShortMessage("Código de Verificação Inválido!");
                    }
                }
                else
                {
                    MessageToast.ShortMessage("Código de Verificação Inválido!");
                }
                btOk.IsEnabled = true;
            }
            catch(Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }
    }
}