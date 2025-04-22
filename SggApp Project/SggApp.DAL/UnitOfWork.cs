using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SggApp.DAL.Data;
using SggApp.DAL.Repositorios;

namespace SggApp.DAL
{
    public class UnitOfWork : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private bool _disposed = false;

        // Repositorios
        private UsuarioRepository _usuarioRepository;
        private CategoriaRepository _categoriaRepository;
        private MonedaRepository _monedaRepository;
        private GastoRepository _gastoRepository;
        private PresupuestoRepository _presupuestoRepository;
        private TipoCambioRepository _tipoCambioRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            _usuarioRepository = new UsuarioRepository(_context);
            _categoriaRepository = new CategoriaRepository(_context);
            _monedaRepository = new MonedaRepository(_context);
            _gastoRepository = new GastoRepository(_context);
            _presupuestoRepository = new PresupuestoRepository(_context);
            _tipoCambioRepository = new TipoCambioRepository(_context);
        }

        // Propiedades de repositorios (inicialización perezosa)
        public UsuarioRepository UsuarioRepository
        {
            get
            {
                if (_usuarioRepository == null)
                {
                    _usuarioRepository = new UsuarioRepository(_context);
                }
                return _usuarioRepository;
            }
        }

        public CategoriaRepository CategoriaRepository
        {
            get
            {
                if (_categoriaRepository == null)
                {
                    _categoriaRepository = new CategoriaRepository(_context);
                }
                return _categoriaRepository;
            }
        }

        public MonedaRepository MonedaRepository
        {
            get
            {
                if (_monedaRepository == null)
                {
                    _monedaRepository = new MonedaRepository(_context);
                }
                return _monedaRepository;
            }
        }

        public GastoRepository GastoRepository
        {
            get
            {
                if (_gastoRepository == null)
                {
                    _gastoRepository = new GastoRepository(_context);
                }
                return _gastoRepository;
            }
        }

        public PresupuestoRepository PresupuestoRepository
        {
            get
            {
                if (_presupuestoRepository == null)
                {
                    _presupuestoRepository = new PresupuestoRepository(_context);
                }
                return _presupuestoRepository;
            }
        }

        public TipoCambioRepository TipoCambioRepository
        {
            get
            {
                if (_tipoCambioRepository == null)
                {
                    _tipoCambioRepository = new TipoCambioRepository(_context);
                }
                return _tipoCambioRepository;
            }
        }

        // Guardar todos los cambios de todos los repositorios
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Implementación de IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}