using InterfaceAdapter.DTO;

namespace InterfaceAdapter.ConsumerServices
{
    public interface IEstadoCuentaServices
    {
        Task<EstadoCuentaDTO> DetalleEstadoCuenta(int tarjetaId);
    }
}
