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

        public async Task<int> CreateTransaccionByIdTarjeta(TransaccionesDTO transaccion)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@TarjetaId", SqlDbType.BigInt) { Value = Convert.ToInt32(transaccion.TarjetaId) },
                    new SqlParameter("@FechaTransaccion", SqlDbType.Date) { Value = transaccion.Fecha },
                    new SqlParameter("@Descripcion", SqlDbType.NVarChar, 255) { Value = transaccion.Descripcion },
                    new SqlParameter("@Monto", SqlDbType.Decimal) { Value = transaccion.Monto },
                    new SqlParameter("@TipoTransaccionId", SqlDbType.BigInt) { Value = Convert.ToInt32(transaccion.TipoTransaccion) },
                    new SqlParameter("@Usuario", SqlDbType.NVarChar, 255) { Value = "UsuarioWeb" }
                };

                var result = await _unitOfWork.ExecuteStoredProcedureAsync<int>("AgregarTransaccion", parameters);
                return result;
            }
            catch (Exception ex)
            {
                // Manejar la excepción según sea necesario
                return await Task.FromResult(0);
            }
        }

        public async Task<List<TiposTransaccionesDTO>> GetTiposTransacciones()
        {
            try
            {
                var result = await _unitOfWork.ExecuteStoredProcedureAsyncList<TiposTransaccionesRepository>("ObtenerTiposTransacciones");
                return _parser.Parse<List<TiposTransaccionesDTO>, List<TiposTransaccionesRepository>>((List<TiposTransaccionesRepository>)result);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new List<TiposTransaccionesDTO>());
            }
        }
    }
}
