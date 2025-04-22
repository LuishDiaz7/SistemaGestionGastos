using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SggApp.DAL.Entidades
{
    public class Moneda
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Simbolo { get; set; }
        public bool Activa { get; set; } = true;

        // Relaciones
        public ICollection<Gasto> Gastos { get; set; }
        public ICollection<Presupuesto> Presupuestos { get; set; }
        public ICollection<Usuario> UsuariosConMonedaPredeterminada { get; set; }
        public ICollection<TipoCambio> TiposCambioOrigen { get; set; }
        public ICollection<TipoCambio> TiposCambioDestino { get; set; }
    }
}
