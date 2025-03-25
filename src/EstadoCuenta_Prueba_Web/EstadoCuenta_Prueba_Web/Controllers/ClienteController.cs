using InterfaceAdapter.BussinesLogic;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace EstadoCuenta_Prueba_Web.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteBOL _clienteBOL;

        public ClienteController(IClienteBOL clienteBOL)
        {
            _clienteBOL = clienteBOL;
        }

        public async Task<IActionResult> ListadoClientes()
        {
            List<ClienteViewModel> clientes = await _clienteBOL.GetAllCliente();

            return View(clientes);
        }

        public async Task<IActionResult> DetalleCliente(int idCliente)
        {
            InformacionClienteViewModel cliente = await _clienteBOL.InformacionCliente(idCliente);
            return View(cliente);
        }
    }
}
