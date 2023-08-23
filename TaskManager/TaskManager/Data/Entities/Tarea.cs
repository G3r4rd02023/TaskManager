using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TaskManager.Data.Entities
{
    public class Tarea
    {
        public int Id { get; set; }

        [Display(Name = "Titulo")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string? Titulo { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción")]
        [MaxLength(500, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Descripcion { get; set; }

        [Display(Name = "Fecha de Vencimiento")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]       
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public DateTime FechaVencimiento { get; set; }

        [Display(Name = "Tarea Completada")]
        public bool Completada { get; set; }


    }
}
