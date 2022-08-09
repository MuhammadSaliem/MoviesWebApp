using Microsoft.AspNetCore.Mvc;

namespace MoviesWebApp.Controllers
{
    public class MoviesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
