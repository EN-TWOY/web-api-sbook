using Microsoft.AspNetCore.Mvc;

namespace ProyectoSBooks.Controllers
{
    public class InicioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
