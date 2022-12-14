﻿using Final.Data;
using Final.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;

namespace Final.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly MiContexto _context;

        public LoginController(ILogger<LoginController> logger, MiContexto contexto)
        {
            _context = contexto;
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
            _logger = logger;
        }
        public IActionResult Index()
        {
            var usuarioLogeado = _context.usuarios.Where(u => u.dni == HttpContext.Session.GetInt32("UserDni") && u.password == HttpContext.Session.GetString("UserPass")).FirstOrDefault();
            if (usuarioLogeado != null)
            {
                return RedirectToAction("Index", "Main");
            }
            ViewBag.logeado = "no";
            return View();
        }
        [HttpPost]
        public IActionResult Index(int Dni, string password)
        {
            try
            {
                var usuario = _context.usuarios.Where(u => u.dni == Dni).FirstOrDefault();
                if (usuario == null)
                {
                    ViewBag.errorLogin = 1;
                    return View();
                }
                if (usuario.password != password)
                {
                    if (usuario.intentosFallidos == 3)
                    {
                        usuario.bloqueado = true;
                        _context.Update(usuario);
                        _context.SaveChanges();
                        ViewBag.errorLogin = 3;
                    }
                    else
                    {
                        ViewBag.errorLogin = 2;
                        usuario.intentosFallidos++;
                        _context.Update(usuario);
                        _context.SaveChanges();
                    }
                    return View();
                }
                if (usuario.bloqueado)
                {
                    ViewBag.errorLogin = 4;
                    return View();
                }
                HttpContext.Session.SetInt32("UserId", usuario.id);
                HttpContext.Session.SetInt32("UserDni", Dni);
                HttpContext.Session.SetString("UserPass", password);

                if (usuario.isAdmin)
                {
                    ViewData["Admin"] = "True";
                }
                else
                {
                    ViewData["Admin"] = "False";
                }
                return RedirectToAction("Index", "Main");
            }
            catch
            {

                return View();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}