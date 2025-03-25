using InterfaceAdapter.BussinesLogic;
using Microsoft.AspNetCore.Mvc;

namespace EstadoCuenta_Prueba_Web.Controllers
{
    public class TransaccionesController : Controller
    {
        readonly ITransaccionesBOL _transaccionesBOL;

        public TransaccionesController(ITransaccionesBOL transaccionesBOL)
        {
            _transaccionesBOL = transaccionesBOL;
        }

        public async Task<IActionResult> ListadoTransacciones(int tarjetaId)
        {
            var transacciones = await _transaccionesBOL.GetTransacciones(tarjetaId);
            return PartialView("_TransaccionesPartial", transacciones);
        }
    }
}
