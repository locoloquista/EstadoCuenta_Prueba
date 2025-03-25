using ViewModels;

namespace InterfaceAdapter.BussinesLogic
{
    public interface IClienteBOL
    {
        Task<List<ClienteViewModel>> GetAllCliente();
        Task<InformacionClienteViewModel> InformacionCliente(int idCliente);
    }
}
