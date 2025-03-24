using InterfaceAdapter.DTO.BussinesLogic;

namespace InterfaceAdapter.DataAccess
{
    public interface ITransaccionesDAO
    {
        Task<List<TransaccionesDTO>> GetTransaccionesByIdTarjeta(int idTarjeta);
    }
}
