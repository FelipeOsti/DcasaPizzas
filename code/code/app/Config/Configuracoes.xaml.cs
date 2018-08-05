using AppRomagnole.Logic;
using AppRomagnole.Menu;
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
	public partial class Configuracoes : ContentPage
	{
		public Configuracoes ()
		{
			InitializeComponent();

            CarregaImagem();
            //lblNome.Text = MainPage.usuarAD.nome;

            btMinhaConta.Clicked += BtMinhaContaClicked;
        }

        private async void CarregaImagem()
        {
            try
            {
                string _imgConfig = MainPage.urlIMG + "settings_white.png";
                imgConfig.Source = _imgConfig;
            }
            catch
            {
                MessageToast.LongMessage("Não foi possível carregar a imagem!");
            }
        }

        private async void BtMinhaContaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MinhaConta(),true);
        }

        /*protected override bool OnBackButtonPressed()
        {
            var page = HomePage.Instance;// (Page)Activator.CreateInstance(typeof(MenuDetail));
            Menu.Menu.menu.Detail = new NavigationPage(page);

            return true;
        }*/
    }
}