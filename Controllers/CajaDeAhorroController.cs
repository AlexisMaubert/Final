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
using Microsoft.AspNetCore.Html;
using Microsoft.IdentityModel.Protocols;

namespace Final.Controllers
{
    public class CajaDeAhorroController : Controller
    {
        private readonly MiContexto _context;
        private Usuario uLogeado;

        public CajaDeAhorroController(MiContexto context, IHttpContextAccessor httpContextAccessor)
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
            uLogeado = _context.usuarios.Where(u => u.id == httpContextAccessor.HttpContext.Session.GetInt32("UserId")).FirstOrDefault();
        }
        // GET: CajaDeAhorro
        public IActionResult Index()
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (uLogeado.isAdmin)
            {
                ViewBag.Admin = "True";
                ViewBag.Nombre = "Administrador: " + uLogeado.nombre + " " + uLogeado.apellido;
                return View(_context.cajas.ToList());
            }
            else
            {
                ViewBag.Admin = "False";
                ViewBag.Nombre = uLogeado.nombre + " " + uLogeado.apellido;
                return View(uLogeado.cajas.ToList());
            }
            
        }
        [HttpGet]
        public IActionResult Index(string success)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.success = success;
            if (uLogeado.isAdmin)
            {
                ViewBag.Admin = "True";
                ViewBag.Nombre = "Administrador: " + uLogeado.nombre + " " + uLogeado.apellido;
                return View(_context.cajas.ToList());
            }
            else
            {
                ViewBag.Admin = "False";
                ViewBag.Nombre = uLogeado.nombre + " " + uLogeado.apellido;
                return View(uLogeado.cajas.ToList());
            }

        }

        // GET: CajaDeAhorro/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (uLogeado == null)
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
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Admin = uLogeado.isAdmin;

            ViewBag.id_titular = new SelectList(_context.usuarios, "id", "apellido");
            Random random = new();
            int nuevoNumero = random.Next(100000000, 999999999);
            ViewBag.cbu = nuevoNumero;
            while (_context.cajas.Any(t => t.cbu == nuevoNumero))
            {  // Mientras haya alguna tarjeta con ese numero se crea otro numero
                nuevoNumero = random.Next(100000000, 999999999);
                ViewBag.cbu = nuevoNumero;
            }
            ViewBag.saldo = 0;
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
                uLogeado.cajas.Add(cajaDeAhorro);
                cajaDeAhorro.titulares.Add(uLogeado);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","CajaDeAhorro", new { success = "1" });
            }
            return View(cajaDeAhorro);
        }

        // GET: CajaDeAhorro/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (uLogeado == null)
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
                return RedirectToAction("Index","CajaDeAhorro", new { success = "2" });
            }
            return View(cajaDeAhorro);
        }

        // GET: CajaDeAhorro/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (uLogeado == null)
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
            return RedirectToAction("Index", "CajaDeAhorro", new { success = "3" });
        }


        // GET: CajaDeAhorro/Depositar
        public IActionResult Depositar(int? id)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (id == null || uLogeado.cajas == null)
            {
                return NotFound();
            }
            CajaDeAhorro caja;
            if (uLogeado.isAdmin)
            {
                caja = _context.cajas.FirstOrDefault(c => c.id == id);
            }
            else
            {
                caja = uLogeado.cajas
                .FirstOrDefault(c => c.id == id);
            }
            if (caja == null)
            {
                return NotFound();
            }

            return View(caja);
        }

        // POST: CajaDeAhorro/Depositar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Depositar(int? id, float Monto)
        {
            CajaDeAhorro caja;
            if (uLogeado.isAdmin)
            {
                caja = _context.cajas.FirstOrDefault(C => C.id == id);
            }
            else
            {
                caja = uLogeado.cajas.FirstOrDefault(C => C.id == id);
            }
            if (caja == null)
            {
                return NotFound();
            }
            if (Monto <= 0)
            {
                ViewBag.error = 1;
                return View(caja);
            }
            altaMovimiento(caja, "Depositar", Monto);
            caja.saldo += Monto;
            _context.Update(caja);
            _context.SaveChanges();
            return RedirectToAction("Index", "CajaDeAhorro", new { success = "4" });

        }

        // GET: CajaDeAhorro/Retirar
        public IActionResult Retirar(int? id)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (id == null || uLogeado.cajas == null)
            {
                return NotFound();
            }

            CajaDeAhorro caja;
            if (uLogeado.isAdmin)
            {
                caja = _context.cajas.FirstOrDefault(c => c.id == id);
            }
            else
            {
                caja = uLogeado.cajas
                .FirstOrDefault(c => c.id == id);
            }
            if (caja == null)
            {
                return NotFound();
            }

            return View(caja);
        }

        // POST: CajaDeAhorro/Retirar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Retirar(int? id, float Monto)
        {
            CajaDeAhorro caja;
            if (uLogeado.isAdmin)
            {
                caja = _context.cajas.FirstOrDefault(C => C.id == id);
            }
            else
            {
                caja = uLogeado.cajas.FirstOrDefault(C => C.id == id);
            }
            if (caja == null)
            {
                return NotFound();
            }
            if (Monto <= 0)
            {
                ViewBag.error = 1;
                return View(caja);
            }
            altaMovimiento(caja, "Retirar", Monto);
            caja.saldo -= Monto;
            _context.Update(caja);
            _context.SaveChanges();
            return RedirectToAction("Index", "CajaDeAhorro", new { success = "5" });
        }

        public IActionResult Transferir()
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.cbuOrigen = uLogeado.cajas;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Transferir(int CbuOrigen, int CbuDestino, float Monto)
        {
            CajaDeAhorro cajaMia = uLogeado.cajas.FirstOrDefault(c => c.cbu == CbuOrigen);
            CajaDeAhorro cajaDestino = _context.cajas.FirstOrDefault(c => c.cbu == CbuDestino);
            if (cajaMia == null)
            {
                ViewBag.error = 1;
                return View();
            }
            if (cajaDestino == null)
            {
                ViewBag.error = 2;
                return View();
            }
            if (Monto <= 0)
            {
                ViewBag.error = 3;
                return View();
            }
            if (cajaDestino.saldo < Monto)
            {
                ViewBag.error = 4;
                return View();
            }
            cajaMia.saldo -= Monto;
            cajaDestino.saldo += Monto;
            altaMovimiento(cajaMia, "Transferencia", Monto);
            altaMovimiento(cajaDestino, "Transferencia Recibida", Monto);
            _context.Update(cajaMia);
            _context.Update(cajaDestino);
            _context.SaveChanges();

            return RedirectToAction("Index", "CajaDeAhorro", new { success = "6" });
        }

        // GET: CajaDeAhorro/AgregarTitular
        public async Task<IActionResult> AgregarTitular(int? id)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.cajas == null)
            {
                return NotFound();
            }
            CajaDeAhorro caja;
            if (uLogeado.isAdmin)
            {
                caja = _context.cajas.FirstOrDefault(C => C.id == id);
                ViewBag.usuarios = await _context.usuarios.ToListAsync();
            }
            else
            {
                caja = uLogeado.cajas.FirstOrDefault(C => C.id == id);
            }
            ViewBag.Titulares = caja.titulares;
            ViewBag.Admin = uLogeado.isAdmin;
            return View(caja);
        }

        // POST: CajaDeAhorro/AgregarTitular
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarTitular(int id, int idUsuario)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }

            CajaDeAhorro caja;
            Usuario usuario;
            if (uLogeado.isAdmin)
            {
                caja = _context.cajas.FirstOrDefault(C => C.id == id);
                usuario = _context.usuarios.FirstOrDefault(C => C.id == idUsuario);
            }
            else
            {
                caja = uLogeado.cajas.FirstOrDefault(C => C.id == id);
                usuario = _context.usuarios.FirstOrDefault(C => C.dni == idUsuario);
            }
            if (caja == null)
            {
                return NotFound();
            }
            if (usuario == null)
            {
                ViewBag.error = 1;
                return View(caja);
            }
            if (caja.titulares.Contains(usuario))
            {
                ViewBag.error = 2;
                return View(caja);
            }
            ViewBag.Titulares = caja.titulares;
            ViewBag.Admin = uLogeado.isAdmin;
            caja.titulares.Add(usuario);
            usuario.cajas.Add(caja);
            _context.Update(usuario);
            _context.Update(caja);
            _context.SaveChanges();

            return RedirectToAction("Index", "CajaDeAhorro", new { success = "7" });
        }

        public IActionResult EliminarTitular(int? id)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null || _context.cajas == null)
            {
                return NotFound();
            }
            CajaDeAhorro caja;
            if (uLogeado.isAdmin)
            {
                caja = _context.cajas.FirstOrDefault(C => C.id == id);
            }
            else
            {
                caja = uLogeado.cajas.FirstOrDefault(C => C.id == id);
            }
            ViewBag.Titulares = caja.titulares;
            ViewBag.Admin = uLogeado.isAdmin;
            return View(caja);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarTitular(int id, int idUsuario)
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }

            CajaDeAhorro caja;
            Usuario usuario;
            if (uLogeado.isAdmin)
            {
                caja = _context.cajas.FirstOrDefault(C => C.id == id);
                usuario = _context.usuarios.FirstOrDefault(C => C.id == idUsuario);

            }
            else
            {
                caja = uLogeado.cajas.FirstOrDefault(C => C.id == id);
                usuario = _context.usuarios.FirstOrDefault(C => C.id == idUsuario);
            }
            if (caja == null)
            {
                return NotFound();
            }

            ViewBag.Titulares = caja.titulares;
            if (caja.titulares.Count() == 1)
            {
                ViewBag.error = 1;
                return View(caja);
            }
            caja.titulares.Remove(usuario);
            usuario.cajas.Remove(caja);
            _context.Update(usuario);
            _context.Update(caja);
            _context.SaveChanges();

            return RedirectToAction("Index", "CajaDeAhorro", new { succes = "8" });
        }
        private bool CajaDeAhorroExists(int id)
        {
            return _context.cajas.Any(e => e.id == id);
        }
        /*
         * *
         * *MOVIMIENTOS
         * *
         */

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
