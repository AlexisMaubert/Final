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
    public class PagoController : Controller
    {
        private readonly MiContexto _context;

        public PagoController(MiContexto context)
        {
            _context = context;
        }

        // GET: Pago
        public async Task<IActionResult> Index()
        {
            var usuarioLogeado = _context.usuarios.Where(u => u.dni == HttpContext.Session.GetInt32("UserDni") && u.password == HttpContext.Session.GetString("UserPass")).FirstOrDefault();
            if (usuarioLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var miContexto = _context.pagos.Include(p => p.usuario);
            return View(await miContexto.ToListAsync());
        }

        // GET: Pago/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var usuarioLogeado = _context.usuarios.Where(u => u.dni == HttpContext.Session.GetInt32("UserDni") && u.password == HttpContext.Session.GetString("UserPass")).FirstOrDefault();
            if (usuarioLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }

            var pago = await _context.pagos
                .Include(p => p.usuario)
                .FirstOrDefaultAsync(m => m.id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // GET: Pago/Create
        public IActionResult Create()
        {
            var usuarioLogeado = _context.usuarios.Where(u => u.dni == HttpContext.Session.GetInt32("UserDni") && u.password == HttpContext.Session.GetString("UserPass")).FirstOrDefault();
            if (usuarioLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewData["id_usuario"] = new SelectList(_context.usuarios, "id", "apellido");
            return View();
        }

        // POST: Pago/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,id_usuario,nombre,monto,pagado,metodo")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["id_usuario"] = new SelectList(_context.usuarios, "id", "apellido", pago.id_usuario);
            return View(pago);
        }

        // GET: Pago/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var usuarioLogeado = _context.usuarios.Where(u => u.dni == HttpContext.Session.GetInt32("UserDni") && u.password == HttpContext.Session.GetString("UserPass")).FirstOrDefault();
            if (usuarioLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }

            var pago = await _context.pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }
            ViewData["id_usuario"] = new SelectList(_context.usuarios, "id", "apellido", pago.id_usuario);
            return View(pago);
        }

        // POST: Pago/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,id_usuario,nombre,monto,pagado,metodo")] Pago pago)
        {
            if (id != pago.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagoExists(pago.id))
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
            ViewData["id_usuario"] = new SelectList(_context.usuarios, "id", "apellido", pago.id_usuario);
            return View(pago);
        }

        // GET: Pago/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var usuarioLogeado = _context.usuarios.Where(u => u.dni == HttpContext.Session.GetInt32("UserDni") && u.password == HttpContext.Session.GetString("UserPass")).FirstOrDefault();
            if (usuarioLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }

            var pago = await _context.pagos
                .Include(p => p.usuario)
                .FirstOrDefaultAsync(m => m.id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // POST: Pago/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.pagos == null)
            {
                return Problem("Entity set 'MiContexto.pagos'  is null.");
            }
            var pago = await _context.pagos.FindAsync(id);
            if (pago != null)
            {
                _context.pagos.Remove(pago);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PagoExists(int id)
        {
          return _context.pagos.Any(e => e.id == id);
        }
    }
}
