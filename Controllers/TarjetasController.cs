using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final.Data;
using Final.Models;
using System.Reflection.Metadata.Ecma335;

namespace Final.Controllers
{
    public class TarjetasController : Controller
    {
        private readonly MiContexto _context;
        private Usuario uLogeado;

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

        public Usuario usuarioLogeado() //tomar sesion del usuario
        {
            if (HttpContext != null)
            {
                return _context.usuarios.Where(u => u.id == HttpContext.Session.GetInt32("UserId")).FirstOrDefault();
            }
            return null;
        }

        // GET: Tarjetas
        public IActionResult Index()
        {
            uLogeado = usuarioLogeado();
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (uLogeado.isAdmin)
            {
                ViewData["Admin"] = "True";
                var miContexto = _context.tarjetas.Include(t => t.titular);
                return View( miContexto.ToList());

            }
            else
            {
                ViewData["Admin"] = "False";
                return View(uLogeado.tarjetas.ToList());
            }
        }

        // GET: Tarjetas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            uLogeado = usuarioLogeado();
            if (uLogeado == null)
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
            uLogeado = usuarioLogeado();
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.id_titular = new SelectList(_context.usuarios, "id", "apellido");
            Random random = new();
            int nuevoNumero = random.Next(100000000, 999999999);
            ViewBag.numero = nuevoNumero;
            while (_context.tarjetas.Any(t => t.numero == nuevoNumero))
            {  // Mientras haya alguna tarjeta con ese numero se crea otro numero
                nuevoNumero = random.Next(100000000, 999999999);
                ViewBag.numero = nuevoNumero;
            }
            ViewBag.codigoV = random.Next(100, 999);
            ViewBag.limite = 20000;
            ViewBag.consumo = 0;
            return View();
        }

        // POST: Tarjetas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,limite,numero,codigoV,consumo")] Tarjeta tarjeta)
        {
            uLogeado = usuarioLogeado();
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                tarjeta.titular = uLogeado;
                tarjeta.id_titular = uLogeado.id;

                _context.Add(tarjeta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.id_titular = new SelectList(_context.usuarios, "id", "apellido", tarjeta.id_titular);
            return View(tarjeta);
        }

        // GET: Tarjetas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            uLogeado = usuarioLogeado();
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (!uLogeado.isAdmin)
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
            uLogeado = usuarioLogeado();
            if (uLogeado == null)
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
        // GET: Depositar
       
        private bool TarjetaExists(int id)
        {
            return _context.tarjetas.Any(e => e.id == id);
        }
    }
}
