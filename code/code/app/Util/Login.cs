using AppRomagnole.Forms;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using AppRomagnole.Menu;
using AppRomagnole.Logic;

namespace AppRomagnole.Util
{
    class Login
    { 
        public static ModelAutenticacao auth;

        static public string authority = MainPage.authority;
        public string resourceURI = MainPage.resourceURI;
        public string clientID = MainPage.clientID;
        public string clientReturnURI = MainPage.clientReturnURI;

        public Login()
        {
            if (MenuMaster.menu != null && MainPage.sdsEmail == "") LoginAuto(); //Após vencimento do Token ou logoff/login execução automática.
        }

        private async void LoginAuto()
        {
            try
            {
                MainPage.adfs = await RealizaLogin();
            }
            catch (Exception ex)
            {
                MessageToast.ShortMessage(ex.Message);
            }
        }

        public async Task<ModelAutenticacao> RealizaLogin()
        {
            try
            {
                auth = await DependencyService.Get<IAutentificacao>().AutenticarAsync(authority, resourceURI, clientID, clientReturnURI);
                if (MenuMaster.menu != null)
                {
                    MainPage.sdsEmail = auth.auth.UserInfo.DisplayableId;

                    ldapController ldap = new ldapController();
                    MainPage.usuarAD = new Models.UsuarioAD();
                    MainPage.usuarAD = await ldap.GetUsuarioAD(MainPage.sdsEmail);

                    Menu.Menu.menu.LimpaDetail();
                    MenuMaster.menu.RecuperaMenu();                    
                }
                auth.SegundaAutenticacao = true;

            }
            catch(Exception ex)
            {
                throw;
            }
            return auth;
        }
    }
}
