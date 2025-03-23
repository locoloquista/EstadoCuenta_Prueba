using Infrastructure.DataBase.Repository;
using InterfaceAdapter.DataAccess;
using InterfaceAdapter.DTO.BussinesLogic;
using InterfaceAdapter.Mapping;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.DataBase.DataAccess
{
    public class EstadoCuentaDAO : IEstadoCuentaDAO
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IParser _parser;

        public EstadoCuentaDAO(IUnitOfWork unitOfWork, IParser parser)
        {
            _unitOfWork = unitOfWork;
            _parser = parser;
        }

        public async Task<EstadoCuentaDTO> GetEstadoCuenta(int IdTarjetaCredito)
        {
            try
            {
                var parameter = new SqlParameter("@TarjetaId", SqlDbType.BigInt)
                {
                    Value = IdTarjetaCredito
                };
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<EstadoCuentaRepository>("ObtenerEstadoCuenta", parameter);
                return _parser.Parse<EstadoCuentaDTO, EstadoCuentaRepository>(result);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new EstadoCuentaDTO());
            }
        }

    }
}
