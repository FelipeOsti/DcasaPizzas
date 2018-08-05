using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AppRomagnole.Models;
using AppRomagnole.Logic;
using AppRomagnole.Util;
using System.ComponentModel;
using System.Globalization;

namespace AppRomagnole.Forms
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DetPlanFin : INotifyPropertyChanged
    {
        ItemPlanilhaFin _item;

		public DetPlanFin (ItemPlanilhaFin item)
		{
			InitializeComponent ();

            nnrPlanila.Text = item.nrPlanilha.ToString();
            sdsCliente.Text = item.dsCliente;
            nvlLimiteCr.Text = item.vlValor;
            ddtPlanilha.Text = item.ddtPlanfc;
            //nnrAtraso.Text = item.nnrAtraso.ToString();
            //nvlLimiteCr.Text = item.nvlPedido;

            nvlPedido.Text = item.nvlPedido;

            btAprova.Clicked += BtAprova_Clicked;
            btReprova.Clicked += BtReprova_Clicked;

            loading.IsVisible = false;

            _item = item;

            imgSepara.Source = MainPage.urlIMG + "linhadivisao.png";

        }

        private async void BtReprova_Clicked(object sender, EventArgs e)
        {
            try
            {
                var parecer = sdsParecer.Text;
                if (parecer == "" || parecer == null)
                    parecer = "Parecer automátivo pelo App Romagnole";

                btAprova.IsEnabled = false;
                btReprova.IsEnabled = false;
                loading.IsVisible = true;

                PlanilhaFinanceira planilha = new PlanilhaFinanceira()
                {
                    NR_PLANILHA = _item.nrPlanilha,
                    DS_CLIENTE = "",
                    VL_VALOR = 0,
                    DS_PARECER = parecer,
                    FL_TIPOPLAN = _item.FL_TIPOPLAN,
                    ID_NIVEL = _item.ID_NIVEL,
                    CD_CLIENTE = _item.CD_CLIENTE
                };

                PlanilhasController planC = new PlanilhasController();
                var retorno = await planC.LiberaReprovaPlanilha(planilha, "R");

                retorno = retorno.Replace("\"", "");
                retorno = retorno.Trim();

                if (retorno == "" || retorno == null)
                {
                    libPlanFin.listaPlanilhas.Remove(_item);
                    await Navigation.PopAsync(true);
                }
                else
                {
                    MessageToast.ShortMessage(retorno);
                }
            }
            catch (Exception ex)
            {
                MessageToast.ShortMessage(ex.Message);
            }
            finally
            {
                btAprova.IsEnabled = true;
                btReprova.IsEnabled = true;
                loading.IsVisible = false;
            }
        }

        private async void BtAprova_Clicked(object sender, EventArgs e)
        {
            try
            {
                var parecer = sdsParecer.Text;
                if (parecer == "" || parecer == null)
                    parecer = "Parecer automátivo pelo App Romagnole";

                btAprova.IsEnabled = false;
                btReprova.IsEnabled = false;
                loading.IsVisible = true;

                PlanilhaFinanceira planilha = new PlanilhaFinanceira()
                {
                    NR_PLANILHA = _item.nrPlanilha,
                    DS_CLIENTE = "",
                    VL_VALOR = 0,
                    DS_PARECER = parecer,
                    FL_TIPOPLAN = _item.FL_TIPOPLAN,
                    ID_NIVEL = _item.ID_NIVEL,
                    CD_CLIENTE = _item.CD_CLIENTE
                };

                PlanilhasController planC = new PlanilhasController();
                var retorno = await planC.LiberaReprovaPlanilha(planilha, "A");

                retorno = retorno.Replace("\"", "");
                retorno = retorno.Trim();

                btAprova.IsEnabled = true;
                btReprova.IsEnabled = true;
                loading.IsVisible = false;

                if (retorno == "" || retorno == null)
                {
                    libPlanFin.listaPlanilhas.Remove(_item);
                    await Navigation.PopAsync(true);
                }
                else
                {
                    MessageToast.ShortMessage(retorno);
                }
            }
            catch (Exception ex)
            {
                MessageToast.ShortMessage(ex.Message);
            }            
        }
    }
}