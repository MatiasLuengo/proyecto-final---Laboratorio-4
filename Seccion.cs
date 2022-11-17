using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrtopediaDM
{
    public class Seccion
    {
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El Nombre es requerido")]
        public string nombre { get; set; }
        public List<Tipo> tipos { get; set; }
    }
}
