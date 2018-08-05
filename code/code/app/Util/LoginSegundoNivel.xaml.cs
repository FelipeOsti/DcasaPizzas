using AppRomagnole.PIN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRomagnole.Util
{
    public delegate Task<bool> OnPinDigitadoHandler(string pin);

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginSegundoNivel : ContentPage
    {
       
        public static event OnPinDigitadoHandler OnPinDigitado;

        public LoginSegundoNivel()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        public static async Task<bool> ValidaPIN(IList<char> pin)
        {
            string strPin = "";
            foreach (char ch in pin)
            {
                strPin += ch;
            }

            return await OnPinDigitado(strPin);
        }      
    }
}