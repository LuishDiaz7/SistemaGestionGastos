using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SggApp.DAL.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public int? MonedaPredeterminadaId { get; set; }
        public bool Activo { get; set; } = true;

        // Relaciones
        public Moneda MonedaPredeterminada { get; set; }
        public ICollection<Categoria> Categorias { get; set; }
        public ICollection<Gasto> Gastos { get; set; }
        public ICollection<Presupuesto> Presupuestos { get; set; }
    }
}
