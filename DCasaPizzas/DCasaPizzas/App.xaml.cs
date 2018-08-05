using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DCasaPizzas
{
	public partial class App : Application
	{
        internal static string sdsNome;
        internal static string sdsUriImg;
        internal static string sdsEmail;
        internal static long IdUsuario;

        public App ()
		{
			InitializeComponent();

            //MainPage = new DCasaPizzas.MainPage();
            MainPage = new NavigationPage(new DCasaPizzas.MainPage());
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
