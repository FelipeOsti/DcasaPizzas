using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DCasaPizzas;
using DCasaPizzas.Logic;
using DCasaPizzas.Models;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCasaPizzas.Login
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FacebookLogin : ContentPage
	{
        string ClientID = "517987311872645";
        public string accesToken = "";
        public string FacebookProfile = "";
        WebView webView;

        public FacebookLogin ()
		{
			InitializeComponent();
            Title = "Login com Facebook";
            Login();
		}

        private void Login()
        {
            string apiRequest = "https://www.facebook.com/v2.12/dialog/oauth?client_id=" + ClientID +
            "&display=popup&response_type=token&redirect_uri=https://www.facebook.com/connect/login_success.html&scope=email";

            webView = new WebView {
               VerticalOptions = LayoutOptions.FillAndExpand,
               HorizontalOptions = LayoutOptions.FillAndExpand,
               Source = apiRequest
            };
            stackTela.Children.Add(webView);
            //Content = webView;
            webView.Navigated += WebView_Navigated;
            webView.IsVisible = false;
        }

        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            accesToken = ExtractAccessToken(e.Url);
            if (accesToken != "")
            {
                if(Application.Current.Properties.ContainsKey("tokenFace"))
                {
                    Application.Current.Properties["tokenFace"] = accesToken;
                }
                else
                {
                    Application.Current.Properties.Add("tokenFace", accesToken);
                }

                webView.IsVisible = false;
                stackLoading.IsVisible = true;
                await GetDadosFace();
                MainPage.AbrirMenu();
                stackLoading.IsVisible = false;
            }
            else
            {
                stackLoading.IsVisible = false;
                webView.IsVisible = true;
            }
        }

        public async Task GetDadosFace()
        {
            try
            {
                Facebook infoFace = new Facebook();
                FacebookProfile = await GetFacebookProfile(accesToken);
                if (FacebookProfile != "")
                {
                    infoFace = JsonConvert.DeserializeObject<Facebook>(FacebookProfile);

                    App.sdsNome = infoFace.Name;
                    App.sdsUriImg = infoFace.Picture.Data.Url;
                    App.sdsEmail = infoFace.Email;

                }
                Usuario user = new Usuario();
                await user.CriarUsuario(new UsuarioModel() { DS_EMAIL = infoFace.Email, NM_NOME = infoFace.Name,BO_FACEBOOK = "T" },false);
                App.IdUsuario = await user.GetIDUsuario(infoFace.Email);
            }
            catch(Exception e)
            {
                await DisplayAlert("Falha", "Falha ao realizar login com Facebook"+e.Message, "Tentar Novamente");
            }
        }

        public async Task<string> GetFacebookProfile(string accesToken)
        {
            try
            {
                var requestUrl = "https://graph.facebook.com/v2.12/me/" +
                    "?fields=name,picture,cover,age_range,devices,email,gender,is_verified" +
                    "&access_token=" + accesToken;

                var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(requestUrl);

                return response;
            }
            catch ( Exception e)
            {
                throw;
            }
        }

        private string ExtractAccessToken(string url)
        {
            if(url.Contains("access_token") && url.Contains("&expires_in="))
            {
                var at = url.Replace("https://www.facebook.com/connect/login_success.html#access_token=", "");
                at = at.Replace("https://web.facebook.com/connect/login_success.html?_rdc=1&_rdr#access_token=", "");
                //if(Device.RuntimePlatform == Device.WinPhone || Device.RuntimePlatform == Device.WinRT)
               // {
               //     at = at.Replace("http://www.facebook.com/connect/login_success.html#access_token=", "");
               // }

                var accessToken = at.Remove(at.IndexOf("&expires_in="));
                return accessToken;
            }

            return string.Empty;
        }
    }
}