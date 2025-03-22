using Infrastructure.DataBase.Repository;
using InterfaceAdapter.DataAccess;
using InterfaceAdapter.DTO.BussinesLogic;
using InterfaceAdapter.Mapping;
using Microsoft.Data.SqlClient;
using System.Data;

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

        public async Task<ClienteDTO> GetClientebyId(int idCliente)
        {
            try
            {
                var parameter = new SqlParameter("@ClienteId", SqlDbType.BigInt)
                {
                    Value = idCliente
                };

                var result = await _unitOfWork.ExecuteStoredProcedureAsync<ClienteRepository>("ObtenerInformacionClientePorId", parameter);
                return _parser.Parse<ClienteDTO, ClienteRepository>(result);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ClienteDTO());
            }
        }
    }
}
