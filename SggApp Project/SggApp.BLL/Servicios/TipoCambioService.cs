using SggApp.BLL.Interfaces;
using SggApp.DAL.Entidades;
using SggApp.DAL.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Necesario si el repositorio usa métodos async o si necesitas el DbContext
// using SggApp.DAL.Data; // Descomentar si necesitas inyectar ApplicationDbContext aquí

namespace SggApp.BLL.Servicios
{
    public class TipoCambioService : ITipoCambioService // <<-- Implementa la interfaz completa
    {
        private readonly TipoCambioRepository _tipoCambioRepository;
        // private readonly ApplicationDbContext _context; // <-- Opcional: Inyecta DbContext si tu repositorio NO llama a SaveChangesAsync()

        public TipoCambioService(TipoCambioRepository tipoCambioRepository /*, ApplicationDbContext context */) // <-- Ajusta el constructor si inyectas DbContext
        {
            _tipoCambioRepository = tipoCambioRepository;
            // _context = context; // Asigna el DbContext si lo inyectas
        }

        // --- Métodos existentes (de tu código original) ---

        public async Task<TipoCambio?> ObtenerTipoCambioAsync(int monedaOrigen, int monedaDestino)
        {
            // Este método parece buscar un tipo de cambio específico por par de monedas.
            return await _tipoCambioRepository.GetByMonedasAsync(monedaOrigen, monedaDestino);
        }

        public async Task<decimal> ObtenerTasaCambioAsync(int monedaOrigen, int monedaDestino)
        {
            var tipoCambio = await ObtenerTipoCambioAsync(monedaOrigen, monedaDestino);
            return tipoCambio?.Tasa ?? throw new KeyNotFoundException("No se encontró el tipo de cambio especificado.");
        }

        // Nota: Este método ActualizarTipoCambioAsync (por par de monedas y tasa) existe.
        // La interfaz ahora también tiene ActualizarAsync (por objeto TipoCambio). Ambos pueden coexistir.
        public async Task ActualizarTipoCambioAsync(int monedaOrigen, int monedaDestino, decimal tasa)
        {
            var tipoCambio = await ObtenerTipoCambioAsync(monedaOrigen, monedaDestino);

            if (tipoCambio == null)
            {
                // Si no existe, lo crea.
                tipoCambio = new TipoCambio
                {
                    MonedaOrigenId = monedaOrigen,
                    MonedaDestinoId = monedaDestino,
                    Tasa = tasa,
                    FechaActualizacion = DateTime.Now
                };
                await _tipoCambioRepository.AddAsync(tipoCambio);
                // await _context.SaveChangesAsync(); // Llamar si el repositorio no guarda
            }
            else
            {
                // Si existe, actualiza la tasa y la fecha.
                tipoCambio.Tasa = tasa;
                tipoCambio.FechaActualizacion = DateTime.Now;
                _tipoCambioRepository.Update(tipoCambio);
                // await _context.SaveChangesAsync(); // Llamar si el repositorio no guarda
            }
        }

        public async Task<decimal> ConvertirMontoAsync(decimal monto, int monedaOrigen, int monedaDestino)
        {
            if (monedaOrigen == monedaDestino)
                return monto;

            var tasa = await ObtenerTasaCambioAsync(monedaOrigen, monedaDestino);
            return monto * tasa;
        }

        // --- IMPLEMENTACIONES DE MÉTODOS CRUD FALTANTES ---

        // Implementa: Task<IEnumerable<TipoCambio>> ObtenerTodosAsync()
        public async Task<IEnumerable<TipoCambio>> ObtenerTodosAsync()
        {
            return await _tipoCambioRepository.GetAllAsync(); // Llama al método correspondiente en el repositorio
        }

        // Implementa: Task<TipoCambio> ObtenerPorIdAsync(int id)
        public async Task<TipoCambio> ObtenerPorIdAsync(int id)
        {
            // Llama al método correspondiente en el repositorio
            return await _tipoCambioRepository.GetByIdAsync(id);
            // Nota: Tu interfaz devuelve Task<TipoCambio> (no nullable), pero GetByIdAsync podría devolver null.
            // Asegúrate de manejar posibles nulls en el controlador o cambiar el tipo de retorno en la interfaz a Task<TipoCambio?>.
            // Por ahora, mantenemos el tipo de retorno de la interfaz y asumimos que el controlador manejará el null.
        }

        // Implementa: Task AgregarAsync(TipoCambio tipoCambio)
        public async Task AgregarAsync(TipoCambio tipoCambio)
        {
            // Llama al método correspondiente en el repositorio.
            // Puedes añadir validaciones aquí si es necesario (ej. que no exista ya un tipo de cambio para el par de monedas).
            // if (await ObtenerTipoCambioAsync(tipoCambio.MonedaOrigenId, tipoCambio.MonedaDestinoId) != null)
            // {
            //     throw new ArgumentException("Ya existe un tipo de cambio para este par de monedas.");
            // }

            // Asegúrate de que la fecha de actualización se establezca (si no se hace en otro lado, ej. en la entidad o DB)
            if (tipoCambio.FechaActualizacion == default)
            {
                tipoCambio.FechaActualizacion = DateTime.Now;
            }

            await _tipoCambioRepository.AddAsync(tipoCambio);
            // await _context.SaveChangesAsync(); // Llamar si el repositorio no guarda
        }

        // Implementa: Task ActualizarAsync(TipoCambio tipoCambio) - Esta es la versión que recibe el objeto completo
        public async Task ActualizarAsync(TipoCambio tipoCambio)
        {
            // Llama al método correspondiente en el repositorio.
            // Es buena práctica verificar si la entidad existe antes de intentar actualizar.
            var existingTipoCambio = await _tipoCambioRepository.GetByIdAsync(tipoCambio.Id);
            if (existingTipoCambio == null)
            {
                throw new KeyNotFoundException($"Tipo de cambio con ID {tipoCambio.Id} no encontrado para actualizar.");
            }

            // Si usas AutoMapper en el controlador para mapear a 'existingTipoCambio', solo necesitas la siguiente línea.
            // Si el objeto 'tipoCambio' que recibes ya tiene los datos actualizados, solo llama al repositorio:
            _tipoCambioRepository.Update(tipoCambio);

            // Puedes actualizar la fecha aquí si quieres que refleje la última modificación desde el controlador.
            // tipoCambio.FechaActualizacion = DateTime.Now; // Si actualizas 'tipoCambio' antes de pasar al repo

            // await _context.SaveChangesAsync(); // Llamar si el repositorio no guarda
        }

        // Implementa: Task EliminarAsync(int id)
        public async Task EliminarAsync(int id)
        {
            // Llama al método correspondiente en el repositorio.
            // Basado en tu esquema DB, TipoCambio no tiene FKs apuntándole, así que no necesita verificar registros asociados en otras tablas.
            var tipoCambioToDelete = await _tipoCambioRepository.GetByIdAsync(id);
            if (tipoCambioToDelete == null)
            {
                // No se encontró para eliminar. Puedes lanzar excepción o simplemente terminar (si no lanza, asume que "ya no está").
                throw new KeyNotFoundException($"Tipo de cambio con ID {id} no encontrado para eliminar.");
                // O simplemente return;
            }

            _tipoCambioRepository.Delete(tipoCambioToDelete);
            // await _context.SaveChangesAsync(); // Llamar si el repositorio no guarda
        }
    }
}
