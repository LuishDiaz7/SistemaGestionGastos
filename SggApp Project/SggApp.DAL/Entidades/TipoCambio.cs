using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SggApp.DAL.Entidades
{
    public class TipoCambio
    {
        public int Id { get; set; }
        public int MonedaOrigenId { get; set; }
        public int MonedaDestinoId { get; set; }
        public decimal Tasa { get; set; }
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        // Relaciones
        public Moneda MonedaOrigen { get; set; }
        public Moneda MonedaDestino { get; set; }
    }
}

