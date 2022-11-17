using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrtopediaDM
{
    public class Marca
    {
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El Nombre es requerido")]
        public string nombre { get; set; }
        [Display(Name = "Origen")]
        [Required(ErrorMessage = "El Origen es requerido")]
        public string origen { get; set; }
        [Display(Name = "Logo")]
        public string imagenLogo { get; set; }
        public List<Producto> productos { get; set; }
    }
}
