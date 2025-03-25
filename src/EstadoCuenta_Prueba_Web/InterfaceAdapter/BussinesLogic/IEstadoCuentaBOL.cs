using ViewModels;

namespace InterfaceAdapter.BussinesLogic
{
    public interface IEstadoCuentaBOL
    {
        public Task<EstadoCuentaCompletoViewModel> EstadoCuentaCompleto(int tarjetaId);
    }
}
