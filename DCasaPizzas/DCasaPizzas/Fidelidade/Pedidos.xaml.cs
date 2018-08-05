using DCasaPizzas.Models;
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

namespace DCasaPizzas.Fidelidade
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Pedidos : INotifyPropertyChanged
    {
        public ListView ListView;
        ObservableCollection<Models.PedidoTela> lstPedidos = new ObservableCollection<Models.PedidoTela>();

        public Pedidos ()
		{
			InitializeComponent ();
            IsBusy = true;
            BindingContext = this;
            Title = "Meus Pedidos";
            ListView = ListViewPedidos;
            GetPedidos();
        }

        public async void GetPedidos()
        {
            try
            {
                Logic.Pedidos pedidos = new Logic.Pedidos();
                var peds = await pedidos.ListarPedidos();

                foreach (var ped in peds)
                {
                    lstPedidos.Add(new PedidoTela()
                    {
                        ID_PEDIDO = ped.ID_PEDIDO,
                        DS_CLIENTE = ped.DS_CLIENTE,
                        DT_PEDIDO = ped.DT_PEDIDO,
                        ID_USUARIO = ped.ID_USUARIO,
                        VL_PEDIDO = ped.VL_PEDIDO,
                        Title = "Pedido " + ped.ID_PEDIDO + " - " + ped.DT_PEDIDO //+ " - " + ped.DS_VLPEDIDO
                    });

                    foreach (var iten in ped.itens)
                    {
                        lstPedidos[lstPedidos.Count - 1].Add(new ItemPedido()
                        {
                            CD_PRODUTO = iten.CD_PRODUTO,
                            DS_PRODUTO = iten.DS_PRODUTO,
                            ID_ITEMPEDIDO = iten.ID_ITEMPEDIDO,
                            QT_PRODUTO = iten.QT_PRODUTO,
                            VL_TOTAL = iten.VL_TOTAL,
                            VL_UNITARIO = iten.VL_UNITARIO
                        });

                    }
                }

                ListViewPedidos.ItemsSource = lstPedidos;
                IsBusy = false;
            }
            catch
            {
                await DisplayAlert("Falha", "Uma falha ao carregar os pedidos ocorreu", "Que pena");
                IsBusy = false;
            }
        }
        
    }
}