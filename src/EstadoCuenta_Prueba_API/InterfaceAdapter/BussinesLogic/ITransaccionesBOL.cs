using InterfaceAdapter.DTO.BussinesLogic;
using InterfaceAdapter.DTO.Response;

namespace InterfaceAdapter.BussinesLogic
{
    public interface ITransaccionesBOL
    {
        Task<List<TransaccionesDTO>> GetTransaccionesByIdTarjeta(int idTarjeta);
        Task<List<TransaccionesDTO>> CreateTransaccionByIdTarjeta(TransaccionesDTO transaccion);
        Task<List<TiposTransaccionesDTO>> GetTiposTransacciones();
    }
}
