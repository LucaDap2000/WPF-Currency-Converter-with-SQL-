using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Static_Currency_Converter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Root val = new Root();

        public class Root // Root Class is a Main Class. API returns rates with currency name and value
        {
            public Rate rates { get; set; } // Get all records in rates and Set in Rate class as Currency name
            public long timestamp;
            public string license;
        }

        public class Rate // Make sure API return Value Names and where you want to store that name are the same. 
        {
            public double INR { get; set; }
            public double JPY { get; set; }
            public double USD { get; set; }
            public double NZD { get; set; }
            public double EUR { get; set; }
            public double CAD { get; set; }
            public double ISK { get; set; }
            public double PHP { get; set; }
            public double DKK { get; set; }
            public double CZK { get; set; }
            public double GBP { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            GetValue();
        }

        private async void GetValue()
        {
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=c4e2b3d1f78a4142a83f45dbbfb55aca"); // App ID
            BindCurrency();
        }

        public static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();
            try
            {
                using (var client = new HttpClient()) // HttpClient class provides a base class for sending/receiving the Http requests/responses from a URL
                {
                    client.Timeout = TimeSpan.FromMinutes(1); // Wait for 1 minute before timeout
                    HttpResponseMessage response = await client.GetAsync(url); // HttpResponseMessage is a way of returning a message/data from your action
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) // Check API response status code OK
                    {
                        var ResponseString = await response.Content.ReadAsStringAsync(); // Serialize the Http content to a string as an asynchronous operation
                        var ResponseObject = JsonConvert.DeserializeObject<Root>(ResponseString); // JsonConvert.DeserializeObject to deserialize Json to C#

                        // MessageBox.Show("Rates: " + ResponseString, "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        return ResponseObject; // Return API response
                    }

                    return myRoot;
                }
            }
            catch
            {
                return myRoot;
            }
        }

        private void BindCurrency()
        {
            DataTable dtCurrency = new DataTable();
            dtCurrency.Columns.Add("Text");
            dtCurrency.Columns.Add("Value");

            // Add rows in the DataTable with text and value
            dtCurrency.Rows.Add("--SELECT--", 0);
            dtCurrency.Rows.Add("INR", val.rates.INR);
            dtCurrency.Rows.Add("USD", val.rates.USD);
            dtCurrency.Rows.Add("NZD", val.rates.NZD);
            dtCurrency.Rows.Add("JPY", val.rates.JPY);
            dtCurrency.Rows.Add("EUR", val.rates.EUR);
            dtCurrency.Rows.Add("CAD", val.rates.CAD);
            dtCurrency.Rows.Add("ISK", val.rates.ISK);
            dtCurrency.Rows.Add("PHP", val.rates.PHP);
            dtCurrency.Rows.Add("DKK", val.rates.DKK);
            dtCurrency.Rows.Add("CZK", val.rates.CZK);
            dtCurrency.Rows.Add("GBP", val.rates.GBP);

            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbFromCurrency.DisplayMemberPath = "Text";
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbFromCurrency.SelectedIndex = 0;

            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = 0;
        }

        private void Convert_Click(object sender, EventArgs e)
        {
            // Create the variable as ConvertedValue with double datatype to store currency converted value
            double convertedValue;

            // Check if the amount TextBox is Null or Empty
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                // If amount TextBox is Null or Empty it will show this message box
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                // After clicking on MessageBox OK set focus on amount TextBox
                txtCurrency.Focus();
                return;
            }
            // Else if currency From is not selected or select default text --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                // Show the message
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                // Set focus on the From ComboBox
                cmbFromCurrency.Focus();
                return;
            }
            // Else if currency To is not selected or select default text --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                // Show the message
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                // Set focus on the To ComboBox
                cmbToCurrency.Focus();
                return;
            }

            // Check if From and To ComboBox selected values are the same
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                // Amount TextBox value set in convertedValue
                // Double.Parse is used for converting the datatype String to Double
                // TextBox text have string and convertedValue is Double datatype
                convertedValue = double.Parse(txtCurrency.Text);
                // Show the label converted currency and converted currency name and ToString("N3") is used to place 000 after the dot(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + convertedValue.ToString("N3");
            }
            else
            {
                // Calculation for currency converter is From Currency value multiply(*)
                // With the amount TextBox value and then that total divided(/) with To Currency value
                convertedValue = (double.Parse(cmbToCurrency.SelectedValue.ToString()) *
                                  double.Parse(txtCurrency.Text)) / 
                                  double.Parse(cmbFromCurrency.SelectedValue.ToString());

                // Show the label converted currency and converted currency name
                lblCurrency.Content = cmbToCurrency.Text + " " + convertedValue.ToString("N3");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
            {
                cmbFromCurrency.SelectedIndex = 0;
            }
            if (cmbToCurrency.Items.Count > 0)
            {
                cmbToCurrency.SelectedIndex = 0;
            }
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+[,]+[.]");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}