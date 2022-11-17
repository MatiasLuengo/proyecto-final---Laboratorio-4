using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrtopediaDM.ViewsModels
{
    public class ProductoViewModel
    {
        public List<Producto> Articulos { get; set; }
        public SelectList Marcas { get; set; }
        public SelectList Tipos { get; set; }
        public SelectList Secciones { get; set; }
        public string nombre { get; set; }
        public paginador paginador { get; set; }
    }

    public class paginador
    {
        public int cantReg { get; set; }
        public int regXpag { get; set; }
        public int pagActual { get; set; }
        public int totalPag => (int)Math.Ceiling((decimal)cantReg / regXpag);
        public Dictionary<string, string> ValoresQueryString { get; set; } = new Dictionary<string, string>();
    }

}
