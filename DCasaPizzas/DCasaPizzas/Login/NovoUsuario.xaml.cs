using DCasaPizzas.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCasaPizzas.Login
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NovoUsuario : ContentPage
	{
		public NovoUsuario ()
		{
			InitializeComponent ();
            Title = "Novo usuário";
		}

        private async void btOk_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (Email.Text == null)
                {
                    await DisplayAlert("E-mail", "Favor informar o e-mail", "Informar");
                    Email.Focus();
                    return;
                }

                string email = Email.Text;
                Regex rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
                if (!rg.IsMatch(email))
                {
                    await DisplayAlert("E-mail", "E-mail informado é inválido!", "Corrigir");
                    Email.Focus();
                    return;
                }

                if (Nome.Text == null)
                {
                    await DisplayAlert("Nome", "Favor informar o seu Nome", "Informar");
                    Nome.Focus();
                    return;
                }

                if (Senha.Text == null)
                {
                    await DisplayAlert("Senha", "Favor informar a sua Senha", "Informar");
                    Senha.Focus();
                    return;
                }

                btOk.IsEnabled = false;
                indiCriando.IsVisible = true;

                Usuario user = new Usuario();
                var retorno = await user.CriarUsuario(new Models.UsuarioModel()
                {
                    DS_CPF = CPF.Text,
                    DS_EMAIL = Email.Text,
                    NM_NOME = Nome.Text,
                    DS_BAIRRO = Bairro.Text,
                    DS_ENDERECO = Endereco.Text,
                    DS_COMPLEMENTO = Complemento.Text,
                    DS_SENHA = Senha.Text,
                    DS_TELEFONE = Telefone.Text,
                    NR_NUMERO = Numero.Text,
                    BO_FACEBOOK = "F"
                },false);

                if (retorno == "T")
                {
                    await DisplayAlert("Já Existe", "O e-mail informado já consta em nossa lista de usuários", "Tentar realizar Login");
                    await Navigation.PopAsync(true);
                    return;
                }

                btOk.IsEnabled = true;

                await DisplayAlert("Sucesso", "Usuário cadastrado com sucesso!", "Login");
                await Navigation.PopAsync(true);
                indiCriando.IsVisible = false;

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Alguma coisa de errado aconteceu! Tente novamente!", "Que pena");
                btOk.IsEnabled = true;
                indiCriando.IsVisible = false;
            }
        }
    }
}