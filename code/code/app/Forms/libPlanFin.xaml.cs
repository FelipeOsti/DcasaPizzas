using AppRomagnole.Logic;
using AppRomagnole.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AppRomagnole.Menu;
using AppRomagnole.Util;
using System.Windows.Input;
using System.Linq;

//https://emojipedia.org/google/

namespace AppRomagnole.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class libPlanFin : INotifyPropertyChanged
    {
        private bool _bboCarregando;
        public bool bboCarregando {
            get {return _bboCarregando; }
            set
            {
                _bboCarregando = value;
                OnPropertyChanged("bboCarregando");
            }
        }

        
        private bool _bboMostraBusca;
        public bool bboMostraBusca
        {
            get { return _bboMostraBusca; }
            set
            {
                _bboMostraBusca = value;
                OnPropertyChanged("bboMostraBusca");
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

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;
                    lblNenhum.IsVisible = false;

                    RecuperaPlan();

                    IsRefreshing = false;
                });
            }
        }
        CultureInfo minhaCultura = new CultureInfo("pt-BR");

        public static ObservableCollection<ItemPlanilhaFin> listaPlanilhas;
        public static ObservableCollection<ItemPlanilhaFin> listaPlanCompleta;

        public libPlanFin()
		{
			InitializeComponent();

            BindingContext = this;

            //pt-BR usada como base
            minhaCultura.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            minhaCultura.DateTimeFormat.ShortTimePattern = "HH:mm";
            minhaCultura.NumberFormat.NumberDecimalDigits = 2;
            minhaCultura.NumberFormat.NumberGroupSeparator = "_";
            minhaCultura.NumberFormat.NumberDecimalSeparator = ",";

            RecuperaPlan();
            filtroPlan.TextChanged += FiltroPlan_TextChanged;
            listPlanilhas.ItemSelected += ListPlanilhas_ItemSelected;
        }

        private void FiltroPlan_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = filtroPlan.Text;
            var result = listaPlanCompleta.Where(x => x.dsCliente.ToLower().Contains(filter.ToLower()) || x.nrPlanilha.ToString().Contains(filter));
            listPlanilhas.ItemsSource = result;
        }

        private async void ListPlanilhas_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (listPlanilhas.SelectedItem == null) return;
            ItemPlanilhaFin item = (ItemPlanilhaFin)e.SelectedItem;
            if (item == null) return;

            //ItemPlanilhaFin item = (ItemPlanilhaFin)imagem.BindingContext;

            await Navigation.PushAsync(new DetPlanFin(item));
            listPlanilhas.SelectedItem = null;
        }

        public async void RecuperaPlan()
        {
            listPlanilhas.ItemsSource = null;
            listPlanilhas.ItemsSource = await getPlanilhas();
        }

        private async Task<ObservableCollection<ItemPlanilhaFin>> getPlanilhas()
        {         
            listaPlanilhas = new ObservableCollection<ItemPlanilhaFin>();
            List<PlanilhaFinanceira> lstItemPl = new List<PlanilhaFinanceira>();
            try
            {
                await ApareceLoading();

                PlanilhasController PlCont = new PlanilhasController();
                lstItemPl = await PlCont.ListarAsync();

                if(lstItemPl.Count == 0)
                    lblNenhum.IsVisible = true;
                else
                    lblNenhum.IsVisible = false;

                foreach (var itemPl in lstItemPl)
                {
                    listaPlanilhas.Add(new ItemPlanilhaFin() { nrPlanilha = itemPl.NR_PLANILHA, CD_CLIENTE=itemPl.CD_CLIENTE, ID_NIVEL=itemPl.ID_NIVEL ,dsCliente = itemPl.DS_CLIENTE, imDetalhe = MainPage.urlIMG + "detalhe.png", imgSeta = MainPage.urlIMG + "setadir.png", imgDivisao = MainPage.urlIMG + "linhadivisao.png", vlValor = String.Format(minhaCultura, "{0:C}", itemPl.VL_VALOR), ddtPlanfc = itemPl.DT_PLANPFC, nnrAtraso = itemPl.NR_ATRASO, inProcess = false, AtivaBotao = true, FL_TIPOPLAN = itemPl.FL_TIPOPLAN,nvlPedido = String.Format(minhaCultura, "{0:C}", itemPl.VL_PEDIDO) });
                }

                listaPlanCompleta = listaPlanilhas;
            }
            catch (Exception ex)
            {
                MessageToast.ShortMessage(ex.Message);
                listaPlanilhas = null;
            }
            finally
            {
                await SomeLoading();
            }
            return listaPlanilhas;
        }

        public async Task ApareceLoading()
        {
            bboCarregando = true;
            bboMostraBusca = false;
        }

        public async Task SomeLoading()
        {
            bboCarregando = false;
            if(lblNenhum.IsVisible)
                bboMostraBusca = false;
            else
                bboMostraBusca = true;
        }

        public async void BtAprovaClick(object sender, EventArgs e)
        {
            try
            {
                Button botao = (Button)sender;
                ItemPlanilhaFin item = (ItemPlanilhaFin)botao.BindingContext;

                item.inProcess = true;
                item.AtivaBotao = false;

                PlanilhaFinanceira planilha = new PlanilhaFinanceira()
                {
                    NR_PLANILHA = item.nrPlanilha,
                    DS_CLIENTE = "",
                    VL_VALOR = 0,
                    DS_PARECER = "Parecer automático pelo App Romagnole",
                    FL_TIPOPLAN = item.FL_TIPOPLAN,
                    ID_NIVEL = item.ID_NIVEL,
                    CD_CLIENTE = item.CD_CLIENTE
                };

                PlanilhasController planC = new PlanilhasController();
                var retorno = await planC.LiberaReprovaPlanilha(planilha, "A");

                retorno = retorno.Replace("\"", "");
                retorno = retorno.Trim();

                if (retorno == "" || retorno == null)
                    listaPlanilhas.Remove(item);
                else
                {
                    MessageToast.ShortMessage(retorno);
                    item.inProcess = false;
                    item.AtivaBotao = true;
                }
            }
            catch (Exception ex)
            {
                MessageToast.ShortMessage(ex.Message);
            }
        }

        public async void BtReprovaClick(object sender, EventArgs e)
        {
            try
            {
                Button botao = (Button)sender;
                ItemPlanilhaFin item = (ItemPlanilhaFin)botao.BindingContext;

                item.inProcess = true;
                item.AtivaBotao = false;

                PlanilhaFinanceira planilha = new PlanilhaFinanceira()
                {
                    NR_PLANILHA = item.nrPlanilha,
                    DS_CLIENTE = "",
                    VL_VALOR = 0,
                    DS_PARECER = "Parecer automático pelo App Romagnole",
                    FL_TIPOPLAN = item.FL_TIPOPLAN,
                    ID_NIVEL = item.ID_NIVEL,
                    CD_CLIENTE = item.CD_CLIENTE
                };

                PlanilhasController planC = new PlanilhasController();
                var retorno = await planC.LiberaReprovaPlanilha(planilha, "R");

                retorno = retorno.Replace("\"", "");
                retorno = retorno.Trim();

                if (retorno == "" || retorno == null)
                    listaPlanilhas.Remove(item);
                else
                {
                    MessageToast.ShortMessage(retorno);
                    item.inProcess = false;
                    item.AtivaBotao = true;
                }
            }
            catch(Exception ex)
            {
                MessageToast.ShortMessage(ex.Message);
            }
        }

        public async void BtDetalhePlanilha(object sender, EventArgs e)
        {
            Image imagem = (Image)sender;
            ItemPlanilhaFin item = (ItemPlanilhaFin)imagem.BindingContext;

            await Navigation.PushAsync(new DetPlanFin(item));
        }

        /*protected override bool OnBackButtonPressed()
        {
            var page = HomePage.Instance;// (Page)Activator.CreateInstance(typeof(MenuDetail));
            Menu.Menu.menu.Detail = new NavigationPage(page);
            
            return true;
        }*/
    }
}