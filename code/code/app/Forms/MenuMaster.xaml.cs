using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRomagnole.Forms
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuMaster : ContentPage
    {
        public ListView ListView;

        public MenuMaster()
        {
            InitializeComponent();

            string imagem = "";
            string imgLogo = "";

            switch (Device.RuntimePlatform)
            {
                case Device.Android: imagem = "menu.png"; imgLogo = "logomarcawhite.png"; break;
                default: imagem = "imagens/menu.png"; imgLogo = "imagens/logomarcawhite.png"; break;
            }

            NavigationPage.SetTitleIcon(this, imagem);
            imgGrid.Source = imgLogo;


            BindingContext = new MenuMasterViewModel();
            ListView = MenuItemsListView;
        }

        class MenuMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MenuMenuItem> MenuItems { get; set; }
            
            public MenuMasterViewModel()
            {
                MenuItems = new ObservableCollection<MenuMenuItem>(new[]
                {
                    new MenuMenuItem { Id = 0, Title = "Automação DataCenter"},
                    new MenuMenuItem { Id = 1, Title = "Monitoramento Fornos e Estufas" },
                    new MenuMenuItem { Id = 2, Title = "Liberação Planilha Financeira", TargetType=typeof(libPlanFin)}
                });
            }
            
            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}