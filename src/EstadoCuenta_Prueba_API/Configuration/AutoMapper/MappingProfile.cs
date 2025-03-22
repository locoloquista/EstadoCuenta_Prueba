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
        }

        private void MappingDtoToRepository()
        {
            CreateMap<ClienteDTO, ClienteRepository>();
        }
    }
}
