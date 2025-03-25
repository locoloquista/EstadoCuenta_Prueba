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
        private readonly ITransaccionesBOL _transaccionesBOL;
        private readonly IParser _parser;

        public EstadoCuentaBOL(IEstadoCuentaServices estadoCuentaServices, ITransaccionesBOL transaccionesBOL, IParser parser)
        {
            _estadoCuentaServices = estadoCuentaServices;
            _transaccionesBOL = transaccionesBOL;
            _parser = parser;
        }

        public async Task<EstadoCuentaCompletoViewModel> EstadoCuentaCompleto(int tarjetaId)
        {
            EstadoCuentaCompletoViewModel estadoCompleto;
            try
            {
                EstadoCuentaDTO estadoCuenta = await _estadoCuentaServices.DetalleEstadoCuenta(tarjetaId);

                List<TransaccionesViewModel> transacciones = await _transaccionesBOL.GetTransacciones(tarjetaId);

                estadoCompleto = new EstadoCuentaCompletoViewModel
                {
                    EstadoCuenta = _parser.Parse<EstadoCuentaViewModel, EstadoCuentaDTO>(estadoCuenta),
                    Transacciones = transacciones
                };

                return estadoCompleto;
            }
            catch(Exception ex)
            {
                estadoCompleto = new EstadoCuentaCompletoViewModel
                {
                    EstadoCuenta = new EstadoCuentaViewModel(),
                    Transacciones = new List<TransaccionesViewModel>()
                };
            }

            return estadoCompleto;
        }
    }
}
