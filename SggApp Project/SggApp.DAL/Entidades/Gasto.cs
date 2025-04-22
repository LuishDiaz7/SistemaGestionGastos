using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SggApp.DAL.Entidades
{
    public class Gasto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int CategoriaId { get; set; }
        public int MonedaId { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public string Lugar { get; set; }
        public bool EsRecurrente { get; set; } = false;
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Relaciones
        public Usuario Usuario { get; set; }
        public Categoria Categoria { get; set; }
        public Moneda Moneda { get; set; }
    }
}
