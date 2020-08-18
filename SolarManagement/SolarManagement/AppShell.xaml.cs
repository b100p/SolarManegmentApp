using System;
using System.Collections.Generic;
using SolarManagement.ViewModels;
using SolarManagement.Views;
using Xamarin.Forms;

namespace SolarManagement
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Setting), typeof(Setting));
            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
           // Shell.Current.GoToAsync("//LoginPage");
        }

    }
}
