using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using AppRomagnole.Menu;

namespace AppRomagnole.Util
{
    class Logout : ContentPage
    {
        public Logout()
        {
            string autority = MainPage.authority;
            var bboOK = DependencyService.Get<IAutentificacao>().LogoutAsync(autority);
            if (bboOK)
            {
                MenuMaster.menu.LimpaMenu();
            }
        }
    }
}
