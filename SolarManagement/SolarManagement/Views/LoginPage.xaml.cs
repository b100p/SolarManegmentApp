using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using SolarManagement.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SolarManagement.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        Contact contact;
        
        public LoginPage()
        {

            InitializeComponent();
            BindingContext = contact = new Contact();
            email.Completed += (s,e)=> pass.Focus();
            pass.Completed += (s, e) => btnlog_Clicked(s, e);
        }
        async void btnlog_Clicked(object sender, EventArgs e)
        {
            string server = "77.75.95.65";
            string database = "tempdb";
            string uid = "fypuser";
            string password = "CCNE@2020";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            MySqlConnection cn = new MySqlConnection(connectionString);
            if (cn.State != ConnectionState.Open)
            {
                try
                {
                    cn.Open();
                }
                catch (MySqlException ex)
                {
                    await DisplayAlert("NetworkError", "Please check that the server is up and running.", "Done");
                    return;
                }
            }
            MySqlCommand cmd = new MySqlCommand("select * from users where email=@Email and password=@Password", cn);
            cmd.Parameters.AddWithValue("@Email", email.Text.ToLower().Trim());
            cmd.Parameters.AddWithValue("@Password", pass.Text.Trim());
            var res = cmd.ExecuteReader();
            var bindpage = new ItemsPage();

            if (res.HasRows)
            {
                res.Read();
                var contact = new Contact
                {
                    Name = res[2].ToString(),
                    Admin = bool.Parse(res[4].ToString())
                };
                bindpage.BindingContext = contact;
                await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");
                await Xamarin.Essentials.SecureStorage.SetAsync("User", res[2].ToString());
                Application.Current.MainPage = new AppShell();
                await Shell.Current.GoToAsync("//main");
            }
            else
                await DisplayAlert("Authentication failed", "There was an error logging in,please try again.", "Try again");


        }
    }
}