using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json;

namespace Currency_Converter
{
    public partial class MainWindow : Window
    {
        Root val = new Root();
        public class Root
        {
            public Rate rates { get; set; }
            public long timestamp;
            public string license;
        }

        public class Rate
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
            public double BGN { get; set; }
            public double CHF { get; set; }
            public double KRW { get; set; }
            public double TRY { get; set; }
            public double XAG { get; set; }
            public double XAU { get; set; }
            public double XPT { get; set; }
            public double XPD { get; set; }
            public double HUF { get; set; }

        }

        public static async Task<Root> GetDataGetMethod<T>(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(1);

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var ResponseString = await response.Content.ReadAsStringAsync();
                        var ResponseObject = JsonConvert.DeserializeObject<Root>(ResponseString);

                        //MessageBox.Show("TimeStamp: " + ResponseObject.timestamp, "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        return ResponseObject;  //Return API response
                    }
                    else
                    {
                        MessageBox.Show("Failed to fetch data from API", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;  //Return null if API call fails
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while fetching data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;  //Return null if an exception occurs
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            GetValue();
            ClearControls();
        }


        private async void GetValue()
        {
            val = await GetDataGetMethod<Root>("https://openexchangerates.org/api/latest.json?app_id=3d184aaef6b244bc9d95c830cfdba40c"); //API Link
            if (val == null)
            {
                MessageBox.Show("Failed to fetch data from API. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                BindCurrency();
            }
        }
        private void BindCurrency()
        {

            if (val == null)
            {
                GetValue(); 
                return; 
            }

            DataTable dtCurrency = new DataTable();
            dtCurrency.Columns.Add("Text");

            //value column in DataTable
            dtCurrency.Columns.Add("Rate");

            //rows in Datatable with text and value
            dtCurrency.Rows.Add("--SELECT--", 0);
            dtCurrency.Rows.Add("INR", val.rates.INR);
            dtCurrency.Rows.Add("JPY", val.rates.JPY);
            dtCurrency.Rows.Add("USD", val.rates.USD);
            dtCurrency.Rows.Add("NZD", val.rates.NZD);
            dtCurrency.Rows.Add("EUR", val.rates.EUR);
            dtCurrency.Rows.Add("CAD", val.rates.CAD);
            dtCurrency.Rows.Add("ISK", val.rates.ISK);
            dtCurrency.Rows.Add("PHP", val.rates.PHP);
            dtCurrency.Rows.Add("DKK", val.rates.DKK);
            dtCurrency.Rows.Add("CZK", val.rates.CZK);
            dtCurrency.Rows.Add("BGN", val.rates.BGN);
            dtCurrency.Rows.Add("CHF", val.rates.CHF);
            dtCurrency.Rows.Add("KRW", val.rates.KRW);
            dtCurrency.Rows.Add("TRY", val.rates.TRY);
            dtCurrency.Rows.Add("XAG", val.rates.XAG);
            dtCurrency.Rows.Add("XAU", val.rates.XAU);
            dtCurrency.Rows.Add("XPT", val.rates.XPT);
            dtCurrency.Rows.Add("XPD", val.rates.XPD);
            dtCurrency.Rows.Add("HUF", val.rates.HUF);
            
            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbFromCurrency.DisplayMemberPath = "Text";
            cmbFromCurrency.SelectedValuePath = "Rate";
            cmbFromCurrency.SelectedIndex = 0;

            
            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Rate";
            cmbToCurrency.SelectedIndex = 0;

        }
        private void ClearControls()
        {
            currencyTxt.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            currencyTxt.Focus();
        }
        //Allow only the integer value in TextBox
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regular Expression
            Regex regex = new Regex(@"^\d+$");
            e.Handled = !regex.IsMatch(e.Text);
        }
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;

            //Check amount textbox is Null or Blank
            if (currencyTxt.Text == null || currencyTxt.Text.Trim() == "")
            { 
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //After clicking on message box OK sets the Focus on amount textbox
                currencyTxt.Focus();
                return;
            }
            //Else if the currency from is not selected or it is default text --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on From Combobox
                cmbFromCurrency.Focus();
                return;
            }
            //Else if Currency To is not Selected or Select Default Text --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on To Combobox
                cmbToCurrency.Focus();
                return;
            }
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                //The amount textbox value set in ConvertedValue.
                //double.parse is used to convert datatype String To Double.
                //Textbox text have string and ConvertedValue is double datatype
                ConvertedValue = double.Parse(currencyTxt.Text);

                // added ToString("N3") to place 000 after the(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {

                //Calculation for currency converter is From Currency value multiply(*) 
                // with amount textbox value and then the total is divided(/) with To Currency value
                ConvertedValue = (double.Parse(cmbToCurrency.SelectedValue.ToString()) * double.Parse(currencyTxt.Text)) / double.Parse(cmbFromCurrency.SelectedValue.ToString());

                //Show in label converted currency and converted currency name.
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }

        }
        //Clear button click event
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //ClearControls method  is used to clear all control value
            ClearControls();
        }


    }
}
