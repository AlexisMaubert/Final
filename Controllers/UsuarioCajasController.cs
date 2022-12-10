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
    public class UsuarioCajasController : Controller
    {
        private readonly MiContexto _context;

        public UsuarioCajasController(MiContexto context)
        {
            _context = context;
        }

        // GET: UsuarioCajas
        public async Task<IActionResult> Index()
        {
            var miContexto = _context.UsuarioCaja.Include(u => u.caja).Include(u => u.usuario);
            return View(await miContexto.ToListAsync());
        }

        // GET: UsuarioCajas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UsuarioCaja == null)
            {
                return NotFound();
            }

            var usuarioCaja = await _context.UsuarioCaja
                .Include(u => u.caja)
                .Include(u => u.usuario)
                .FirstOrDefaultAsync(m => m.idUsuario == id);
            if (usuarioCaja == null)
            {
                return NotFound();
            }

            return View(usuarioCaja);
        }

        // GET: UsuarioCajas/Create
        public IActionResult Create()
        {
            ViewData["idCaja"] = new SelectList(_context.cajas, "id", "id");
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "apellido");
            return View();
        }

        // POST: UsuarioCajas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idUsuario,idCaja")] UsuarioCaja usuarioCaja)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuarioCaja);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["idCaja"] = new SelectList(_context.cajas, "id", "id", usuarioCaja.idCaja);
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "apellido", usuarioCaja.idUsuario);
            return View(usuarioCaja);
        }

        // GET: UsuarioCajas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UsuarioCaja == null)
            {
                return NotFound();
            }

            var usuarioCaja = await _context.UsuarioCaja.FindAsync(id);
            if (usuarioCaja == null)
            {
                return NotFound();
            }
            ViewData["idCaja"] = new SelectList(_context.cajas, "id", "id", usuarioCaja.idCaja);
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "apellido", usuarioCaja.idUsuario);
            return View(usuarioCaja);
        }

        // POST: UsuarioCajas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idUsuario,idCaja")] UsuarioCaja usuarioCaja)
        {
            if (id != usuarioCaja.idUsuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarioCaja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioCajaExists(usuarioCaja.idUsuario))
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
            ViewData["idCaja"] = new SelectList(_context.cajas, "id", "id", usuarioCaja.idCaja);
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "apellido", usuarioCaja.idUsuario);
            return View(usuarioCaja);
        }

        // GET: UsuarioCajas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UsuarioCaja == null)
            {
                return NotFound();
            }

            var usuarioCaja = await _context.UsuarioCaja
                .Include(u => u.caja)
                .Include(u => u.usuario)
                .FirstOrDefaultAsync(m => m.idUsuario == id);
            if (usuarioCaja == null)
            {
                return NotFound();
            }

            return View(usuarioCaja);
        }

        // POST: UsuarioCajas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UsuarioCaja == null)
            {
                return Problem("Entity set 'MiContexto.UsuarioCaja'  is null.");
            }
            var usuarioCaja = await _context.UsuarioCaja.FindAsync(id);
            if (usuarioCaja != null)
            {
                _context.UsuarioCaja.Remove(usuarioCaja);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioCajaExists(int id)
        {
          return _context.UsuarioCaja.Any(e => e.idUsuario == id);
        }
    }
}
