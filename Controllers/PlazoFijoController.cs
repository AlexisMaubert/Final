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
    public class PlazoFijoController : Controller
    {
        private readonly MiContexto _context;
        private Usuario? uLogeado;

        public PlazoFijoController(MiContexto context, IHttpContextAccessor httpContextAccessor)
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
            _context.plazosFijos.Load();
            uLogeado = _context.usuarios.Where(u => u.id == httpContextAccessor.HttpContext.Session.GetInt32("UserId")).FirstOrDefault();
        }

        // GET: PlazoFijo
        public IActionResult Index()
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (uLogeado.isAdmin)
            {
                ViewBag.Admin = true;
                ViewBag.Nombre = "Administrador: " + uLogeado.nombre + " " + uLogeado.apellido;
                return View(_context.plazosFijos.ToList());
            }
            else
            {
                ViewBag.Admin = false;
                ViewBag.Nombre = uLogeado.nombre + " " + uLogeado.apellido;
                return View(uLogeado.pf.ToList());
            }
        }
        [HttpGet]
        public IActionResult Index(int success)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.success = success;
            if (uLogeado.isAdmin)
            {
                ViewBag.Admin = true;
                ViewBag.Nombre = "Administrador: " + uLogeado.nombre + " " + uLogeado.apellido;
                return View(_context.plazosFijos.ToList());
            }
            else
            {
                ViewBag.Admin = false;
                ViewBag.Nombre = uLogeado.nombre + " " + uLogeado.apellido;
                return View(uLogeado.pf.ToList());
            }
        }

        // GET: PlazoFijo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.plazosFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazosFijos
                .Include(p => p.titular)
                .FirstOrDefaultAsync(m => m.id == id);
            if (plazoFijo == null)
            {
                return NotFound();
            }

            return View(plazoFijo);
        }

        // GET: PlazoFijo/Create
        public IActionResult Create()
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Admin = uLogeado.isAdmin;
            if (uLogeado.isAdmin)
            {
                ViewBag.cajas = _context.cajas.ToList();
            }
            else
            {
                ViewBag.cajas = uLogeado.cajas.ToList();
            }
            ViewBag.fechaIn = DateTime.Now;
            ViewBag.fechaFin = DateTime.Now.AddMonths(1);
            ViewBag.tasa = 90;

            return View();
        }

        // POST: PlazoFijo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,monto,fechaIni,fechaFin,tasa,id_titular,cbu")] PlazoFijo plazoFijo)
        {
            if (uLogeado.isAdmin)
            {
                ViewBag.cajas = _context.cajas.ToList();
            }
            else
            {
                ViewBag.cajas = uLogeado.cajas.ToList();
            }
            ViewBag.fechaIn = DateTime.Now;
            ViewBag.fechaFin = DateTime.Now.AddMonths(1);
            ViewBag.tasa = 90;
            if (plazoFijo == null)
            {
                return NotFound();
            }
            if (plazoFijo.monto < 1000)
            {
                ViewBag.error = 0;
                return View();
            }
            CajaDeAhorro caja = _context.cajas.FirstOrDefault(c => c.cbu == plazoFijo.cbu);
            if (caja == null)
            {
                return NotFound();
            }
            if (caja.saldo < plazoFijo.monto)
            {
                ViewBag.error = 1;
                return View();
            }

            if (ModelState.IsValid)
            {
                _context.Add(plazoFijo);
                if (!uLogeado.isAdmin)
                {
                    plazoFijo.id_titular = uLogeado.id;
                    plazoFijo.titular = uLogeado;
                    uLogeado.pf.Add(plazoFijo);
                    _context.Update(uLogeado);
                }
                else
                {
                    Usuario titular = caja.titulares.ToList().FirstOrDefault();
                    if (titular == null)
                    {
                        ViewBag.error = 2;
                        return View(plazoFijo);
                    }
                    plazoFijo.titular = titular;
                    titular.pf.Add(plazoFijo);
                    _context.Update(titular);
                }
                caja.saldo -= plazoFijo.monto;
                altaMovimiento(caja, "Alta plazo fijo", plazoFijo.monto);
                _context.Update(caja);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","PlazoFijo", new {success = 1});
            }

            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido", plazoFijo.id_titular);
            return View(plazoFijo);
        }


        // GET: PlazoFijo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.plazosFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazosFijos
                .Include(p => p.titular)
                .FirstOrDefaultAsync(m => m.id == id);
            if (plazoFijo == null)
            {
                return NotFound();
            }

            return View(plazoFijo);
        }

        // POST: PlazoFijo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.plazosFijos == null)
            {
                return Problem("Entity set 'MiContexto.plazosFijos'  is null.");
            }
            var plazoFijo = await _context.plazosFijos.FindAsync(id);
            if (plazoFijo == null)
            {
                return NotFound();
            }
            if (!plazoFijo.pagado)
            {
                ViewBag.error = 0;
                return View(plazoFijo);
            }
            _context.plazosFijos.Remove(plazoFijo);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "PlazoFijo", new { success = 2 });
        }

        private bool PlazoFijoExists(int id)
        {
            return _context.plazosFijos.Any(e => e.id == id);
        }

        public bool altaMovimiento(CajaDeAhorro Caja, string Detalle, float Monto)
        {
            try
            {
                Movimiento movimientoNuevo = new Movimiento(Caja, Detalle, Monto);
                _context.movimientos.Add(movimientoNuevo);
                Caja.movimientos.Add(movimientoNuevo);
                _context.Update(Caja);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
