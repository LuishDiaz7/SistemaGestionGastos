using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SggApp.DAL.Data;
using SggApp.DAL.Entidades;

namespace SggApp.DAL.Repositorios
{
    public class CategoriaRepository : GenericRepository<Categoria>
    {

        public CategoriaRepository(ApplicationDbContext context) : base(context)
        {
           
        }

        // Método específico: Obtener categorías de un usuario
        public async Task<IEnumerable<Categoria>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Categorias
                .Where(c => c.UsuarioId == usuarioId && c.Activa)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        // Método específico: Verificar si existe una categoría con el mismo nombre para un usuario
        public async Task<bool> ExistsByNameForUserAsync(int usuarioId, string nombre)
        {
            return await _context.Categorias
                .AnyAsync(c => c.UsuarioId == usuarioId && c.Nombre.ToLower() == nombre.ToLower());
        }
    }
}