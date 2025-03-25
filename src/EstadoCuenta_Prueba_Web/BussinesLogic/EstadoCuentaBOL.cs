using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.ConsumerServices;
using InterfaceAdapter.DTO;
using InterfaceAdapter.Mapping;
using ViewModels;

namespace BussinesLogic
{
    public class EstadoCuentaBOL : IEstadoCuentaBOL
    {
        private readonly IEstadoCuentaServices _estadoCuentaServices;
        private readonly IParser _parser;

        public EstadoCuentaBOL(IEstadoCuentaServices estadoCuentaServices, IParser parser)
        {
            _estadoCuentaServices = estadoCuentaServices;
            _parser = parser;
        }
        public async Task<EstadoCuentaViewModel> DetalleEstadoCuenta(int tarjetaId)
        {
            EstadoCuentaDTO estadoCuenta = await _estadoCuentaServices.DetalleEstadoCuenta(tarjetaId);

            EstadoCuentaViewModel estadoCuentaViewModel = _parser.Parse<EstadoCuentaViewModel, EstadoCuentaDTO>(estadoCuenta);

            return estadoCuentaViewModel;
        }
    }
}
