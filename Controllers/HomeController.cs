using Final.Data;
using Final.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace Final.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MiContexto _context;

        public HomeController(ILogger<HomeController> logger, MiContexto contexto)
        {
            _context = contexto;
            _logger = logger;
        }
        public IActionResult Index()
        {
            var usuario = _context.usuarios.Where(u => u.dni == HttpContext.Session.GetInt32("UserDni") && u.password == HttpContext.Session.GetString("UserPass")).FirstOrDefault();
            if (usuario == null)
            {

            }
            return View();
        }
        [HttpPost]
        public IActionResult Index(int Dni, string password)
        {
            try
            {
                var usuario = _context.usuarios.Where(u => u.dni == Dni && u.password == password).FirstOrDefault();
                if (usuario == null)
                {
                    return View();
                }


                HttpContext.Session.SetInt32("UserDni", Dni);
                HttpContext.Session.SetString("UserPass", password);
                return RedirectToAction("Index", "Usuarios");
            }
            catch
            {

            }
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}