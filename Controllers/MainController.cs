using Final.Data;
using Final.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final.Controllers
{
    public class MainController : Controller
    {
        private readonly MiContexto _context;
        private Usuario uLogeado;

        public MainController(MiContexto context)
        {
            _context = context;
            _context.usuarios
                    .Include(u => u.tarjetas)
                    .Include(u => u.cajas)
                    .Include(u => u.pf)
                    .Include(u => u.pagos)
                    .Load();
            _context.cajas
                .Include(c => c.movimientos)
                .Include(c => c.titulares)
                .Load();
            _context.tarjetas.Load();
            _context.pagos.Load();
            _context.movimientos.Load();
            _context.plazosFijos.Load();
        }
        public Usuario usuarioLogeado() //tomar sesion del usuario
        {
            if (HttpContext != null)
            {
                return _context.usuarios.Where(u => u.id == HttpContext.Session.GetInt32("UserId")).FirstOrDefault();
            }
            return null;
        }
        // GET: MainController
        public ActionResult Index()
        {
            uLogeado = usuarioLogeado();
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (uLogeado.isAdmin)
            {
                ViewBag.Admin = "True";
            }
            return View();
        }
    }
}
