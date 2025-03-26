using InterfaceAdapter.BussinesLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ViewModels;

namespace EstadoCuenta_Prueba_Web.Controllers
{
    public class TransaccionesController : Controller
    {
        readonly ITransaccionesBOL _transaccionesBOL;

        public TransaccionesController(ITransaccionesBOL transaccionesBOL)
        {
            _transaccionesBOL = transaccionesBOL;
        }

        [HttpGet]
        public IActionResult AgregarCompraPagoByTarjeta(int tarjetaId)
        {
            CargarVistaAgregarCompraPago(tarjetaId);

            return View(new TransaccionesViewModel { TarjetaId = tarjetaId });
        }

        [HttpPost]
        public async Task<IActionResult> AgregarCompraPagoByTarjeta(TransaccionesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CargarVistaAgregarCompraPago(model.TarjetaId);
                return View(model);
            }

            var resultado = await _transaccionesBOL.AgregarCompraPagoByTarjeta(model);

            if(!(resultado.Count > 0))
            {
                CargarVistaAgregarCompraPago(model.TarjetaId);
                return View(model);
            }

            return RedirectToAction("EstadoCuentaCompleto", "EstadoCuenta", new { tarjetaId = model.TarjetaId });
        }

        private void CargarVistaAgregarCompraPago(int tarjetaId)
        {
            try
            {
                int? idCliente = HttpContext.Session.GetInt32("idCliente");

                var tiposTransacciones = _transaccionesBOL.GetTiposTransacciones()
                .Result
                .Select(t => new SelectListItem { Value = t.TransaccionId.ToString(), Text = t.TipoTransaccion })
                .ToList();

                ViewBag.idCliente = idCliente;
                ViewBag.TarjetaId = tarjetaId;
                ViewBag.TiposTransacciones = tiposTransacciones;
            }
            catch (Exception ex)
            {
                RedirectToAction("ListadoClientes", "Cliente");
            }

        }
    }
}
