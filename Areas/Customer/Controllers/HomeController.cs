using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;


[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    public String AhmedElqassa() {
        Console.WriteLine("This me in the ahmed elqasas page ");
        Console.WriteLine("We are here in the ahmedelqasas controller");
        return "hello qasas page edited";
    }
    public IActionResult  Get1(int id) {
        Console.WriteLine("The id is {0}", id);
        return Content("Hello sayed elqassa");
        // return "sated osam";
    }
    public IActionResult Privacy()
    {
        return View();
    }

    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
