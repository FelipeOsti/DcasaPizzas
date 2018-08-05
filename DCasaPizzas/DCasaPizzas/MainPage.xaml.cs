using DCasaPizzas.Logic;
using DCasaPizzas.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DCasaPizzas
{
	public partial class MainPage : ContentPage
	{
        public static string apiURI = "http://solariweb.azurewebsites.net/API/";

        public MainPage()
        {
            try
            {
                InitializeComponent();
                btFacebook.Clicked += BtLogin_ClickedAsync;
                Title = "Identifique-se";

                VerificaLoginAuto();
            }
            catch(Exception ex)
            {
                DisplayAlert("Login", "Falha ao realizar o login! Tente novamente!", "Que pena");
            }
        }

        private async void VerificaLoginAuto()
        {
            if (Application.Current.Properties.ContainsKey("usuar"))    
            {
                usuar.Text = (string)Application.Current.Properties["usuar"];
                sdsSenha.Text = (string)Application.Current.Properties["senha"];
                btLogin_Clicked(this, new EventArgs());
            }
            if (Application.Current.Properties.ContainsKey("tokenFace"))
            {
                BtLogin_ClickedAsync(this, new EventArgs());
            }
        }

        private async void BtLogin_ClickedAsync(object sender, EventArgs e)
        {
            var LoginFacebook = new FacebookLogin();
            await Navigation.PushAsync(LoginFacebook);
        }

        public static void AbrirMenu()
        {
            App.Current.MainPage = new Menu.Menu();
        }

        private void SemCadastro_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new NovoUsuario(), true);
        }

        private async void btLogin_Clicked(object sender, EventArgs e)
        {
            try
            {
                indiLogin.IsVisible = true;
                btLogin.IsEnabled = false;
                Usuario user = new Usuario();
                var ok = await user.VerificaUsuarioSenha(new Models.UsuarioModel { DS_EMAIL = usuar.Text, DS_SENHA = sdsSenha.Text });
                if (ok)
                {
                    if (Application.Current.Properties.ContainsKey("usuar"))
                    {
                        Application.Current.Properties["usuar"] = usuar.Text;
                        Application.Current.Properties["senha"] = sdsSenha.Text;
                    }
                    else
                    {
                        Application.Current.Properties.Add("usuar", usuar.Text);
                        Application.Current.Properties.Add("senha", sdsSenha.Text);
                    }

                    App.sdsEmail = usuar.Text;
                    App.sdsNome = await user.GetNome();
                    App.IdUsuario = await user.GetIDUsuario(usuar.Text);
                    AbrirMenu();


                    if (await user.SenhaProvisoria())
                    {
                        await DisplayAlert("Senha Provisória", "Você está utilizando a senha provisória! Considere mudar", "Vou Mudar em breve");
                    }
                }
                else
                {
                    await DisplayAlert("Falha", "Usuário ou senha inválido", "Ok");
                }
                btLogin.IsEnabled = true;
                indiLogin.IsVisible = false;
            }
            catch
            {
                btLogin.IsEnabled = true;
                indiLogin.IsVisible = false;
                await DisplayAlert("Login", "Falha ao realizar o Login! Tente novamente!", "Que pena");
            }
        }

        private async void EsqueciSenha_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NovaSenha(),true);
        }
    }
}
