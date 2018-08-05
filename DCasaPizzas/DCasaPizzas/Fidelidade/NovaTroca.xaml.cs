using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCasaPizzas.Fidelidade
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NovaTroca : ContentPage
	{
        List<Models.Produto> lstProduto;

        double nnrPontos = 0;

		public NovaTroca (List<Models.Produto> _lstProd)
		{
			InitializeComponent ();
            lstProduto = _lstProd;

            Title = "Confirmar Troca";

            lstViewProdutos.ItemsSource = lstProduto;

            foreach (var prod in lstProduto)
            {
                nnrPontos += prod.NR_PONTOS;
            }
        }

        private async void btnConfirma_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (Menu.MenuDetail.instance.nnrPontos < nnrPontos)
                {
                    await DisplayAlert("Que Pena", "Seu saldo é insuficiente para realizar essa troca!", "Acumule mais pontos!");
                }
                else
                {
                    indiTroca.IsVisible = true;
                    btnConfirma.IsEnabled = false;
                    Logic.Fidelidade fidelidade = new Logic.Fidelidade();
                    bool bboOk = await fidelidade.SalvarTroca(lstProduto);
                    if (bboOk)
                    {
                        await DisplayAlert("Sucesso", "Troca realizada com sucesso! Vá até a loja para retirar os produtos!", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Que pena", "Alguma coisa aconteceu de errado! Tente novamente!", "OK");
                        btnConfirma.IsEnabled = true;
                        indiTroca.IsVisible = false;
                    }
                }
            }
            catch
            {
                await DisplayAlert("Que pena", "Alguma coisa aconteceu de errado! Tente novamente!", "OK");
            }
            finally
            {
                indiTroca.IsVisible = false;
                btnConfirma.IsEnabled = true;
                await Navigation.PopAsync(true);
            }
        }
    }
}