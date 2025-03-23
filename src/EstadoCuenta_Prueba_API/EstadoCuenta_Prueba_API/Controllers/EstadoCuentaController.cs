using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.DTO.BussinesLogic;
using InterfaceAdapter.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace EstadoCuenta_Prueba_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoCuentaController : ControllerBase
    {
        private readonly IEstadoCuentaBOL _estadoCuentaBOL;

        public EstadoCuentaController(IEstadoCuentaBOL estadoCuentaBOL)
        {
            _estadoCuentaBOL = estadoCuentaBOL;
        }

        [HttpGet("GetEstadoCuentaByIdTarjeta")]
        public async Task<ResponseMessage<EstadoCuentaDTO>> GetEstadoCuenta(int IdTarjetaCredito)
        {
            ResponseMessage<EstadoCuentaDTO> response;
            try
            {
                var result = await _estadoCuentaBOL.GetEstadoCuenta(IdTarjetaCredito);
                response = new ResponseMessage<EstadoCuentaDTO>("200: Success", true, result);
            }
            catch (Exception ex)
            {
                response = new ResponseMessage<EstadoCuentaDTO>("500: Error " + ex.Message, false, new EstadoCuentaDTO());
            }
            return response;
        }
    }
}
