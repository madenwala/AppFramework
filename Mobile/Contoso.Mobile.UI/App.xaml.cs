﻿using Contoso.Mobile.UI.Services;
using Contoso.Mobile.UI.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Contoso.Mobile.UI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            DependencyService.Register<NavigationService>();

            MainPage = new ShellView();


            //If i dont have a user navigate to Login
            Shell.Current.GoToAsync("//LoginView");
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
