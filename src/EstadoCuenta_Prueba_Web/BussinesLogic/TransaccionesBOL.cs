using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.ConsumerServices;
using InterfaceAdapter.DTO;
using InterfaceAdapter.Mapping;
using ViewModels;

namespace BussinesLogic
{
    public class TransaccionesBOL : ITransaccionesBOL
    {
        private readonly IParser _parser;
        private readonly ITransaccionesServices _transaccionesServices;

        public TransaccionesBOL(IParser parser, ITransaccionesServices transaccionesServices)
        {
            _parser = parser;
            _transaccionesServices = transaccionesServices;
        }

        public async Task<List<TransaccionesViewModel>> AgregarCompraPagoByTarjeta(TransaccionesViewModel model)
        {
            List<TransaccionDTO> transacciones = await _transaccionesServices.AgregarCompraPagoByTarjeta(
                                                _parser.Parse<TransaccionDTO, TransaccionesViewModel>(model)
                                                );

            List<TransaccionesViewModel> transaccionesViewModels = _parser.Parse<List<TransaccionesViewModel>, List<TransaccionDTO>>(transacciones);

            return transaccionesViewModels;
        }

        public async Task<List<TiposTransaccionesViewModel>> GetTiposTransacciones()
        {
            List<TiposTransaccionesDTO> tiposTransacciones = await _transaccionesServices.GetTiposTransacciones();

            List<TiposTransaccionesViewModel> tiposTransaccionesViewModels = _parser.Parse<List<TiposTransaccionesViewModel>, List<TiposTransaccionesDTO>>(tiposTransacciones);
        
            return tiposTransaccionesViewModels;
        }

        public async Task<List<TransaccionesViewModel>> GetTransacciones(int tarjetaId)
        {
            List<TransaccionDTO> transacciones = await _transaccionesServices.GetTransacciones(tarjetaId);

            List<TransaccionesViewModel> transaccionesViewModels = _parser.Parse<List<TransaccionesViewModel>, List<TransaccionDTO>>(transacciones);

            return transaccionesViewModels;
        }
    }
}
