using AppRomagnole.Logic;
using AppRomagnole.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRomagnole.Config
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MinhaConta : ContentPage
	{
        string pinDigitado = null;
        PINController pinC = new PINController();

        public MinhaConta ()
		{
			InitializeComponent();
            lblEmail.Text = MainPage.sdsEmail;
            lblNome.Text = MainPage.usuarAD.nome;
            lblCargo.Text = MainPage.usuarAD.cargo;
            lblCCusto.Text = MainPage.usuarAD.ccusto;

            carregaImagem();

            btMeuPIN.Clicked += BtMeuPINClicked;
            btSair.Clicked += BtSairClicked;

            VerificaPossuiPIN();
        }

        private async void carregaImagem()
        {
            try
            {
                string _imgUsuario = MainPage.urlIMG + "usuario.png";
                ldapController ldap = new ldapController();
                var imgS = await ldap.GetUserPicture(MainPage.sdsEmail);
                if (imgS == null)
                    imgUsuario.Source = _imgUsuario;
                else
                   imgUsuario.Source = imgS;
            }
            catch
            {
                MessageToast.LongMessage("Não foi possível carregar a foto do perfil");
            }
        }

        private async void VerificaPossuiPIN()
        {
            try
            {
                if (await pinC.PossuiPIN(MainPage.sdsEmail))
                    btMeuPIN.Text = "Alterar meu PIN";
            }
            catch (Exception ex)
            {
                MessageToast.ShortMessage(ex.Message);
            }
        }

        private void BtSairClicked(object sender, EventArgs e)
        {
            Logout _logout = new Logout();
            Navigation.PopAsync(true);
        }

        public async void BtMeuPINClicked(object sender, EventArgs e)
        {
            try
            {
                LoginSegundoNivel login2nv = new LoginSegundoNivel();

                if (await pinC.PossuiPIN(MainPage.sdsEmail))
                {
                    MessageToast.ShortMessage("Informe seu PIN ATUAL.");
                    LoginSegundoNivel.OnPinDigitado += LoginSegundoNivel_OnPinDigitadoConfirmaAsync;
                }
                else
                {
                    MessageToast.ShortMessage("Informe seu novo PIN");
                    LoginSegundoNivel.OnPinDigitado += LoginSegundoNivelOnPinDigitadoAsync;
                }
                await Navigation.PushAsync(login2nv);
            }
            catch (Exception ex)
            {
                MessageToast.ShortMessage(ex.Message);
            }
        }

        private async Task<bool> LoginSegundoNivel_OnPinDigitadoConfirmaAsync(string pin)
        {
            try
            {
                LoginSegundoNivel.OnPinDigitado -= LoginSegundoNivel_OnPinDigitadoConfirmaAsync;

                pin = await pinC.CriptografaAsync(pin);
                if (await pinC.VerificaPIN(pin, MainPage.sdsEmail))
                {
                    MessageToast.ShortMessage("Informe seu novo PIN");
                    LoginSegundoNivel.OnPinDigitado += LoginSegundoNivelOnPinDigitadoAsync;
                    return false;
                }
                else
                {
                    await DisplayAlert("PIN", "PIN informado não confere!", "OK");
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageToast.ShortMessage(ex.Message);
                return false;
            }
        }

        private async Task<bool> LoginSegundoNivelOnPinDigitadoAsync(string pin)
        {
            try
            {
                if (pinDigitado == null)
                {
                    pinDigitado = pin;
                    MessageToast.ShortMessage("Confirme seu novo PIN");
                    return false;
                }
                else
                {
                    LoginSegundoNivel.OnPinDigitado -= LoginSegundoNivelOnPinDigitadoAsync;
                    if (pin == pinDigitado)
                    {                        
                        var pinCripto = await pinC.CriptografaAsync(pin);
                        if (await pinC.SalvarPin(pinCripto, MainPage.sdsEmail))
                        {
                            await DisplayAlert("PIN", "O PIN foi cadastrado com sucesso!", "OK");
                        }
                        else
                        {
                            await DisplayAlert("PIN", "Houve um erro ao salvar o PIN!\nTente novamente!", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("PIN", "O PIN digitado não confere com o primeiro informado\nTente Novamente!", "OK");
                    }
                    pinDigitado = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageToast.ShortMessage(ex.Message);
                return false;
            }
            
        }
    }
}