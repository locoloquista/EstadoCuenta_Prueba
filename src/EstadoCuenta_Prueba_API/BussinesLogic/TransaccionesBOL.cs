using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.DataAccess;
using InterfaceAdapter.DTO.BussinesLogic;

namespace BussinesLogic
{
    public class TransaccionesBOL : ITransaccionesBOL
    {
        private readonly ITransaccionesDAO _transaccionesDAO;

        public TransaccionesBOL(ITransaccionesDAO transaccionesDAO)
        {
            _transaccionesDAO = transaccionesDAO;
        }

        public async Task<List<TransaccionesDTO>> GetTransaccionesByIdTarjeta(int idTarjeta)
        {
            return await _transaccionesDAO.GetTransaccionesByIdTarjeta(idTarjeta);
        }
    }
}
