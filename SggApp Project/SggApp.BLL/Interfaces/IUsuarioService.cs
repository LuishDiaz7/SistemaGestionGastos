using SggApp.DAL.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SggApp.BLL.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> ObtenerTodosAsync();
        Task<Usuario> ObtenerPorIdAsync(int id);
        Task<Usuario> ObtenerPorEmailAsync(string email);
        Task AgregarAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);
        Task EliminarAsync(int id);
        Task<bool> ExisteEmailAsync(string email);
        Task CambiarMonedaPredeterminadaAsync(int usuarioId, int monedaId);
    }
}
