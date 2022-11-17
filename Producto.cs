using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrtopediaDM
{
    public class Producto
    {
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El Nombre es requerido")]
        public string nombre { get; set; }
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }
        [Display(Name = "Imagen")]
        public string imagenArticulo { get; set; }
        [Display(Name = "Marca")]
        public int marcaId { get; set; }
        [Display(Name = "Marca")]
        public Marca marca { get; set; }
        [Display(Name = "Tipo")]
        public int tipoId { get; set; }
        [Display(Name = "Tipo")]
        public Tipo tipo { get; set; }
    }
}
