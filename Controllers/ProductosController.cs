using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrtopediaDM;
using OrtopediaDM.Data;
using OrtopediaDM.ViewsModels;

namespace OrtopediaDM.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment env;

        public ProductosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }

        [AllowAnonymous]
        // GET: Productos
        public async Task<IActionResult> Index(string filtroNombre, int? marcaId, int? tipoId, int? seccionId, int pagina = 1)
        {
            //ViewData["marcaId"] = new SelectList(_context.Marca, "Id", "nombre", marcaId);
            //var applicationDbContext = _context.Producto.Include(p => p.marca).Include(p => p.tipo).ThenInclude(y => y.seccion);
            //return View(await applicationDbContext.ToListAsync());

            paginador paginador = new paginador()
            {
                pagActual = pagina,
                regXpag = 5
            };

            var applicationDbContext = _context.Producto.Include(p => p.marca).Include(p => p.tipo).ThenInclude(y => y.seccion).Select(p => p).Select(y => y);

            if (!string.IsNullOrEmpty(filtroNombre))
            {
                applicationDbContext = applicationDbContext.Where(a => a.nombre.Contains(filtroNombre));
            }
            if (marcaId.HasValue)
            {
                applicationDbContext = applicationDbContext.Where(a => a.marcaId == marcaId.Value);
            }
            if (tipoId.HasValue)
            {
                applicationDbContext = applicationDbContext.Where(a => a.tipoId == tipoId.Value);
            }
            if (seccionId.HasValue)
            {
                applicationDbContext = applicationDbContext.Where(a => a.tipo.seccionId == seccionId.Value);
            }

            paginador.cantReg = applicationDbContext.Count();

            var datosAmostrar = applicationDbContext
                .Skip((paginador.pagActual - 1) * paginador.regXpag)
                .Take(paginador.regXpag);

            foreach (var item in Request.Query)
                paginador.ValoresQueryString.Add(item.Key, item.Value);

            ProductoViewModel modelo = new ProductoViewModel()
            {
                Articulos = datosAmostrar.ToList(),
                Marcas = new SelectList(_context.Marca, "Id", "nombre", marcaId),
                Tipos = new SelectList(_context.Tipo, "Id", "nombre", tipoId),
                Secciones = new SelectList(_context.Seccion, "Id", "nombre", seccionId),
                nombre = filtroNombre,
                paginador = paginador
            };

            //ViewData["marcaId"] = new SelectList(_context.Marca, "Id", "nombre", marcaId);
            //ViewData["tipoId"] = new SelectList(_context.Tipo, "Id", "nombre", tipoId);
            //ViewData["seccionId"] = new SelectList(_context.Seccion, "Id", "nombre", seccionId);

            //return View(await applicationDbContext.ToListAsync());
             return View (modelo);
        }

        public async Task<IActionResult> Importar()
        {
            var archivos = HttpContext.Request.Form.Files;
            if (archivos != null && archivos.Count > 0)
            {
                var archivo = archivos[0];
                if (archivo.Length > 0)
                {
                    var pathDestino = Path.Combine(env.WebRootPath, "import");
                    var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(archivo.FileName);
                    var rutaDestino = Path.Combine(pathDestino, archivoDestino);
                    using (var filestream = new FileStream(rutaDestino, FileMode.Create))
                    {
                        archivo.CopyTo(filestream);
                    }

                    using (var file = new FileStream(rutaDestino, FileMode.Open))
                    {
                        List<string> renglones = new List<string>();
                        List<Producto> productoArchivo = new List<Producto>();

                        StreamReader fileContent = new StreamReader(file, System.Text.Encoding.Default);
                        do
                        {
                            renglones.Add(fileContent.ReadLine());
                        } while (!fileContent.EndOfStream);

                        if (renglones.Count() > 0)
                        {
                            foreach (var row in renglones)
                            {
                                string[] data = row.Split(';');
                                if (data.Length == 5)
                                {
                                    Producto productoCarga = new Producto();

                                    productoCarga.nombre = data[0].Trim();
                                    productoCarga.descripcion = data[1].Trim();
                                    productoCarga.imagenArticulo = data[2].Trim();
                                    productoCarga.marcaId = int.Parse(data[3].Trim());
                                    productoCarga.tipoId = int.Parse(data[4].Trim());
                                    productoArchivo.Add(productoCarga);
                                }
                            }
                            if (productoArchivo.Count() > 0)
                            {
                                _context.AddRange(productoArchivo);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }//end of using
                }
            }
            //var applicationDbContext = _context.Producto.Include(p => p.marca).Include(p => p.tipo).ThenInclude(y => y.seccion);
            //return View("Index",await applicationDbContext.ToListAsync());

            return RedirectToAction(nameof(Index));

        }

        [AllowAnonymous]
        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .Include(p => p.marca)
                .Include(p => p.tipo).ThenInclude(y => y.seccion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["marcaId"] = new SelectList(_context.Marca, "Id", "nombre");
            ViewData["tipoId"] = new SelectList(_context.Tipo, "Id", "nombre");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,nombre,descripcion,imagenArticulo,marcaId,tipoId")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                var archivos = HttpContext.Request.Form.Files;
                if (archivos != null && archivos.Count > 0)
                {
                    var archivoImagen = archivos[0];
                    if (archivoImagen.Length > 0)
                    {
                        var pathDestino = Path.Combine(env.WebRootPath, "images\\productos");
                        var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(archivoImagen.FileName);
                        var rutaDestino = Path.Combine(pathDestino, archivoDestino);
                        using (var filestream = new FileStream(rutaDestino, FileMode.Create))
                        {
                            archivoImagen.CopyTo(filestream);
                            producto.imagenArticulo = archivoDestino;
                        }
                    }
                }

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["marcaId"] = new SelectList(_context.Marca, "Id", "nombre", producto.marcaId);
            ViewData["tipoId"] = new SelectList(_context.Tipo, "Id", "nombre", producto.tipoId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["marcaId"] = new SelectList(_context.Marca, "Id", "nombre", producto.marcaId);
            ViewData["tipoId"] = new SelectList(_context.Tipo, "Id", "nombre", producto.tipoId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,nombre,descripcion,imagenArticulo,marcaId,tipoId")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var archivos = HttpContext.Request.Form.Files;
                if (archivos != null && archivos.Count > 0)
                {
                    var archivoImagen = archivos[0];
                    if (archivoImagen.Length > 0)
                    {
                        var pathDestino = Path.Combine(env.WebRootPath, "images\\productos");
                        var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(archivoImagen.FileName);
                        var rutaDestino = Path.Combine(pathDestino, archivoDestino);

                        if (!string.IsNullOrEmpty(producto.imagenArticulo))
                        {
                            string imagenAnterior = Path.Combine(pathDestino, producto.imagenArticulo);
                            if (System.IO.File.Exists(imagenAnterior))
                                System.IO.File.Delete(imagenAnterior);
                        }

                        using (var filestream = new FileStream(rutaDestino, FileMode.Create))
                        {
                            archivoImagen.CopyTo(filestream);
                            producto.imagenArticulo = archivoDestino;
                        }
                    }
                }

                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
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
            ViewData["marcaId"] = new SelectList(_context.Marca, "Id", "nombre", producto.marcaId);
            ViewData["tipoId"] = new SelectList(_context.Tipo, "Id", "nombre", producto.tipoId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .Include(p => p.marca)
                .Include(p => p.tipo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Producto.FindAsync(id);
            _context.Producto.Remove(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Producto.Any(e => e.Id == id);
        }
    }
}
