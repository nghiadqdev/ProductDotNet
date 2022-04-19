using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Eshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //if (HttpContext.Request.Cookies.ContainsKey("tenTK"))
            //{
            //    ViewBag.Name = HttpContext.Request.Cookies["hoTen"];
            //}
            //else
            //{
            //    ViewBag.Name = "Khách";
            //}

            if (HttpContext.Session.GetString("tenTK") != null)
            {
                ViewBag.Name = HttpContext.Session.GetString("hoTen");
            }
            else
            {
                ViewBag.Name = "Khách";
            }

            return View();
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
}