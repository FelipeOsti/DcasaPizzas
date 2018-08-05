using AppRomagnole.Logic;
using AppRomagnole.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRomagnole.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu : MasterDetailPage
    {
        public static Menu menu;
        public static ItemMenu item;
        private string sdsTituloOld = "";

        public Menu()
        {
            try
            {
                InitializeComponent();

                //Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(MenuDetail))) { };
                Detail = new NavigationPage(HomePage.Instance);//new NavigationPage(MenuDetail.Current);
                menu = this;
            }
            catch
            {
                throw;
            }
        }

        public void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (MasterPage.ListView.SelectedItem == null) return;
            try
            {
                //VerificaPermissao();
                //item = null;
                //if (e.SelectedItem is ItemMenu)
                // {
                //    item = e.SelectedItem as ItemMenu;
                //    if (item == null)
                //        return;
                //}
                //if (item == null) return;
                //if (sdsTituloOld == item.Title)
                //{
                //    sdsTituloOld = "";
                //    return;
                //}
                //sdsTituloOld = item.Title;
                

                item = e.SelectedItem as ItemMenu;
                if (item == null) return;

                SelecionaItem(item);

                if (item.bboForm == false)
                {
                    Activator.CreateInstance(item.TargetTypeType);
                    return;
                }
                else
                {
                    var page = (Page)Activator.CreateInstance(item.TargetTypeType);
                    page.Title = item.Title;
                    //Detail = new NavigationPage(page);
                    Detail.Navigation.PushAsync(page,true);
                }                
            }
            catch(Exception ex)
            {
                DisplayAlert("Erro", ex.Message, "OK");
                //throw;
            }
            finally
            {
                //sdsTituloOld = "";                
                MasterPage.ListView.SelectedItem = null;
                IsPresented = false;
            }
        }

        private void SelecionaItem(ItemMenu item)
        {
            MenuMaster.menu.AtualizaLista(item);
        }

        private async Task VerificaPermissao()
        {
            AuthDispositivoController dispositivo = new AuthDispositivoController(MainPage.sdsEmail);
            var dispAuth = await dispositivo.DispositivoAutorizadoAsync();
            if (dispAuth == "F")
            {
                MessageToast.LongMessage("Acesso não autorizado! Entre em contato com a GSIS!");
                return;
            }
        }

        public async void LimpaDetail()
        {
            Detail = new NavigationPage(HomePage.Instance);// MenuDetail.Current);
            await HomePage.Instance.CarregaGraficos();
            //MenuDetail.Current.CarregaGraf();
            IsPresented = false;
            MasterPage.ListView.SelectedItem = null;
        }
    }
}