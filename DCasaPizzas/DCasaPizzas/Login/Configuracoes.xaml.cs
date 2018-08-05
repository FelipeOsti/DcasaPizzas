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
	public partial class Configuracoes : ContentPage
	{
        Logic.Usuario user = null;
        Models.UsuarioModel dados = null;
        public Configuracoes ()
		{
			InitializeComponent ();

            CarregarUsuario();
		}

        private async void CarregarUsuario()
        {
            user = new Logic.Usuario();
            dados = await user.GetDadosUsuario(App.sdsEmail);

            Email.Text = dados.DS_EMAIL;
            Email.IsEnabled = false;
            Nome.Text = dados.NM_NOME;
            CPF.Text = dados.DS_CPF;

            Senha.Text = "";

            Telefone.Text = dados.DS_TELEFONE;
            Endereco.Text = dados.DS_ENDERECO;
            Numero.Text = dados.NR_NUMERO;
            Bairro.Text = dados.DS_BAIRRO;
            Complemento.Text = dados.DS_COMPLEMENTO;

            Senha.IsEnabled = true;
            NovaSenha.IsEnabled = true;

            if (dados.BO_FACEBOOK.Replace("\"","") == "T")
            {
                Senha.IsEnabled = false;
                NovaSenha.IsEnabled = false;
            }
        }

        private async void btOk_Clicked(object sender, EventArgs e)
        {
            try
            {

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

                string bboFace = "T";
                if (Senha.IsEnabled) bboFace = "F";

                Usuario user = new Usuario();
                var usuario = new Models.UsuarioModel()
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
                    BO_FACEBOOK = bboFace
                };

                if (bboFace == "F")
                {
                    if (await user.VerificaUsuarioSenha(usuario))
                    {
                        await DisplayAlert("Alerta", "E-mail e senha informados não conferem! Favor verifique!", "OK");
                        Senha.Text = "";
                        return;
                    }
                }
                if(NovaSenha.Text != "") usuario.DS_SENHA = NovaSenha.Text;

                btOk.IsEnabled = false;
                indiCriando.IsVisible = true;

                var retorno = await user.CriarUsuario(usuario,true);

                btOk.IsEnabled = true;

                await DisplayAlert("Sucesso", "Usuário atualizado com sucesso!", "OK");
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