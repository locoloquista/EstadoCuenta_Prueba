using Infrastructure.DataBase.Repository;
using InterfaceAdapter.DataAccess;
using InterfaceAdapter.DTO.BussinesLogic;
using InterfaceAdapter.Mapping;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.DataBase.DataAccess
{
    public class TransaccionesDAO : ITransaccionesDAO
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IParser _parser;
        public TransaccionesDAO(IUnitOfWork unitOfWork, IParser parser)
        {
            _unitOfWork = unitOfWork;
            _parser = parser;
        }

        public async Task<List<TransaccionesDTO>> GetTransaccionesByIdTarjeta(int idTarjeta)
        {
            try
            {
                var parameter = new SqlParameter("@TarjetaId", SqlDbType.BigInt)
                {
                    Value = idTarjeta
                };

                var result = await _unitOfWork.ExecuteStoredProcedureAsyncList<TransaccionesRepository>("ObtenerTransaccionesDelMes", parameter);
                return _parser.Parse<List<TransaccionesDTO>, List<TransaccionesRepository>>((List<TransaccionesRepository>)result);

            }
            catch (Exception ex)
            {
                return await Task.FromResult(new List<TransaccionesDTO>());
            }
        }
    }
}
