using Newtonsoft.Json;
using StockAnalyzer.Core;
using StockAnalyzer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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

    /// <summary>   (Additonal helpful notes/points)
    /// - Calling .Result or .Wait() may cause deadlocks
    /// - Avoid async/void at all costs in method signature!
    ///     - This can block an async operation.
    ///     - *Only use async/void for event handlers*
    /// - Methods marked as 'async Task' will automatically have a Task returned from them without explicitly having to return anything.
    /// - Code before and after the 'await' keyword is executed on the calling thread.
    /// - Always await your async operations at some point in the chain.
    /// - Removing await from async operations means that there is no longer a continuation that will execute and the operation is not validated.
    /// - Exceptions that occur in an async/void method cannot be caught.
    /// - Try to minimize the code in 'async/void' methods (event handlers) and make sure that they ALWAYS wrap their own code in a try/catch.
    /// - Always return a Task from a async method.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    private async void Search_Click(object sender, RoutedEventArgs e)
    {
        BeforeLoadingStockData();

        try
        {
            await GetStocks();
        }
        catch (Exception ex) {
            Notes.Text = ex.Message;
        }
        // Line/Code below will run when responseTask has completed.
        AfterLoadingStockData();
        #region "old"
        /*  // Before
        BeforeLoadingStockData();
        using (var client = new HttpClient())
        {
            var responseTask = client.GetAsync($"{API_URL}/{StockIdentifier.Text}");

            var response = await responseTask;

            //var badHabbit = responseTask.Result;  // Try not to do, because will block the thread until the result is avaliable.
            //var badHabbit2 = response.Content.ReadAsStringAsync().Result; // Not good beacause this is running synchronously. (.Result is not returned until the entire content is read.)

            var content = await response.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<IEnumerable<StockPrice>>(content);

            Stocks.ItemsSource = data;
        }
        AfterLoadingStockData();
        */
        #endregion
    }

    private async Task GetStocks()
    {
        try {
            var store = new DataStore();

            var responseTask = store.GetStockPrices(StockIdentifier.Text);

            Stocks.ItemsSource = await responseTask;
        }
        catch (Exception ex)
        {
            throw;
        }
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