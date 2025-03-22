using Infrastructure.DataBase.Repository;
using InterfaceAdapter.DataAccess;
using InterfaceAdapter.DTO.BussinesLogic;
using InterfaceAdapter.Mapping;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.DataBase.DataAccess
{
    public class TarjetaCreditoDAO : ITarjetaCreditoDAO
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IParser _parser;

        public TarjetaCreditoDAO(IUnitOfWork unitOfWork, IParser parser)
        {
            _unitOfWork = unitOfWork;
            _parser = parser;
        }

        public async Task<List<TarjetaCreditoDTO>> GetTartejaCreditoByClienteId(int IdCliente)
        {
            try
            {
                var parameter = new SqlParameter("@ClienteId", SqlDbType.BigInt)
                {
                    Value = IdCliente
                };

                var result = await _unitOfWork.ExecuteStoredProcedureAsyncList<TarjetaCreditoRepository>("ObtenerInformacionTarjetasPorClienteId", parameter);
                return _parser.Parse<List<TarjetaCreditoDTO>, List<TarjetaCreditoRepository>>((List<TarjetaCreditoRepository>)result);

            }
            catch (Exception ex)
            {
                return await Task.FromResult(new List<TarjetaCreditoDTO>());
            }
        }
    }
}
