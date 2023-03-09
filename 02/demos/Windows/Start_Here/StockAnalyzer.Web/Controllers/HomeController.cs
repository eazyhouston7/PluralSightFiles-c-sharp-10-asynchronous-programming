using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockAnalyzer.Core.Domain;
using StockAnalyzer.Web.Models;
using System.Diagnostics;

namespace StockAnalyzer.Web.Controllers;

public class HomeController : Controller
{
    private static string API_URL = "https://ps-async.fekberg.com/api/stocks";

    public async Task<IActionResult> Index()
    {
        using (var client = new HttpClient())
        {
            var responseTask = client.GetAsync($"{API_URL}/MSFT");

            var response = await responseTask;

            //var badHabbit = responseTask.Result;  // Try not to do, because will block the thread until the result is avaliable.
            //var badHabbit2 = response.Content.ReadAsStringAsync().Result; // Not good beacause this is running synchronously. (.Result is not returned until the entire content is read.)

            var content = await response.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<IEnumerable<StockPrice>>(content);
            return View(data);
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}