using InterfaceAdapter.BussinesLogic;
using Microsoft.AspNetCore.Mvc;

namespace EstadoCuenta_Prueba_Web.Controllers
{
    public class EstadoCuentaController : Controller
    {
        private readonly IEstadoCuentaBOL _estadoCuentaBOL;

        public EstadoCuentaController(IEstadoCuentaBOL estadoCuentaBOL)
        {
            _estadoCuentaBOL = estadoCuentaBOL;
        }

        public async Task<IActionResult> DetalleEstadoCuenta(int tarjetaId)
        {
            var estadoCuenta = await _estadoCuentaBOL.DetalleEstadoCuenta(tarjetaId);
            return PartialView("_EstadoCuentaPartial", estadoCuenta);
        }
    }
}
