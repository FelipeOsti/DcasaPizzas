using DCasaPizzas.Logic;
using DCasaPizzas.Login;
using DCasaPizzas.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DCasaPizzas
{
	public partial class App : Application
	{
        internal static string sdsNome;
        internal static string sdsUriImg;
        internal static string sdsEmail;
        internal static long IdUsuario;

        public static bool LoggedApp { get; set; }
        public static bool bboLoginAuto { get; set; }

        public App ()
		{
			InitializeComponent();
            LoggedApp = false;

            MainPage = new Menu.Menu();
            VerificaLogin();            
        }

        private async void VerificaLogin()
        {
            bool bboFace = false;
            if (Application.Current.Properties.ContainsKey("usuar"))
            {
                string sdsUsuar = (string)Application.Current.Properties["usuar"];
                string sdsSenha = (string)Application.Current.Properties["senha"];
                await VerificaLoginNormalAsync(sdsUsuar, sdsSenha);
            }
            else if (Application.Current.Properties.ContainsKey("tokenFace"))
            {
                bboFace = true;
                VerificaLoginFacebook();
            }

            if (LoggedApp && !bboFace)
            {
                await MenuDetail.instance.GetPontos();
                MenuMaster.instance.CriarMenu();
            }
            //else
            //    App.Current.MainPage = new MainPage();
        }

        private void VerificaLoginFacebook()
        {
            var loginF = new FacebookLogin();
            loginF.Login();
        }

        private async Task VerificaLoginNormalAsync(string sdsUsuar, string sdsSenha)
        {
            Usuario user = new Usuario();
            var ok = await user.VerificaUsuarioSenha(new Models.UsuarioModel { DS_EMAIL = sdsUsuar, DS_SENHA = sdsSenha });
            if (ok)
            {
                LoggedApp = true;
                if (Application.Current.Properties.ContainsKey("usuar"))
                {
                    Application.Current.Properties["usuar"] = sdsUsuar;
                    Application.Current.Properties["senha"] = sdsSenha;
                }
                else
                {
                    Application.Current.Properties.Add("usuar", sdsUsuar);
                    Application.Current.Properties.Add("senha", sdsSenha);
                }

                sdsEmail = sdsUsuar;
                sdsNome = await user.GetNome();
                IdUsuario = await user.GetIDUsuario(sdsUsuar);
            }
            else
            {
                LoggedApp = false;
            }
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
