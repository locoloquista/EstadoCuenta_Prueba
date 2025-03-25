using AutoMapper;
using InterfaceAdapter.DTO;
using ViewModels;
namespace Configuration.AutoMapper
{
    class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MappingViewModeltoDto();
            MappingDtoToViewModel();
        }

        private void MappingViewModeltoDto()
        {
            CreateMap<ClienteViewModel, ClienteDTO>();
            CreateMap<TarjetaCreditoViewModel, TarjetaCreditoDTO>();
            CreateMap<TransaccionesViewModel, TransaccionDTO>();
            CreateMap<EstadoCuentaViewModel, EstadoCuentaDTO>();
        }

        private void MappingDtoToViewModel()
        {
            CreateMap<ClienteDTO, ClienteViewModel>();
            CreateMap<TarjetaCreditoDTO, TarjetaCreditoViewModel>();
            CreateMap<TransaccionDTO, TransaccionesViewModel>()
                .ForMember(dest => dest.FechaFormateada, opt => opt.Ignore())
                .ForMember(dest => dest.MontoFormateado, opt => opt.Ignore());
            CreateMap<EstadoCuentaDTO, EstadoCuentaViewModel>()
                .ForMember(dest => dest.LimiteCreditoFormateado, opt => opt.Ignore())
                .ForMember(dest => dest.SaldoDisponibleFormateado, opt => opt.Ignore())
                .ForMember(dest => dest.SaldoTotalFormateado, opt => opt.Ignore())
                .ForMember(dest => dest.ComprasMesAnteriorFormateado, opt => opt.Ignore())
                .ForMember(dest => dest.ComprasMesActualFormateado, opt => opt.Ignore())
                .ForMember(dest => dest.InteresBonificableFormateado, opt => opt.Ignore())
                .ForMember(dest => dest.CuotaMinimaFormateado, opt => opt.Ignore())
                .ForMember(dest => dest.PagoContadoConInteresesFormateado, opt => opt.Ignore())
                .ForMember(dest => dest.TasaInteresFormateada, opt => opt.Ignore());
        }
    }
}
