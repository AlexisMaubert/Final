using Final.Data;
using Final.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final.Controllers
{
    public class RegistroController : Controller
    {
        private readonly MiContexto _context;
        private Usuario uLogeado;

        public RegistroController(MiContexto context)
        { //Relaciones del context
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
        // GET: RegistroController
        public ActionResult Index()
        {
            uLogeado = usuarioLogeado();
            if (uLogeado != null)
            {
                return RedirectToAction("Index", "Main");
            }
            ViewBag.logeado = "no";
            return View();
        }
        [HttpPost]
        public ActionResult Index([Bind("id,dni,nombre,apellido,mail,password")] Usuario usuario)
        {
            uLogeado = usuarioLogeado();
            if (uLogeado != null)
            {
                return RedirectToAction("Index", "Main");
            }
            ViewBag.logeado = "no";
            if (_context.usuarios.Any(us => us.dni == usuario.dni))
            {
                ViewBag.error = "Ya existe un usuario con este DNI";
                return View();
            }
            if (ModelState.IsValid)
            {
                usuario.bloqueado = false;
                usuario.isAdmin = false;
                usuario.intentosFallidos = 0;
                _context.usuarios.Add(usuario);
                _context.SaveChanges();
                return RedirectToAction("Index", "Login");
            }
            return View();

        }
    }
}
