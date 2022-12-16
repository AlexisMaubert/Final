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
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Final.Controllers
{
    public class PagoController : Controller
    {
        private readonly MiContexto _context;
        private Usuario? uLogeado;

        public PagoController(MiContexto context, IHttpContextAccessor httpContextAccessor)
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
            _context.pagos.Load();
            _context.tarjetas.Load();
            uLogeado = _context.usuarios.Where(u => u.id == httpContextAccessor.HttpContext.Session.GetInt32("UserId")).FirstOrDefault();

        }
        // GET: Pago
        public IActionResult Index()
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (uLogeado.isAdmin)
            {
                ViewData["Admin"] = "True";
                return View(_context.pagos.ToList());
            }
            else
            {
                ViewData["Admin"] = "False";
                return View(uLogeado.pagos.ToList());
            }
        }

        // GET: Pago/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (uLogeado == null)
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
            if (uLogeado == null)
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
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewData["id_usuario"] = new SelectList(_context.usuarios, "id", "apellido", pago.id_usuario);
            Usuario titular;
            if (uLogeado.isAdmin)
            {
                titular = _context.usuarios.FirstOrDefault(u => u.id == pago.id_usuario);
                if (titular == null)
                {
                    ViewBag.error = 1;
                    return View();
                }
            }
            else
            {
                titular = uLogeado;
                pago.id_usuario = uLogeado.id;
            }
            if (ModelState.IsValid)
            {

                pago.usuario = titular;
                _context.Add(pago);
                titular.pagos.Add(pago);
                _context.Update(titular);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Pago");
            }
            return View(pago);
        }

        // GET: Pago/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (!uLogeado.isAdmin)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Admin = uLogeado.isAdmin;
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
                ViewBag.Admin = uLogeado.isAdmin; //Es para la vista
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
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }
            Pago pago;
            if (uLogeado.isAdmin)
            {
                pago = await _context.pagos
                   .Include(p => p.usuario)
                   .FirstOrDefaultAsync(m => m.id == id);
            }
            else
            {
                pago = uLogeado.pagos
                    .FirstOrDefault(p => p.id == id);
            }
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
            if (pago == null)
            {
                return NotFound();
            }
            if (!pago.pagado)
            {
                ViewBag.error = 0;
                return View(pago);
            }
            if (uLogeado.isAdmin)
            {
                pago.usuario.pagos.Remove(pago);
                _context.Update(pago.usuario);
            }
            else

            {
                uLogeado.pagos.Remove(pago);
                _context.Update(uLogeado);
            }
            _context.pagos.Remove(pago);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        //Get Pagar
        public async Task<IActionResult> Pagar(int? id)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }
            Pago pago;
            if (uLogeado.isAdmin)
            {
                ViewBag.cajas = _context.cajas.ToList();
                ViewBag.tarjetas = _context.tarjetas.ToList();
                pago = await _context.pagos
                   .Include(p => p.usuario)
                   .FirstOrDefaultAsync(m => m.id == id);
            }
            else
            {
                ViewBag.cajas = uLogeado.cajas.ToList();
                ViewBag.tarjetas = uLogeado.tarjetas.ToList();
                pago = uLogeado.pagos
                    .FirstOrDefault(p => p.id == id);
            }
            if (pago == null)
            {
                return NotFound();
            }
            
            return View(pago);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pagar(int id, string metodo, int idMetodoDePago)
        {
            if (_context.pagos == null)
            {
                return Problem("Entity set 'MiContexto.pagos'  is null.");
            }
            var pago = await _context.pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }
            CajaDeAhorro caja =await  _context.cajas.Where(c => c.id == idMetodoDePago).FirstOrDefaultAsync();
            if (pago.pagado)
            {
                ViewBag.error = 0;
                return View(pago);
            }
            if(caja == null)
            {
                ViewBag.error = 1;
                return View(pago);
            }
            if(caja.saldo < pago.monto)
            {

            }
            pago.pagado = true;
            pago.metodo = metodo;
            _context.Update(pago);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        private bool PagoExists(int id)
        {
            return _context.pagos.Any(e => e.id == id);
        }
    }
}
