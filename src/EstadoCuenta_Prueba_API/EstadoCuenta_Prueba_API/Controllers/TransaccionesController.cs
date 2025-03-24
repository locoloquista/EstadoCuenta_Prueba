using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.DTO.BussinesLogic;
using InterfaceAdapter.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace EstadoCuenta_Prueba_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionesController : ControllerBase
    {
        private readonly ITransaccionesBOL _transaccionesBOL;

        public TransaccionesController(ITransaccionesBOL transaccionesBOL)
        {
            _transaccionesBOL = transaccionesBOL;
        }

        [HttpGet("GetTransaccionesByIdTarjeta")]
        public async Task<ResponseListMessage<TransaccionesDTO>> GetTransaccionesByIdTarjeta(int idTarjeta)
        {
            ResponseListMessage<TransaccionesDTO> response;

            try
            {
                var result = await _transaccionesBOL.GetTransaccionesByIdTarjeta(idTarjeta);

                response = new ResponseListMessage<TransaccionesDTO>("200: Success", true, result);
            }
            catch (Exception ex)
            {
                response = new ResponseListMessage<TransaccionesDTO>("500: Error " + ex.Message, false, new List<TransaccionesDTO>());
            }

            return response;
        }

        [HttpPost("CreateTransaccionByIdTarjeta")]
        public async Task<ResponseListMessage<TransaccionesDTO>> CreateTransaccionByIdTarjeta(TransaccionesDTO transaccion)
        {
            ResponseListMessage<TransaccionesDTO> response;

            try
            {
                var result = await _transaccionesBOL.CreateTransaccionByIdTarjeta(transaccion);

                response = new ResponseListMessage<TransaccionesDTO>("200: Success", true, result);
            }
            catch (Exception ex)
            {
                response = new ResponseListMessage<TransaccionesDTO>("500: Error " + ex.Message, false, new List<TransaccionesDTO>());
            }

            return response;
        }
    }
}
