using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.DataAccess;
using InterfaceAdapter.DTO.BussinesLogic;

namespace BussinesLogic
{
    public class TarjetaCreditoBOL : ITarjetaCreditoBOL
    {
        private readonly ITarjetaCreditoDAO _tarjetaCreditoDAO;

        public TarjetaCreditoBOL(ITarjetaCreditoDAO tarjetaCreditoDAO)
        {
            _tarjetaCreditoDAO = tarjetaCreditoDAO;
        }

        public async Task<List<TarjetaCreditoDTO>> GetTartejaCreditoByClienteId(int IdCliente)
        {
            return await _tarjetaCreditoDAO.GetTartejaCreditoByClienteId(IdCliente);
        }
    }
}
