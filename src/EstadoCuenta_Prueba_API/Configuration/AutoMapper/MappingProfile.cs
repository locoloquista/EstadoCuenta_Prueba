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
            CreateMap<EstadoCuentaRepository, EstadoCuentaDTO>();
            CreateMap<TransaccionesRepository, TransaccionesDTO>();
            CreateMap<TiposTransaccionesRepository, TiposTransaccionesDTO>();
        }

        private void MappingDtoToRepository()
        {
            CreateMap<ClienteDTO, ClienteRepository>();
            CreateMap<TarjetaCreditoDTO, TarjetaCreditoRepository>();
            CreateMap<EstadoCuentaDTO, EstadoCuentaRepository>();
            CreateMap<TransaccionesDTO, TransaccionesRepository>();
            CreateMap<TiposTransaccionesDTO, TiposTransaccionesRepository>();
        }
    }
}
