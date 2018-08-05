using DCasaPizzas.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCasaPizzas.Fidelidade
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MeusPontos : INotifyPropertyChanged
    {
        ObservableCollection<Token> lstTrocas = new ObservableCollection<Token>();
        ObservableCollection<Token> lstTrocasExpirar = new ObservableCollection<Token>();

        public MeusPontos ()
		{
			InitializeComponent ();
            Title = "Meus Pontos";
            BindingContext = this;
            IsBusy = true;
            GetTrocas();
            GetExpirar();
		}

        private async void GetExpirar()
        {
            Logic.Fidelidade fidelidade = new Logic.Fidelidade();
            lstTrocasExpirar = await fidelidade.ListaPontosExpirando();
            ListViewExpirar.ItemsSource = lstTrocasExpirar;
            IsBusy = false;
        }

        private async void GetTrocas()
        {
            Logic.Fidelidade fidelidade = new Logic.Fidelidade();
            lstTrocas = await fidelidade.ListarPontos();
            ListViewTrocas.ItemsSource = lstTrocas;
        }
    }
}