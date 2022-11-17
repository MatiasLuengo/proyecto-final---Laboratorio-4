using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrtopediaDM
{
    public class Tipo
    {
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El Nombre es requerido")]
        public string nombre { get; set; }
        [Display(Name = "Sección")]
        public int seccionId { get; set; }
        [Display(Name = "Sección")]
        public Seccion seccion { get; set; }
        public List<Producto> productos { get; set; }
    }
}
