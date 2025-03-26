using InterfaceAdapter.DTO;

namespace InterfaceAdapter.ConsumerServices
{
    public interface ITransaccionesServices
    {
        Task<List<TransaccionDTO>> GetTransacciones(int tarjetaId);
        Task<List<TiposTransaccionesDTO>> GetTiposTransacciones();
        Task<List<TransaccionDTO>> AgregarCompraPagoByTarjeta(TransaccionDTO model);
    }
}
