using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.ConsumerServices;
using InterfaceAdapter.DTO;
using InterfaceAdapter.Mapping;
using ViewModels;

namespace BussinesLogic
{
    public class TarjetaCreditoBOL : ITarjetaCreditoBOL
    {
        private readonly ITarjetaCreditoServices _tarjetaCreditoServices;
        private readonly IParser _parser;

        public TarjetaCreditoBOL(ITarjetaCreditoServices tarjetaCreditoServices, IParser parser)
        {
            _tarjetaCreditoServices = tarjetaCreditoServices;
            _parser = parser;
        }

        public async Task<List<TarjetaCreditoViewModel>> GetTartejaCreditoByClienteId(int idCliente)
        {
            List<TarjetaCreditoDTO> result = await _tarjetaCreditoServices.GetTartejaCreditoByClienteId(idCliente);

            List<TarjetaCreditoViewModel> resultViewModel = _parser.Parse<List<TarjetaCreditoViewModel>, List<TarjetaCreditoDTO>>(result);

            return resultViewModel;
        }
    }
}
