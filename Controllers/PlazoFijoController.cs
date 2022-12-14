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
    public class PlazoFijoController : Controller
    {
        private readonly MiContexto _context;
        private Usuario uLogeado;

        public PlazoFijoController(MiContexto context)
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
            
        }

        public Usuario usuarioLogeado() //tomar sesion del usuario
        {
            if (HttpContext != null)
            {
                return _context.usuarios.Where(u => u.id == HttpContext.Session.GetInt32("UserId")).FirstOrDefault();
            }
            return null;
        }
        // GET: PlazoFijo
        public async Task<IActionResult> Index()
        {
            uLogeado = usuarioLogeado();
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var miContexto = _context.plazosFijos.Include(p => p.titular);
            return View(await miContexto.ToListAsync());
        }

        // GET: PlazoFijo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            uLogeado = usuarioLogeado();
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
            uLogeado = usuarioLogeado();
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido");
            return View();
        }

        // POST: PlazoFijo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,monto,fechaIni,fechaFin,tasa,pagado,id_titular,cbu")] PlazoFijo plazoFijo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plazoFijo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido", plazoFijo.id_titular);
            return View(plazoFijo);
        }

        // GET: PlazoFijo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            uLogeado = usuarioLogeado();
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.plazosFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazosFijos.FindAsync(id);
            if (plazoFijo == null)
            {
                return NotFound();
            }
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido", plazoFijo.id_titular);
            return View(plazoFijo);
        }

        // POST: PlazoFijo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,monto,fechaIni,fechaFin,tasa,pagado,id_titular,cbu")] PlazoFijo plazoFijo)
        {
            if (id != plazoFijo.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plazoFijo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlazoFijoExists(plazoFijo.id))
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
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido", plazoFijo.id_titular);
            return View(plazoFijo);
        }

        // GET: PlazoFijo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            uLogeado = usuarioLogeado();
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
            if (plazoFijo != null)
            {
                _context.plazosFijos.Remove(plazoFijo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlazoFijoExists(int id)
        {
          return _context.plazosFijos.Any(e => e.id == id);
        }
    }
}
