using AutoMapper;

namespace Configuration.AutoMapper
{
    class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MappingEntitytoDto();
        }

        private void MappingEntitytoDto()
        {
            //CreateMap<ClaseOrigen, ClaseDestino>();
        }
    }
}
