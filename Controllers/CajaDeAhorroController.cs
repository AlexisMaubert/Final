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
    public class CajaDeAhorroController : Controller
    {
        private readonly MiContexto _context;

        public CajaDeAhorroController(MiContexto context)
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
        // GET: CajaDeAhorro
        public  IActionResult Index()
        {
            Usuario usuario =  usuarioLogeado();
            if ( usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (usuario.isAdmin)
            {
                ViewBag.Nombre = "Administrador: " +usuario.nombre + " " + usuario.apellido;
                return View( _context.cajas.ToList());
            }
            ViewBag.Nombre = usuario.nombre + " " + usuario.apellido;
            return View( usuario.cajas.ToList());
        }

        // GET: CajaDeAhorro/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (usuarioLogeado() == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.cajas == null)
            {
                return NotFound();
            }

            var cajaDeAhorro = await _context.cajas
                .FirstOrDefaultAsync(m => m.id == id);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }

            return View(cajaDeAhorro);
        }

        // GET: CajaDeAhorro/Create
        public IActionResult Create()
        {
            if (usuarioLogeado() == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        // POST: CajaDeAhorro/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,cbu,saldo")] CajaDeAhorro cajaDeAhorro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cajaDeAhorro);
                Usuario usuario = usuarioLogeado();
                usuario.cajas.Add(cajaDeAhorro);
                cajaDeAhorro.titulares.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cajaDeAhorro);
        }

        // GET: CajaDeAhorro/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (usuarioLogeado() == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.cajas == null)
            {
                return NotFound();
            }

            var cajaDeAhorro = await _context.cajas.FindAsync(id);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }
            return View(cajaDeAhorro);
        }

        // POST: CajaDeAhorro/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,cbu,saldo")] CajaDeAhorro cajaDeAhorro)
        {
            if (id != cajaDeAhorro.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cajaDeAhorro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CajaDeAhorroExists(cajaDeAhorro.id))
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
            return View(cajaDeAhorro);
        }

        // GET: CajaDeAhorro/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (usuarioLogeado() == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.cajas == null)
            {
                return NotFound();
            }
            var cajaDeAhorro = await _context.cajas.FirstOrDefaultAsync(m => m.id == id);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }
            return View(cajaDeAhorro);
        }

        // POST: CajaDeAhorro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.cajas == null)
            {
                return Problem("Entity set 'MiContexto.cajas'  is null.");
            }
            var cajaDeAhorro = await _context.cajas.FindAsync(id);
            if (cajaDeAhorro != null)
            {
                _context.cajas.Remove(cajaDeAhorro);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CajaDeAhorroExists(int id)
        {
          return _context.cajas.Any(e => e.id == id);
        }

        public Usuario usuarioLogeado() //tomar sesion del usuario
        {
            return  _context.usuarios.Where(u => u.id == HttpContext.Session.GetInt32("UserId")).FirstOrDefault();
        }
    }
}
