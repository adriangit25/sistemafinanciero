using Microsoft.AspNetCore.Mvc;

namespace SF.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Puedes obtener el nombre de usuario desde la sesión si lo guardas ahí
            ViewBag.Usuario = HttpContext.Session.GetString("usuario") ?? "Usuario";
            return View();
        }
    }
}
