using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final.Data;
using Final.Models;
using Microsoft.AspNetCore.Http;

namespace Final.Controllers
{
    public class MovimientoController : Controller
    {
        private readonly MiContexto _context;
        private Usuario? uLogeado;

        public MovimientoController(MiContexto context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _context.usuarios
                    .Include(u => u.tarjetas)
                    .Include(u => u.cajas)
                    .Load();
            _context.cajas
                .Include(c => c.movimientos)
                .Include(c => c.titulares)
                .Load();
            _context.tarjetas.Load();
            uLogeado = _context.usuarios.Where(u => u.id == httpContextAccessor.HttpContext.Session.GetInt32("UserId")).FirstOrDefault();
        }
        // GET: Movimiento
        public async Task<IActionResult> Index()
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var miContexto = _context.movimientos.Include(m => m.caja);
            return View(await miContexto.ToListAsync());
        }

    }
}
