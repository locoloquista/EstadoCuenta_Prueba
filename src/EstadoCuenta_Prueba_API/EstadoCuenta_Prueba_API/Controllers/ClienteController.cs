using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.DTO.BussinesLogic;
using InterfaceAdapter.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace EstadoCuenta_Prueba_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        readonly IClienteBOL _clienteBOL;

        public ClienteController(IClienteBOL clienteBOL)
        {
            _clienteBOL = clienteBOL;
        }

        [HttpGet("GetAllCliente")]
        public async Task<ResponseListMessage<ClienteDTO>> GetAllCliente()
        {
            ResponseListMessage<ClienteDTO> response;

            try
            {
                var result = await _clienteBOL.GetAllCliente();

                response = new ResponseListMessage<ClienteDTO>("200: Success", true, result);
            }
            catch (Exception ex)
            {
                response = new ResponseListMessage<ClienteDTO>("500: Error " + ex.Message, false, new List<ClienteDTO>());
            }

            return response;
        }

        [HttpGet("GetClientebyId")]
        public async Task<ResponseMessage<ClienteDTO>> GetClientebyId(int idCliente)
        {
            ResponseMessage<ClienteDTO> response;

            try
            {
                var result = await _clienteBOL.GetClientebyId(idCliente);

                response = new ResponseMessage<ClienteDTO>("200: Success", true, result);
            }
            catch (Exception ex)
            {
                response = new ResponseMessage<ClienteDTO>("500: Error " + ex.Message, false, new ClienteDTO());
            }

            return response;
        }
    }
}
