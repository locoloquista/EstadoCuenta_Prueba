using InterfaceAdapter.DTO.BussinesLogic;

namespace InterfaceAdapter.DataAccess
{
    public interface IEstadoCuentaDAO
    {
        Task<EstadoCuentaDTO> GetEstadoCuenta(int IdTarjetaCredito);
    }
}
