using InterfaceAdapter.DTO.BussinesLogic;

namespace InterfaceAdapter.BussinesLogic
{
    public interface ITransaccionesBOL
    {
        Task<List<TransaccionesDTO>> GetTransaccionesByIdTarjeta(int idTarjeta);
    }
}
