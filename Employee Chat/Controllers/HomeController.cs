using System.Diagnostics;
using Employee_Chat.Models;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Chat.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
