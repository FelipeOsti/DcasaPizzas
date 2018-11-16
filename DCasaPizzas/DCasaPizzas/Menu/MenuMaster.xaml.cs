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

namespace DCasaPizzas.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuMaster : ContentPage
    {
        public ListView ListView;
        public static MenuMaster instance;

        public MenuMaster()
        {
            InitializeComponent();

            instance = this;
            string img = "menu.png";

            if (Device.RuntimePlatform == Device.iOS) Icon = img;
            NavigationPage.SetTitleIcon(this, img);

            lblNome.Text = "Ola, "+App.sdsNome;

            CriarMenu();
        }

        public void CriarMenu()
        {
            BindingContext = new MenuMasterViewModel();
            ListView = MenuItemsListView;
        }

        class MenuMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MenuMenuItem> MenuItems { get; set; }
            
            public MenuMasterViewModel()
            {
                if (App.LoggedApp)
                {
                    MenuItems = new ObservableCollection<MenuMenuItem>(new[]
                    {
                        new MenuMenuItem {Id = 0, Title = "Acumular Pontuação", TargetType = typeof(Fidelidade.Acumular)},
                        new MenuMenuItem {Id = 1, Title = "Meus Pedidos", TargetType = typeof(Fidelidade.Pedidos)},
                        new MenuMenuItem {Id = 2, Title = "Meus Pontos", TargetType = typeof(Fidelidade.MeusPontos)},
                        new MenuMenuItem {Id = 3, Title = "Configurações", TargetType = typeof(Login.Configuracoes)},
                        new MenuMenuItem {Id = 9, Title = "Sair" },
                    });
                }
                else
                {
                    MenuItems = new ObservableCollection<MenuMenuItem>(new[]
                    {
                        new MenuMenuItem {Id = 0, Title = "Fazer Login / Cadastre-se", TargetType = typeof(MainPage)}                        
                    });
                }
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