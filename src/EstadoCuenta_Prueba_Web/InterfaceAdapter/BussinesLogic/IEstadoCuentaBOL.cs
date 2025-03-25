using ViewModels;

namespace InterfaceAdapter.BussinesLogic
{
    public interface IEstadoCuentaBOL
    {
        public Task<EstadoCuentaViewModel> DetalleEstadoCuenta(int tarjetaId);
    }
}
