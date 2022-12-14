using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final.Data;
using Final.Models;

namespace Final.Controllers
{
    public class TarjetasController : Controller
    {
        private readonly MiContexto _context;

        public TarjetasController(MiContexto context)
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

        }

        // GET: Tarjetas
        public IActionResult Index()
        {
            var usuarioLogeado = _context.usuarios.Where(u => u.dni == HttpContext.Session.GetInt32("UserDni") && u.password == HttpContext.Session.GetString("UserPass")).FirstOrDefault();
            if (usuarioLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (usuarioLogeado.isAdmin)
            {
                ViewData["Admin"] = "True";
                var miContexto = _context.tarjetas.Include(t => t.titular);
                return View( miContexto.ToList());

            }
            else
            {
                ViewData["Admin"] = "False";
                return View(usuarioLogeado.tarjetas.ToList());
            }
        }

        // GET: Tarjetas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var usuarioLogeado = _context.usuarios.Where(u => u.dni == HttpContext.Session.GetInt32("UserDni") && u.password == HttpContext.Session.GetString("UserPass")).FirstOrDefault();
            if (usuarioLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.tarjetas == null)
            {
                return NotFound();
            }

            var tarjeta = await _context.tarjetas
                .Include(t => t.titular)
                .FirstOrDefaultAsync(m => m.id == id);
            if (tarjeta == null)
            {
                return NotFound();
            }

            return View(tarjeta);
        }

        // GET: Tarjetas/Create
        public IActionResult Create()
        {
            var usuarioLogeado = _context.usuarios.Where(u => u.dni == HttpContext.Session.GetInt32("UserDni") && u.password == HttpContext.Session.GetString("UserPass")).FirstOrDefault();
            if (usuarioLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido");
            Random random = new();
            int nuevoNumero = random.Next(100000000, 999999999);
            ViewData["numero"] = nuevoNumero;
            while (_context.tarjetas.Any(t => t.numero == nuevoNumero))
            {  // Mientras haya alguna tarjeta con ese numero se crea otro numero
                nuevoNumero = random.Next(100000000, 999999999);
                ViewBag["numero"] = nuevoNumero;
            }
            ViewData["codigoV"] = random.Next(100, 999);
            ViewData["limite"] = 20000;
            ViewData["consumo"] = 0;
            return View();
        }

        // POST: Tarjetas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,limite,numero,codigoV,consumo")] Tarjeta tarjeta)
        {

            var usuarioLogeado = _context.usuarios.Where(u => u.dni == HttpContext.Session.GetInt32("UserDni") && u.password == HttpContext.Session.GetString("UserPass")).FirstOrDefault();
            if (usuarioLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                tarjeta.titular = usuarioLogeado;
                tarjeta.id_titular = usuarioLogeado.id;

                _context.Add(tarjeta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido", tarjeta.id_titular);
            return View(tarjeta);
        }

        // GET: Tarjetas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var usuarioLogeado = _context.usuarios.Where(u => u.dni == HttpContext.Session.GetInt32("UserDni") && u.password == HttpContext.Session.GetString("UserPass")).FirstOrDefault();
            if (usuarioLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (!usuarioLogeado.isAdmin)
            {
                return RedirectToAction("Index", "Main");
            }
            if (id == null || _context.tarjetas == null)
            {
                return NotFound();
            }

            var tarjeta = await _context.tarjetas.FindAsync(id);
            if (tarjeta == null)
            {
                return NotFound();
            }

            return View(tarjeta);
        }

        // POST: Tarjetas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,id_titular,numero,codigoV,limite,consumo")] Tarjeta tarjeta)
        {
            if (id != tarjeta.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarjeta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarjetaExists(tarjeta.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido", tarjeta.id_titular);
            return View(tarjeta);
        }

        // GET: Tarjetas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var usuarioLogeado = _context.usuarios.Where(u => u.dni == HttpContext.Session.GetInt32("UserDni") && u.password == HttpContext.Session.GetString("UserPass")).FirstOrDefault();
            if (usuarioLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.tarjetas == null)
            {
                return NotFound();
            }

            var tarjeta = await _context.tarjetas
                .Include(t => t.titular)
                .FirstOrDefaultAsync(m => m.id == id);
            if (tarjeta == null)
            {
                return NotFound();
            }

            return View(tarjeta);
        }

        // POST: Tarjetas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.tarjetas == null)
            {
                return Problem("Entity set 'MiContexto.tarjetas'  is null.");
            }
            var tarjeta = await _context.tarjetas.FindAsync(id);
            if (tarjeta != null)
            {
                _context.tarjetas.Remove(tarjeta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TarjetaExists(int id)
        {
            return _context.tarjetas.Any(e => e.id == id);
        }
    }
}
