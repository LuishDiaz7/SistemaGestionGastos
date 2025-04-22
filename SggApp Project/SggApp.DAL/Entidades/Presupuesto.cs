using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SggApp.DAL.Entidades
{
    public class Presupuesto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int? CategoriaId { get; set; }
        public int MonedaId { get; set; }
        public decimal Limite { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int? NotificarAl { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relaciones
        public Usuario Usuario { get; set; }
        public Categoria Categoria { get; set; }
        public Moneda Moneda { get; set; }
    }
}
