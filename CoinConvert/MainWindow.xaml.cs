using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoinConvert
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        public MainWindow()
        {
            InitializeComponent();               
            combo_d_source.Items.Add(new { Text = "Bitcoin", Value = "btc-bitcoin" });
            combo_d_source.Items.Add(new { Text = "Euro", Value = "eur-euro-token" });
            combo_d_source.Items.Add(new { Text = "Neurochain", Value = "ncc-neurochain" });
            combo_d_source.DisplayMemberPath = "Text";
            combo_d_source.SelectedValuePath = "Value"; 
            combo_d_source.SelectedIndex = 0;
            combo_d_destination.Items.Add(new { Text = "USD", Value = "usd-us-dollars" });
            combo_d_destination.Items.Add(new { Text = "Ethereum", Value = "eth-ethereum" });
            combo_d_destination.Items.Add(new { Text = "XRP", Value = "xrp-xrp" });
            combo_d_destination.DisplayMemberPath = "Text";
            combo_d_destination.SelectedValuePath = "Value";            
            combo_d_destination.SelectedIndex = 0;
            lab_result.Visibility = Visibility.Hidden;
        }
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool IsInternetAvailable()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int amount;
            if(int.TryParse(txt_montant.Text,out amount) && IsInternetAvailable())
            {

                var client = new RestClient("https://api.coinpaprika.com/v1/");
                string data = "price-converter?base_currency_id=@id_s&quote_currency_id=@id_d&amount=@montant";
                data = Regex.Replace(data, "@id_s", combo_d_source.SelectedValue.ToString());
                data = Regex.Replace(data, "@id_d", combo_d_destination.SelectedValue.ToString());
                data = Regex.Replace(data, "@montant", txt_montant.Text);
                var request = new RestRequest(data, Method.GET);
                request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
                var queryResult = client.Execute(request);
                dynamic magic = JsonConvert.DeserializeObject(queryResult.Content);
                lab_result.Content = magic["price"];
                lab_result.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Champ incorrect ou internet non disponible", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
        }
    }
}
