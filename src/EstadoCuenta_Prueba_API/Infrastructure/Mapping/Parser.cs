using AutoMapper;
using InterfaceAdapter.Mapping;

namespace Infrastructure.Mapping
{
    public class Parser : IParser
    {
        private readonly IMapper _mapper;

        public Parser(IMapper mapper) 
        {
            _mapper = mapper;
        }

        public TResult Parse<TResult, TItem>(TItem mapping)
        {
            return _mapper.Map<TResult>(mapping);
        }
    }
}
