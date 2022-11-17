using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrtopediaDM;
using OrtopediaDM.Data;
using OrtopediaDM.ViewsModels;


namespace OrtopediaDM.Controllers
{
    [Authorize]
    public class SeccionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeccionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Secciones
        public async Task<IActionResult> Index( int pagina = 1)
        {
            paginador paginador = new paginador()
            {
                cantReg = _context.Seccion.Count(),
                pagActual = pagina,
                regXpag = 5
            };
            ViewData["paginador"] = paginador;

            var datosAmostrar = _context.Seccion
                .Skip((paginador.pagActual - 1) * paginador.regXpag)
                .Take(paginador.regXpag);

            return View(await datosAmostrar.ToListAsync());

            //return View(await _context.Seccion.ToListAsync());
        }

        [AllowAnonymous]
        // GET: Secciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seccion = await _context.Seccion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seccion == null)
            {
                return NotFound();
            }

            return View(seccion);
        }

        // GET: Secciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Secciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,nombre")] Seccion seccion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(seccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(seccion);
        }

        // GET: Secciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seccion = await _context.Seccion.FindAsync(id);
            if (seccion == null)
            {
                return NotFound();
            }
            return View(seccion);
        }

        // POST: Secciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,nombre")] Seccion seccion)
        {
            if (id != seccion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeccionExists(seccion.Id))
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
            return View(seccion);
        }

        // GET: Secciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seccion = await _context.Seccion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seccion == null)
            {
                return NotFound();
            }

            return View(seccion);
        }

        // POST: Secciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seccion = await _context.Seccion.FindAsync(id);
            _context.Seccion.Remove(seccion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeccionExists(int id)
        {
            return _context.Seccion.Any(e => e.Id == id);
        }
    }
}
