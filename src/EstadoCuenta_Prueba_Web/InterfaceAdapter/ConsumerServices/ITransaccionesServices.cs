using InterfaceAdapter.DTO;

namespace InterfaceAdapter.ConsumerServices
{
    public interface ITransaccionesServices
    {
        public Task<List<TransaccionDTO>> GetTransacciones(int tarjetaId);
    }
}
