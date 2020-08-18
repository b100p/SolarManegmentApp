using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SolarManagement.Models;
using SolarManagement.Views;
using SolarManagement.ViewModels;
using MySqlConnector;
using Microcharts;
using SkiaSharp;
using System.Data;
using System.Reflection;

[assembly: ExportFont("Fonts/RedRose-Bold.ttf")]
namespace SolarManagement.Views
{
    public partial class ItemsPage : ContentPage
    {
        private string server = "77.75.95.65";
        private string database = "tempdb";
        private string uid = "fypuser";
        private string password = "CCNE@2020";

        Contact contact;
        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = contact = new Contact();
            List<int> years = Enumerable.Range(2000, 20).ToList();
            List<string> yearst = years.ConvertAll(delegate (int i) { return i.ToString(); });
            yearpicker.ItemsSource = yearst;
            List<string> monthst = new List<string>();
            string[] month = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
            monthst.AddRange(month);
            monthpicker.ItemsSource = monthst;
            monthpicker.SelectedItem = "01";
            yearpicker.SelectedItem = "2006";
            //var tapGestureRecognizer = new TapGestureRecognizer();
            //tapGestureRecognizer.Tapped += (s, e) => {
            //    DisplayActionSheet("Preview in browser", "Cancel", null, TimePicker1. ,"Button2","Button3","Button4","Button5" );
            //};
            //Chart1.GestureRecognizers.Add(tapGestureRecognizer);

        }
        private void btnview_Clicked(object sender, EventArgs e)
        {
            if (monthpicker.SelectedItem.ToString()!="" || yearpicker.SelectedItem.ToString() != "")
            {
                string connectionstring = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
                MySqlConnection cn = new MySqlConnection(connectionstring);
                if (cn.State != ConnectionState.Open)
                {
                    try
                    {
                        cn.Open();
                    }
                    catch (MySqlException ex)
                    {
                        DisplayAlert("NetworkError", "Please check that the server is up and running.", "Done");
                        return;
                    }
                }
                string dat = monthpicker.SelectedItem.ToString() + "/" + yearpicker.SelectedItem.ToString();
                DateTime dt = DateTime.Parse(dat);
                string semidate = dt.ToString("MM/yy");
                string finaldate = semidate.Insert(3, "%/");
                List<ChartEntry> entries = new List<ChartEntry>();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM tempdb.solarpanel where Local_Time like '%" + finaldate + "%';", cn);
                var res = cmd.ExecuteReader();
                
                    DateTime date;
                    double power = 0;
                    DateTime temp = new DateTime(2006, 1, 1);
                    double i = 0;
                    int j = 0;
                    double avg = 1;
                    while (res.Read())
                    {
                    if (!res.IsDBNull(0))
                    {
                        i++;
                        date = DateTime.Parse(res[0].ToString());
                        power = Convert.ToDouble(res[1].ToString());
                        if (date.Day == temp.Day)
                        {
                            avg += power;
                        }
                        else
                        {
                            var random = new Random();
                            string[] col = { "#2c3e50", "#77d065", "#b455b6", "#3498db", "#3498db" };
                            var color = col[random.Next(4)];
                            ChartEntry ch = new ChartEntry(float.Parse((avg / i).ToString()))
                            {
                                Label = temp.Day.ToString(),
                                ValueLabel = (avg / i).ToString("#.##"),
                                Color = SKColor.Parse(color)
                            };
                            entries.Add(ch);
                            j++;
                            avg = 0;
                            i = 0;
                            avg += power;
                            temp = temp.AddDays(1);
                        }
                    }
                    else
                    {
                        DisplayAlert("Error", "This specific Month doesn't contain any entry", "Tryagain");
                    }
                    }
                    Chart2.Chart = new LineChart { Entries = entries };

            }
            else
            {
                DisplayAlert("No Value Selected", "Please choose values for both year and month", "Tryagain");
            }

        }

        private async void  DatePicker_change(object sender, DateChangedEventArgs e)
        {
            DateTime date1 = TimePicker1.Date;
            //await DisplayAlert("Test", date1.ToString("MM/dd/yy"), "Done");
            string connectionstring = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            MySqlConnection cn = new MySqlConnection(connectionstring);
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
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM tempdb.solarpanel where Local_Time like '%" + date1.ToString("MM/dd/yy") + "%';", cn);
            MySqlDataReader rd = cmd.ExecuteReader();
            List<ChartEntry> entries = new List<ChartEntry>();
            while (rd.Read())
            {
                var random = new Random();
                string[] col = { "#2c3e50", "#77d065", "#b455b6", "#3498db", "#3498db" };
                var color = col[random.Next(4)];
                ChartEntry ch = new ChartEntry(float.Parse(rd[1].ToString()))
                {
                    Color = SKColor.Parse(color)
                };
                entries.Add(ch);
            }
            Chart1.Chart = new LineChart { Entries = entries };
        }

        private async void btnadd_Clicked(object sender, EventArgs e)
        {
            
            await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "0");
            Application.Current.MainPage = new AppShell();
            await Shell.Current.GoToAsync("//login");
        }
    }
}