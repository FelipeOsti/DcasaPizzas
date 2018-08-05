using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCasaPizzas.Login
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NovaSenha : ContentPage
	{
		public NovaSenha ()
		{
			InitializeComponent ();
		}

        private async void btNovaSenha_Clicked(object sender, EventArgs e)
        {
            btNovaSenha.IsEnabled = false;
            try
            {
                Logic.Usuario usuario = new Logic.Usuario();
                if (await usuario.VerificaEmailExiste(dsEmail.Text))
                {
                    actAguarde.IsVisible = true;
                    lblAguarde.IsVisible = true;
                    await usuario.NovaSenha(dsEmail.Text);
                    await DisplayAlert("Sucesso", "Verifique seu e-mail, uma nova senha foi enviada para você!", "Vou verificar!");
                    await Navigation.PopAsync(true);
                }
                else
                    await DisplayAlert("E-mail", "O e-mail digitado parece não estar correto! Verifique!", "Vou Verificar");
                actAguarde.IsVisible = false;
                lblAguarde.IsVisible = false;
                btNovaSenha.IsEnabled = true;
            }
            catch
            {
                await DisplayAlert("Que pena", "Uma falha aconteceu ao gerar sua nova senha! Tente novamente em alguns minutos!", "Irei Aguardar");
                actAguarde.IsVisible = false;
                lblAguarde.IsVisible = false;
                btNovaSenha.IsEnabled = true;
            }
        }
    }
}