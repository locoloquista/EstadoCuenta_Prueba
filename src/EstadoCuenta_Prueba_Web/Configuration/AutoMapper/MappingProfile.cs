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
        }

        private void MappingDtoToViewModel()
        {
            CreateMap<ClienteDTO, ClienteViewModel>();
            CreateMap<TarjetaCreditoDTO, TarjetaCreditoViewModel>();
        }
    }
}
