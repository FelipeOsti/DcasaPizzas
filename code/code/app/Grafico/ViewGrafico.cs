using AppRomagnole.Logic;
using AppRomagnole.Menu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppRomagnole.Grafico
{
    public class ViewGrafico : ContentPage
    {
        private bool bboDetalhe = false;
        List<GraficoURL> lstGraficos;
        public int height = 0;

        public async Task<StackLayout> CriaGrafico(BarraChart chart, double id, List<GraficoURL> _lstGraficos)
        {
            lstGraficos = _lstGraficos;
            bboDetalhe = false;
            if (lstGraficos != null)
                if (lstGraficos.Count > 0) bboDetalhe = true;

            StackLayout stack = new StackLayout() { Margin = new Thickness(0, 0, 0, 5) };

            Frame frame = new Frame() { CornerRadius = 15 };
            StackLayout stkFrame = new StackLayout();
            StackLayout stkTopo = await GetStackTopo(id, chart.sdsTitulo);

            int heigth = App.ScreenHeight;
            if (App.ScreenWidth > heigth) heigth = App.ScreenWidth;

            stkFrame.HeightRequest = heigth - 150;
            stkFrame.Children.Add(stkTopo);
            stkFrame.Children.Add(chart);
            stkFrame.Children.Add(new Label() { Text = Convert.ToString(id), IsVisible = false });
            stkFrame.VerticalOptions = LayoutOptions.Fill;

            frame.Content = stkFrame;
            stack.Children.Add(frame);

            return stack;
        }

        public async Task<StackLayout> CriaGrafico(LinhaChart chart, double id, List<GraficoURL> _lstGraficos)
        {
            lstGraficos = _lstGraficos;
            bboDetalhe = false;
            if (lstGraficos != null)
                if (lstGraficos.Count > 0) bboDetalhe = true;
            StackLayout stack = new StackLayout() { Margin = new Thickness(0, 0, 0, 5) };

            Frame frame = new Frame() { CornerRadius = 15 };
            StackLayout stkFrame = new StackLayout();
            StackLayout stkTopo = await GetStackTopo(id, chart.sdsTitulo);

            height = App.ScreenHeight;
            if (App.ScreenWidth > height) height = App.ScreenWidth;
            height = height - 150;
            bool bboTopo = true;

            if (this.Parent != null)
            {
                if (this.Parent.ToString().Contains("AppRomagnole.Forms.CompararPedidos.Compara"))
                {
                    height = height * 60 / 100; //60%
                    bboTopo = false;
                };
            };
            
            chart.height = height;
            stkFrame.HeightRequest = height - 50;
            if(bboTopo) stkFrame.Children.Add(stkTopo);
            stkFrame.Children.Add(chart);
            stkFrame.Children.Add(new Label() { Text = Convert.ToString(id), IsVisible = false });
            stkFrame.VerticalOptions = LayoutOptions.Fill;

            frame.Content = stkFrame;
            stack.Children.Add(frame);

            return stack;
        }

        public async Task<StackLayout> CriaGrafico(PizzaChart chart, double id, List<GraficoURL> _lstGraficos)
        {
            lstGraficos = _lstGraficos;
            bboDetalhe = false;
            if(lstGraficos != null)
                if (lstGraficos.Count > 0) bboDetalhe = true;
            StackLayout stack = new StackLayout() { Margin = new Thickness(0, 0, 0, 5) };

            Frame frame = new Frame() { CornerRadius = 15 };
            StackLayout stkFrame = new StackLayout();
            StackLayout stkTopo = await GetStackTopo(id,chart.Title);
            //stkTopo.Parent = stkFrame;

            int heigth = App.ScreenHeight;
            if (App.ScreenWidth > heigth) heigth = App.ScreenWidth;

            stkFrame.HeightRequest = heigth - 150;
            stkFrame.Children.Add(stkTopo);
            stkFrame.Children.Add(chart);
            stkFrame.Children.Add(new Label() { Text = Convert.ToString(id), IsVisible = false });
            stkFrame.VerticalOptions = LayoutOptions.Fill;

            frame.Content = stkFrame;
            stack.Children.Add(frame);

            return stack;
        }

        private async Task<StackLayout> GetStackTopo(double id, string Title)
        {
            StackLayout stkTopo = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromRgb(200, 200, 200),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(-20, -20, -20, 0)
            };

            Label lblTitle = new Label()
            {
                Text = Title,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                HorizontalTextAlignment = TextAlignment.Center
            };
            if (!bboDetalhe) lblTitle.Margin = new Thickness(0, 0, 10, 0);

            string caminhoImg = "favorito_white.png";
            MenuController menuC = new MenuController();
            if (await menuC.VerificaGraficoFavorito(id)) caminhoImg = "favorito.png";

            Button btFavorito = new Button()
            {
                BackgroundColor = Color.Transparent,
                WidthRequest=80,
                Text = "",
                HorizontalOptions = LayoutOptions.Start,
                Image = caminhoImg
            };
            btFavorito.Clicked += BtFavorito_Clicked;

            Button btnDetalhe = new Button()
            {
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
                BorderColor = Color.White,
                BorderWidth = 1,
                CornerRadius = 15,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                Text = "Detalhar",
                WidthRequest = 100                
            };
            if (Device.RuntimePlatform == Device.iOS) btnDetalhe.Margin = new Thickness(0, 5, 5, 5);
            btnDetalhe.IsVisible = bboDetalhe;
            btnDetalhe.Clicked += BtnDetalhe_Clicked;

            if (id > 0) stkTopo.Children.Add(btFavorito);
            stkTopo.Children.Add(lblTitle);
            stkTopo.Children.Add(btnDetalhe);

            return stkTopo;
        }

        private async void BtnDetalhe_Clicked(object sender, EventArgs e)
        {            
            await Graf.Current.AbrirGraficoSegundoNivel(lstGraficos);
        }

        private async void BtFavorito_Clicked(object sender, EventArgs e)
        {
            Button botao = (Button)sender;
            bool bboFav = false;

            if (botao.Image == "favorito_white.png")
            {
                botao.Image = "favorito.png";
                bboFav = true;
            }
            else
            {
                botao.Image = "favorito_white.png";
                bboFav = false;
            }

            await botao.ScaleTo(1.3, 200, Easing.Linear);
            await botao.ScaleTo(1, 200, Easing.Linear);

            var stk = (StackLayout)botao.Parent;
            stk = (StackLayout)stk.Parent;
            foreach (View view in stk.Children)
            {
                if (view.GetType() == typeof(Label))
                {
                    Label label = (Label)view;
                    MenuController menu = new MenuController();
                    await menu.FavoritarGrafico(double.Parse(label.Text), bboFav);
                }
            }

            if (!bboFav)
            {
                Frame frm = (Frame)stk.Parent;
                StackLayout stkFrame = (StackLayout)frm.Parent;
                StackLayout stkContent = (StackLayout)stkFrame.Parent;
                ScrollView scrlView = (ScrollView)stkContent.Parent;
                ContentPage content = (ContentPage)scrlView.Parent;

                if (content.GetType() == typeof(HomePage))
                {
                    await frm.FadeTo(0, 1000);
                    stkFrame.Children.Remove(frm);
                }
            }
        }
    }
}
