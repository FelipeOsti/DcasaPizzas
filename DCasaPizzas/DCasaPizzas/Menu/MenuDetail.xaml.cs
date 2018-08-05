using DCasaPizzas.Fidelidade;
using DCasaPizzas.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCasaPizzas.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuDetail : INotifyPropertyChanged
    {
        ObservableCollection<Models.Produto> Produtos = new ObservableCollection<Models.Produto>();
        public static MenuDetail instance;
        public double nnrPontos;

        public MenuDetail()
        {
            InitializeComponent();
            instance = this;
            nrPontos.Text = "";
            BindingContext = this;

            buscarInformacoes();
        }

        private async void buscarInformacoes()
        {            
            await GetProdutos();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await GetPontos();
        }

        private async Task GetProdutos()
        {
            Produto prod = new Produto();
            lblMsgTroca.IsVisible = false;
            btTroca.IsEnabled = true;
            Produtos = await prod.getProdutos();
            lstViewProdutos.ItemsSource = Produtos;
        }

        public async Task GetPontos()
        {
            Logic.Fidelidade fidelidade = new Logic.Fidelidade();
            var pontos = await fidelidade.GetPontos(App.sdsEmail);
            nnrPontos = pontos;
            nrPontos.Text = pontos.ToString();

            if(pontos < 400)
            {
                lblMsgTroca.IsVisible = true;
                btTroca.IsEnabled = false;
            }
            else
            {
                lblMsgTroca.IsVisible = false;
                btTroca.IsEnabled = true;
            }
        }

        private async void btAcumular_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Acumular(), true);
        }


        private void lstViewProdutos_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                var produto = (Models.Produto)e.Item;
                var index = Produtos.IndexOf(produto);

                produto.BO_SELECTED = !produto.BO_SELECTED;

                Produtos.Remove(produto);
                Produtos.Insert(index, produto);
            }
        }

        private async void btTroca_Clicked(object sender, EventArgs e)
        {
            List<Models.Produto> lstProd = new List<Models.Produto>();

            double nqtPontos = 0;

            foreach(var prod in Produtos)
            {
                if (prod.BO_SELECTED) lstProd.Add(prod);
                nqtPontos += prod.NR_PONTOS;
            }

            if(nqtPontos < 400)
            {
                await DisplayAlert("Que pena", "Pontuação minima para troca é de [ 400 ] pontos", "Utilizar mais pontos");
                return;
            }

            await Navigation.PushAsync(new NovaTroca(lstProd), true);
            GetProdutos();
        }
    }
}