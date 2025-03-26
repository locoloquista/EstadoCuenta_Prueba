using ViewModels;

namespace InterfaceAdapter.BussinesLogic
{
    public interface IEstadoCuentaBOL
    {
        Task<EstadoCuentaCompletoViewModel> EstadoCuentaCompleto(int tarjetaId);
    }
}
