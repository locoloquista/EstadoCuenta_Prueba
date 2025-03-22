using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.DTO.BussinesLogic;
using InterfaceAdapter.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace EstadoCuenta_Prueba_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarjetaCreditoController : ControllerBase
    {
        private readonly ITarjetaCreditoBOL _tarjetaCreditoBOL;
        public TarjetaCreditoController(ITarjetaCreditoBOL tarjetaCreditoBOL)
        {
            _tarjetaCreditoBOL = tarjetaCreditoBOL;
        }

        [HttpGet("GetTartejaCreditoByClienteId")]
        public async Task<ResponseListMessage<TarjetaCreditoDTO>> GetTartejaCreditoByClienteId(int IdCliente)
        {
            ResponseListMessage<TarjetaCreditoDTO> response;

            try
            {
                var result = await _tarjetaCreditoBOL.GetTartejaCreditoByClienteId(IdCliente);

                response = new ResponseListMessage<TarjetaCreditoDTO>("200: Success", true, result);
            }
            catch (Exception ex)
            {
                response = new ResponseListMessage<TarjetaCreditoDTO>("500: Error " + ex.Message, false, new List<TarjetaCreditoDTO>());
            }

            return response;
        }
    }
}
