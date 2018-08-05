using AppRomagnole.Grafico;
using AppRomagnole.Logic;
using AppRomagnole.Models;
using AppRomagnole.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRomagnole.Forms.CompararPedidos
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Compara : INotifyPropertyChanged
    {
        private modelPedidos oldPedido;
        ObservableCollection<GroupPedidos> lstPedidos = new ObservableCollection<GroupPedidos>();
        GroupPedidos sPedido = new GroupPedidos() { GroupName = "Saiu" };
        GroupPedidos ePedido = new GroupPedidos() { GroupName = "Entrou" };

        public Compara ()
		{
            InitializeComponent ();
            this.IsBusy = true;
            BindingContext = this;
            listViewPedidos.ItemTapped += ListViewPedidos_ItemTapped;
            CarregaGraf();
            CarregaDifPedidos();
        }

        private void ListViewPedidos_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var pedido = e.Item as modelPedidos;
            MostrarOuEsconderPedido(pedido);
        }

        private void MostrarOuEsconderPedido(modelPedidos pedido)
        {
            if(oldPedido == pedido)
            {
                pedido.IsVisible = !pedido.IsVisible;
                AtualizarPedidos(pedido);
            }
            else
            {
                if(oldPedido != null)
                {
                    oldPedido.IsVisible = false;
                    AtualizarPedidos(oldPedido);
                }
                pedido.IsVisible = true;
                AtualizarPedidos(pedido);
            }

            oldPedido = pedido;
        }

        private void AtualizarPedidos(modelPedidos pedido)
        {
            if (sPedido.Contains(pedido))
            {
                int index = sPedido.IndexOf(pedido);
                sPedido.Remove(pedido);
                sPedido.Insert(index, pedido);
            }
            else if (ePedido.Contains(pedido))
            {
                int index = ePedido.IndexOf(pedido);
                ePedido.Remove(pedido);
                ePedido.Insert(index, pedido);
            }
        }

        private async void CarregaDifPedidos()
        {
            qtPedidoHoje.Text = "";
            qtCarteiraHoje.Text = "";
            qtPedidoOntem.Text = "";
            qtCarteiraOntem.Text = "";

            var data = DateTime.Now;
            qtPedidoHoje.Text = await GetQtdePedidos(data);
            data = data.AddDays(-1);
            qtPedidoOntem.Text = await GetQtdePedidos(data);

            data = DateTime.Now;
            qtCarteiraHoje.Text = await GetQtdeCarteira(data);
            data = data.AddDays(-1);
            qtCarteiraOntem.Text = await GetQtdeCarteira(data);

            await RecuperarComparacao();
        }

        private async Task RecuperarComparacao()
        {
            try
            {
                var data = DateTime.Now;
                string dia = data.Day.ToString();
                string mes = data.Month.ToString();
                string ano = data.Year.ToString();
                data = data.AddDays(-1);
                string diaC = data.Day.ToString();
                string mesC = data.Month.ToString();
                string anoC = data.Year.ToString();
                string sdsUrl = MainPage.apiURI + "CompararPedidos/GetComparaCarteira?sdsParam=Consolidado&dia=" + dia + "&mes=" + mes + "&ano=" + ano + "&diaC=" + diaC + "&mesC=" + mesC + "&anoC=" + anoC;
                var response = await RequestWS.RequestGET(sdsUrl);
                var retorno = await response.Content.ReadAsStringAsync();
                var pedidosAux = JsonConvert.DeserializeObject<List<modelPedidos>>(retorno);
                if (pedidosAux != null)
                {
                    foreach (modelPedidos ped in pedidosAux)
                    {
                        if (ped.dsSituacao == "Entrou")
                            ePedido.Add(ped);
                        else
                            sPedido.Add(ped);
                    }
                    lstPedidos.Add(sPedido);
                    lstPedidos.Add(ePedido);
                    listViewPedidos.ItemsSource = lstPedidos;

                }
            }
            catch(Exception ex)
            {
                MessageToast.LongMessage(ex.Message);
            }
        }

        private async Task<string> GetQtdePedidos(DateTime data)
        {
            string dia = data.Day.ToString();
            string mes = data.Month.ToString();
            string ano = data.Year.ToString();
            string sdsUrl = MainPage.apiURI + "CompararPedidos/GetQtdePedidos?sdsParam=Consolidado&dia=" + dia + "&mes=" + mes + "&ano=" + ano;
            var response = await RequestWS.RequestGET(sdsUrl);
            var retorno = await response.Content.ReadAsStringAsync();
            var a = (int)Double.Parse(retorno.Replace(".", ","));
            return a + "";
        }

        private async Task<string> GetQtdeCarteira(DateTime data)
        {
            string dia = data.Day.ToString();
            string mes = data.Month.ToString();
            string ano = data.Year.ToString();
            string sdsUrl = MainPage.apiURI + "CompararPedidos/GetQtdeCarteira?sdsParam=Consolidado&dia=" + dia + "&mes=" + mes + "&ano=" + ano;
            var response = await RequestWS.RequestGET(sdsUrl);
            var retorno = await response.Content.ReadAsStringAsync();
            var a = (int)Double.Parse(retorno.Replace(".",","));
            return a + "";
        }

        private async Task<LinhaChart> GetDadosLinha(string url)
        {
            GraficoController grafC = new GraficoController();
            return await grafC.GetDadosGrafComparar(url);
        }

        private async void CarregaGraf()
        {
            string dia = DateTime.Now.Day.ToString();
            string mes = DateTime.Now.Month.ToString();
            string ano = DateTime.Now.Year.ToString();

            Graf.posXLinha = 1;
            var linhaChart = await GetDadosLinha(MainPage.apiURI + "CompararPedidos/BuscarDadosGraficos?sdsParam=Consolidado&dia=" + dia + "&mes=" + mes + "&ano=" + ano);
            ViewGrafico viewGL = new ViewGrafico();
            viewGL.Parent = this;
            stkGrafico.Children.Add(await viewGL.CriaGrafico(linhaChart, 0, null));

            this.IsBusy = false;
        }
    }
}