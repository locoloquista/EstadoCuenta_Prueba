using AutoMapper;
using Infrastructure.DataBase.Repository;
using InterfaceAdapter.DTO.BussinesLogic;

namespace Configuration.AutoMapper
{
    class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MappingRepositorytoDto();
            MappingDtoToRepository();
        }

        private void MappingRepositorytoDto()
        {
            CreateMap<ClienteRepository, ClienteDTO>();
            CreateMap<TarjetaCreditoRepository, TarjetaCreditoDTO>();
        }

        private void MappingDtoToRepository()
        {
            CreateMap<ClienteDTO, ClienteRepository>();
            CreateMap<TarjetaCreditoDTO, TarjetaCreditoRepository>();
        }
    }
}
