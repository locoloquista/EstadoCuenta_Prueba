using Infrastructure.DataBase.Repository;
using InterfaceAdapter.DataAccess;
using InterfaceAdapter.DTO.BussinesLogic;
using InterfaceAdapter.Mapping;

namespace Infrastructure.DataBase.DataAccess
{
    public class ClienteDAO : IClienteDAO
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IParser _parser;
        public ClienteDAO(IUnitOfWork unitOfWork, IParser parser)
        {
            _unitOfWork = unitOfWork;
            _parser = parser;
        }

        public async Task<List<ClienteDTO>> GetAllCliente()
        {
            try
            {
               var result = await _unitOfWork.ExecuteStoredProcedureAsyncList<ClienteRepository>("ObtenerInformacionClientes");
                return _parser.Parse<List<ClienteDTO>, List<ClienteRepository>>((List<ClienteRepository>)result);

            }
            catch (Exception ex)
            {
                return await Task.FromResult(new List<ClienteDTO>());
            }
        }
    }
}
