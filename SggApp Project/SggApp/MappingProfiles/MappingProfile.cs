using AutoMapper;
using SggApp.DAL.Entidades;
using SggApp.Models; 

namespace SggApp.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Usuario mappings
            CreateMap<Usuario, UsuarioViewModel>()
                .ForMember(dest => dest.MonedaPredeterminadaNombre, opt => opt.MapFrom(src =>
                    src.MonedaPredeterminada != null
                        ? $"{src.MonedaPredeterminada.Nombre} ({src.MonedaPredeterminada.Codigo})"
                        : "No definido"));

            CreateMap<RegistroViewModel, Usuario>()
                .ForMember(dest => dest.MonedaPredeterminadaId, opt => opt.MapFrom(src => src.MonedaPredeterminadaId))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => System.DateTime.Now))
                .ForMember(dest => dest.MonedaPredeterminada, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<Usuario, PerfilViewModel>()
                .ForMember(dest => dest.MonedaPredeterminadaId, opt => opt.MapFrom(src => src.MonedaPredeterminada.Id));

            // Categoría mappings
            CreateMap<Categoria, CategoriaViewModel>().ReverseMap();

            // Moneda mappings
            CreateMap<Moneda, MonedaViewModel>().ReverseMap();

            // Gasto mappings
            CreateMap<Gasto, GastoViewModel>()
                .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria.Nombre))
                .ForMember(dest => dest.MonedaCodigo, opt => opt.MapFrom(src => src.Moneda.Codigo));

            CreateMap<GastoViewModel, Gasto>().ReverseMap();
            CreateMap<GastoFormViewModel, Gasto>().ReverseMap();

            // Presupuesto mappings
            CreateMap<Presupuesto, PresupuestoViewModel>()
                .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src =>
                    src.Categoria != null ? src.Categoria.Nombre : "General"))
                .ForMember(dest => dest.MonedaCodigo, opt => opt.MapFrom(src => src.Moneda.Codigo))
                .ForMember(dest => dest.MontoGastado, opt => opt.Ignore())
                .ForMember(dest => dest.PorcentajeUtilizado, opt => opt.Ignore());

            CreateMap<PresupuestoViewModel, Presupuesto>().ReverseMap();
            CreateMap<PresupuestoFormViewModel, Presupuesto>().ReverseMap();



            // Mapeo de Entidad (TipoCambio) a ViewModel (TipoCambioViewModel) para mostrar datos
            CreateMap<TipoCambio, TipoCambioViewModel>()
                .ForMember(dest => dest.MonedaOrigenCodigo, opt => opt.MapFrom(src => src.MonedaOrigen.Codigo))
                .ForMember(dest => dest.MonedaDestinoCodigo, opt => opt.MapFrom(src => src.MonedaDestino.Codigo))
                .ForMember(dest => dest.Tasa, opt => opt.MapFrom(src => src.Tasa)); // 

            // Mapeo de FormViewModel (TipoCambioFormViewModel) a Entidad (TipoCambio) para crear/editar
            CreateMap<TipoCambioFormViewModel, TipoCambio>()
                .ForMember(dest => dest.Tasa, opt => opt.MapFrom(src => src.Tasa)) 
                .ForMember(dest => dest.MonedaOrigen, opt => opt.Ignore())
                .ForMember(dest => dest.MonedaDestino, opt => opt.Ignore());

            // Mapeo de Entidad (TipoCambio) a FormViewModel (TipoCambioFormViewModel) para precargar formularios de edición
            CreateMap<TipoCambio, TipoCambioFormViewModel>()
                .ForMember(dest => dest.MonedaOrigenId, opt => opt.MapFrom(src => src.MonedaOrigenId))
                .ForMember(dest => dest.MonedaDestinoId, opt => opt.MapFrom(src => src.MonedaDestinoId))
                .ForMember(dest => dest.Tasa, opt => opt.MapFrom(src => src.Tasa)) 
                .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(src => src.FechaActualizacion));
        }
    }
}
