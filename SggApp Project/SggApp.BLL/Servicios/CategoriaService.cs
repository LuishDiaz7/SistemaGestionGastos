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
    public class CategoriaService : ICategoriaService
    {
        private readonly CategoriaRepository _categoriaRepository;
        private readonly GastoRepository _gastoRepository;

        public CategoriaService(CategoriaRepository categoriaRepository, GastoRepository gastoRepository)
        {
            _categoriaRepository = categoriaRepository;
            _gastoRepository = gastoRepository;
        }

        public async Task<IEnumerable<Categoria>> ObtenerTodasAsync()
        {
            return await _categoriaRepository.GetAllAsync();
        }

        public async Task<Categoria> ObtenerPorIdAsync(int id)
        {
            return await _categoriaRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Categoria>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _categoriaRepository.GetByConditionAsync(c => c.UsuarioId == usuarioId);
        }

        public async Task AgregarAsync(Categoria categoria)
        {
            // Validar que no exista otra categoría con el mismo nombre para el mismo usuario
            if (await ExisteNombreParaUsuarioAsync(categoria.UsuarioId, categoria.Nombre))
                throw new ArgumentException("Ya existe una categoría con este nombre para el usuario.");

            await _categoriaRepository.AddAsync(categoria);
        }

        public async Task ActualizarAsync(Categoria categoria)
        {
            // Validar que no exista otra categoría con el mismo nombre para el mismo usuario
            var categorias = await _categoriaRepository.GetByConditionAsync(c =>
                c.UsuarioId == categoria.UsuarioId &&
                c.Nombre == categoria.Nombre &&
                c.Id != categoria.Id);

            if (categorias.Any())
                throw new ArgumentException("Ya existe una categoría con este nombre para el usuario.");

            _categoriaRepository.Update(categoria);
        }

        public async Task EliminarAsync(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria != null)
            {
                // Verificar si hay gastos asociados
                var gastos = await _gastoRepository.GetByConditionAsync(g => g.CategoriaId == id);
                if (gastos.Any())
                    throw new InvalidOperationException("No se puede eliminar la categoría porque tiene gastos asociados.");

                _categoriaRepository.Delete(categoria);
            }
        }

        public async Task<bool> ExisteNombreParaUsuarioAsync(int usuarioId, string nombre)
        {
            var categorias = await _categoriaRepository.GetByConditionAsync(c =>
                c.UsuarioId == usuarioId &&
                c.Nombre == nombre);
            return categorias.Any();
        }
    }
}
