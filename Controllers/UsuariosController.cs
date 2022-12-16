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
    public class UsuariosController : Controller
    {
        private readonly MiContexto _context;
        private Usuario? uLogeado;

        public UsuariosController(MiContexto context, IHttpContextAccessor httpContextAccessor)
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
            uLogeado = _context.usuarios.Where(u => u.id == httpContextAccessor.HttpContext.Session.GetInt32("UserId")).FirstOrDefault();

        }
        public Usuario usuarioLogeado() //tomar sesion del usuario
        {
            if (HttpContext != null)
            {
                return _context.usuarios.Where(u => u.id == HttpContext.Session.GetInt32("UserId")).FirstOrDefault();
            }
            return null;
        }
        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            if (uLogeado == null)
            {
                return  RedirectToAction("Index", "Login");
            }
            return View(await _context.usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(m => m.id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,dni,nombre,apellido,mail,intentosFallidos,bloqueado,password,isAdmin")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,dni,nombre,apellido,mail,intentosFallidos,bloqueado,password,isAdmin")] Usuario usuario)
        {
            if (id != usuario.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.id))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }
            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(m => m.id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.usuarios == null)
            {
                return Problem("Entity set 'MiContexto.usuarios'  is null.");
            }
            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
          return _context.usuarios.Any(e => e.id == id);
        }
    }
}
