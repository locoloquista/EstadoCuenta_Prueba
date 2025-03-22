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

        // GET api/<ClienteController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ClienteController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ClienteController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ClienteController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
