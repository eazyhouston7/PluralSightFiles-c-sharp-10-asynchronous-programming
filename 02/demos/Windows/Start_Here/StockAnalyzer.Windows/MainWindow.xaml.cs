using Newtonsoft.Json;
using StockAnalyzer.Core.Domain;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;

namespace StockAnalyzer.Windows;

public partial class MainWindow : Window
{
    private static string API_URL = "https://ps-async.fekberg.com/api/stocks";
    private Stopwatch stopwatch = new Stopwatch();

    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// - Calling Result or Wait() may cause deadlocks
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    private async void Search_Click(object sender, RoutedEventArgs e)
    {
        BeforeLoadingStockData();

        using (var client = new HttpClient())
        {
            var responseTask = client.GetAsync($"{API_URL}/{StockIdentifier.Text}");

            var response = await responseTask;

            //var badHabbit = responseTask.Result;  // Try not to do, because will block the thread until the result is avaliable.
            //var badHabbit2 = response.Content.ReadAsStringAsync().Result; // Not good beacause this is running synchronously. (Result is not returned until the entire content is read.)

            var content = await response.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<IEnumerable<StockPrice>>(content);

            Stocks.ItemsSource = data;
        }

        AfterLoadingStockData();
    }








    private void BeforeLoadingStockData()
    {
        stopwatch.Restart();
        StockProgress.Visibility = Visibility.Visible;
        StockProgress.IsIndeterminate = true;
    }

    private void AfterLoadingStockData()
    {
        StocksStatus.Text = $"Loaded stocks for {StockIdentifier.Text} in {stopwatch.ElapsedMilliseconds}ms";
        StockProgress.Visibility = Visibility.Hidden;
    }

    private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true });

        e.Handled = true;
    }

    private void Close_OnClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}