using InterfaceAdapter.DTO.BussinesLogic;

namespace InterfaceAdapter.DataAccess
{
    public interface ITransaccionesDAO
    {
        Task<List<TransaccionesDTO>> GetTransaccionesByIdTarjeta(int idTarjeta);
        Task<int> CreateTransaccionByIdTarjeta(TransaccionesDTO transaccion);
        Task<List<TiposTransaccionesDTO>> GetTiposTransacciones();
    }
}
