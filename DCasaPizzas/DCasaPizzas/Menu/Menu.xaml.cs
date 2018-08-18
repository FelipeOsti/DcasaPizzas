using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCasaPizzas.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu : MasterDetailPage
    {
        public Menu()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MenuMenuItem;
            if (item == null)
                return;

            if (item.Id == 9)
            {
                if (Application.Current.Properties.ContainsKey("usuar")) Application.Current.Properties.Remove("usuar");
                if (Application.Current.Properties.ContainsKey("senha")) Application.Current.Properties.Remove("senha");
                if (Application.Current.Properties.ContainsKey("tokenFace")) Application.Current.Properties.Remove("tokenFace");
                Application.Current.Properties.Clear();
                App.Current.MainPage = new NavigationPage(new DCasaPizzas.MainPage());
            }
            else
            {
                await abrirNovaPagina(item);
            }
            IsPresented = false;
            MasterPage.ListView.SelectedItem = null;
        }

        private async Task abrirNovaPagina(MenuMenuItem item)
        {
            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;
            await Detail.Navigation.PushAsync(page, true);
        }
    }
}