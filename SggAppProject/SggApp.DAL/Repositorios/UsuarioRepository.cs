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
    public class UsuarioRepository : GenericRepository<Usuario>
    {

        public UsuarioRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        // Método específico: Obtener un usuario por su correo electrónico
        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Método específico: Verificar si un correo electrónico ya está registrado
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        // Método específico: Obtener todos los usuarios con sus gastos asociados
        public async Task<IEnumerable<Usuario>> GetAllWithGastosAsync()
        {
            return await _context.Users
                .Include(u => u.Gastos)
                .ToListAsync();
        }

        // Método específico: Obtener usuario con todas sus relaciones
        public async Task<Usuario?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Gastos)
                .Include(u => u.Presupuestos)
                .Include(u => u.MonedaPredeterminada)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
