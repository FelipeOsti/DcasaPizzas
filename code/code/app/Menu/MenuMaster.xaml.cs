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
using AppRomagnole.Util;
using AppRomagnole.Forms;
using AppRomagnole.Config;
using AppRomagnole.Logic;
using Newtonsoft.Json;

namespace AppRomagnole.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuMaster : INotifyPropertyChanged
    {
        public ListView ListView;
        public ListView SubListView;
        public static MenuMaster menu;

        public void AtualizaLista(ItemMenu item)
        {
            if (item == null) return;

            for (int i = 0; i < LstViewMenu.Count; i++)
            {
                bool bboTrocaGrp = false;

                var grp = LstViewMenu[i];
                for (int x = 0; x < grp.Count; x++)
                {
                    bool bboTroca = false;

                    var itemMenu = grp[x];
                    if (itemMenu == item)
                    {
                        itemMenu.isSelected = true;
                        bboTroca = true;
                    }
                    else if (itemMenu.isSelected)
                    {
                        itemMenu.isSelected = false;
                        bboTroca = true;
                    }

                    if (bboTroca)
                    {
                        grp[x] = itemMenu;
                        bboTrocaGrp = true;
                    }
                }

                if (bboTrocaGrp) LstViewMenu[i] = grp;               
            }
        }

        private ObservableCollection<GroupMenu> _LstViewMenu{get; set;}
        public ObservableCollection<GroupMenu> LstViewMenu {
            get { return _LstViewMenu; }
            set
            {
                _LstViewMenu = value;
                OnPropertyChanged("LstViewMenu");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed(this, new PropertyChangedEventArgs(name));
        }

        public MenuMaster()
        {
            InitializeComponent();

            BindingContext = this;
            string imagem = "";
            string imgLogo = "";
            imagem = MainPage.urlIMG + "menu.png";
            imgLogo = MainPage.urlIMG + "logoRomaWhite.png";

            RecuperaImgUsuer();
            if(Device.RuntimePlatform == Device.iOS) Icon = "menu.png";           

            NavigationPage.SetTitleIcon(this, imagem);
            imgGrid.Source = imgLogo;
            lblNome.Text = MainPage.usuarAD.nome;

            RecuperaMenu();
            ListView = MenuItemsListView;

            menu = this;
        }

        private async void RecuperaImgUsuer()
        {
            ldapController ldap = new ldapController();
            string _imgUsuario = MainPage.urlIMG + "usuario.png";
            var img = await ldap.GetUserPicture(MainPage.sdsEmail);
            if (img == null)
                imgUser.Source = _imgUsuario; 
            else
                imgUser.Source = img;
        }

        public void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Menu.menu.ListView_ItemSelected(sender, e);
        }

        private async Task<ObservableCollection<GroupMenu>> GetMenuItemList()
        {
            MenuController menuCont = new MenuController();
            return await menuCont.GetMenu();
        }

        public void LimpaMenu()
        {
            var lstGroupMenu = new ObservableCollection<GroupMenu>();
            var GrpInicio = new GroupMenu() { GroupName = "Inicio" };
            GrpInicio.Add(new ItemMenu()
            {
                Title = "Login",
                TitleColor = Color.Gray,
                IconSource = MainPage.urlIMG + "login.png",
                TargetTypeType = typeof(Login),
                bboForm = false,
            });
            lstGroupMenu.Add(GrpInicio);

            MainPage.sdsEmail = "";
            MainPage.usuarAD = null;
            MenuItemsListView.SelectedItem = null;
            LstViewMenu = lstGroupMenu;
            Menu.menu.LimpaDetail();
            lblNome.Text = "";
        }

        public async void RecuperaMenu()
        {
            lblNome.Text = MainPage.usuarAD.nome;
            LstViewMenu = await GetMenuItemList();
        }
    }
}