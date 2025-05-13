using SggApp.BLL.Interfaces;
using SggApp.DAL.Entidades;
using SggApp.DAL.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SggApp.BLL.Servicios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioService(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        {
            return await _usuarioRepository.GetAllAsync();
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _usuarioRepository.GetByIdAsync(id);
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            var usuarios = await _usuarioRepository.GetByConditionAsync(u => u.Email == email);
            return usuarios.FirstOrDefault();
        }

        public async Task AgregarAsync(Usuario usuario)
        {
            await _usuarioRepository.AddAsync(usuario);
        }

        public async Task ActualizarAsync(Usuario usuario)
        {
            _usuarioRepository.Update(usuario);
        }

        public async Task EliminarAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario != null)
            {
                _usuarioRepository.Delete(usuario);
            }
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            var usuarios = await _usuarioRepository.GetByConditionAsync(u => u.Email == email);
            return usuarios.Any();
        }

        public async Task CambiarMonedaPredeterminadaAsync(int usuarioId, int monedaId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario != null)
            {
                usuario.MonedaPredeterminadaId = monedaId;
                _usuarioRepository.Update(usuario);
            }
        }
    }
}
