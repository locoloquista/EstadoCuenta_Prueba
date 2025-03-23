using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.DataAccess;
using InterfaceAdapter.DTO.BussinesLogic;

namespace BussinesLogic
{
    public class EstadoCuentaBOL : IEstadoCuentaBOL
    {
        private readonly IEstadoCuentaDAO _estadoCuentaDAO;

        public EstadoCuentaBOL(IEstadoCuentaDAO estadoCuentaDAO)
        {
            _estadoCuentaDAO = estadoCuentaDAO;
        }

        public async Task<EstadoCuentaDTO> GetEstadoCuenta(int IdTarjetaCredito)
        {
            return await _estadoCuentaDAO.GetEstadoCuenta(IdTarjetaCredito);
        }
    }
}
