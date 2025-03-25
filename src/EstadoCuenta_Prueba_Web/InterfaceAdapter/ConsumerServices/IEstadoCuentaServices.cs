using InterfaceAdapter.DTO;

namespace InterfaceAdapter.ConsumerServices
{
    public interface IEstadoCuentaServices
    {
        public Task<EstadoCuentaDTO> DetalleEstadoCuenta(int tarjetaId);
    }
}
