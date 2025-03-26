using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.PdfGeneration;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using ViewModels;

namespace EstadoCuenta_Prueba_Web.Controllers
{
    public class EstadoCuentaController : Controller
    {
        private readonly IEstadoCuentaBOL _estadoCuentaBOL; 
        private readonly IPdfGeneratorServices _pdfGeneratorServices;

        public EstadoCuentaController(IEstadoCuentaBOL estadoCuentaBOL, IPdfGeneratorServices pdfGeneratorServices)
        {
            _estadoCuentaBOL = estadoCuentaBOL;
            _pdfGeneratorServices = pdfGeneratorServices;
        }

        public async Task<IActionResult> EstadoCuentaCompleto(int tarjetaId)
        {
            var estadoCuenta = await _estadoCuentaBOL.EstadoCuentaCompleto(tarjetaId);

            int? idCliente = HttpContext.Session.GetInt32("idCliente");
            ViewBag.IdCliente = idCliente;
            ViewBag.TarjetaId = tarjetaId;

            return View(estadoCuenta);
        }

        [HttpGet]
        public async Task<IActionResult> ExportarEstadoCuentaAsync(int tarjetaId)
        {
            try
            {
                // Obtén los datos necesarios para la vista
                var estadoCuenta = await _estadoCuentaBOL.EstadoCuentaCompleto(tarjetaId);

                // Genera el PDF
                var pdf = await _pdfGeneratorServices.ExportReport(estadoCuenta, "_EstadoCuentaCompletoPdf");

                // Devuelve el PDF como archivo descargable
                return File(pdf, "application/pdf", "EstadoCuenta.pdf");
            }
            catch(Exception ex)
            {
                return View("EstadoCuentaCompleto", tarjetaId);
            }
        }
    }
}
