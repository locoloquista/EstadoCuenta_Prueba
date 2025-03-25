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

        public async Task<IActionResult> EstadoCuentaCompleto(int tarjetaId)
        {
            var estadoCuenta = await _estadoCuentaBOL.EstadoCuentaCompleto(tarjetaId);

            int? idCliente = HttpContext.Session.GetInt32("idCliente");
            ViewBag.IdCliente = idCliente;
            ViewBag.TarjetaId = tarjetaId;

            return View(estadoCuenta);
        }

        //public async Task<IActionResult> ExportarEstadoCuenta(int tarjetaId)
        //{
        //    var estadoCuenta = await _estadoCuentaBOL.EstadoCuentaCompleto(tarjetaId);

        //    int? idCliente = HttpContext.Session.GetInt32("idCliente");
        //    ViewBag.IdCliente = idCliente;
        //    ViewBag.TarjetaId = tarjetaId;

        //    // Genera el PDF
        //    var pdf = _pdfGeneratorServices.ExportReport(estadoCuenta, "_EstadoCuentaPartial");

        //    // Devuelve el PDF como archivo descargable
        //    return File(pdf, "application/pdf", "EstadoCuenta.pdf");
        //}
    }
}
