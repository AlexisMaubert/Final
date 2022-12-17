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
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;

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
                ViewBag.Admin = true;
                return View(_context.pagos.ToList());
            }
            else
            {
                ViewBag.Admin = false;
                return View(uLogeado.pagos.ToList());
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
                return View(_context.pagos.ToList());
            }
            else
            {
                ViewBag.Admin = false;
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
            ViewBag.Admin = uLogeado.isAdmin;
            return View();
        }

        // POST: Pago/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,id_usuario,nombre,monto")] Pago pago)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if(pago.monto < 0)
            {
                ViewBag.error = 1;
                return View();
            }
            ViewBag.Admin = uLogeado.isAdmin;
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
                pago.metodo = "No pagado";
                pago.pagado = false;
                pago.usuario = titular;
                _context.Add(pago);
                titular.pagos.Add(pago);
                _context.Update(titular);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Pago", new { success = 1 });
            }
            return View(pago);
        }


        // POST: Pago/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        public async Task<IActionResult> Edit(int id)
        {
            Pago pago = _context.pagos.FirstOrDefault(p => p.id == id);
            if (id != pago.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                ViewBag.Admin = uLogeado.isAdmin; //Es para la vista
                try
                {
                    pago.pagado = !pago.pagado;
                    if (pago.metodo == "No pagado")
                    {
                        pago.metodo = "Pagado desde el banco";
                    }
                    else
                    {
                        pago.metodo = "No pagado";
                    }
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
                return RedirectToAction("Index", "Pago", new { success = 2 });
            }
            ViewData["id_usuario"] = new SelectList(_context.usuarios, "id", "apellido", pago.id_usuario);
            return RedirectToAction("Index", "Pago"); ;
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
            return RedirectToAction("index", "Pago", new { success = 3 });

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
        public async Task<IActionResult> Pagar(int id, int idMetodoDePago)
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
            if (pago.pagado)
            {
                ViewBag.error = 0;
                return View(pago);
            }
            CajaDeAhorro caja = await _context.cajas.Where(c => c.id == idMetodoDePago).FirstOrDefaultAsync();
            if (caja != null)
            {
                if (caja.saldo < pago.monto)
                {
                    ViewBag.error = 1;
                    return View(pago);
                }
                pago.metodo = "Transferencia";
                altaMovimiento(caja, pago.nombre, pago.monto);
                caja.saldo -= pago.monto;
                _context.Update(caja);
            }
            else
            {
                Tarjeta tarjeta = await _context.tarjetas.Where(c => c.id == idMetodoDePago).FirstOrDefaultAsync();
                if (tarjeta == null)
                {
                    return NotFound();
                }
                float disponible = tarjeta.limite - tarjeta.consumo;
                if (disponible < pago.monto)
                {
                    ViewBag.error = 2;
                    return View(pago);
                }
                pago.metodo = "Tarjeta";
                tarjeta.consumo += pago.monto;
                _context.Update(tarjeta);
            }
            pago.pagado = true;
            _context.Update(pago);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Pago", new { success = 4 });
        }
        private bool PagoExists(int id)
        {
            return _context.pagos.Any(e => e.id == id);
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
