using InterfaceAdapter.DTO.BussinesLogic;

namespace InterfaceAdapter.BussinesLogic
{
    public interface IEstadoCuentaBOL
    {
        Task<EstadoCuentaDTO> GetEstadoCuenta(int IdTarjetaCredito);
    }
}
