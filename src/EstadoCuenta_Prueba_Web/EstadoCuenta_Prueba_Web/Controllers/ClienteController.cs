using Microsoft.AspNetCore.Mvc;

namespace EstadoCuenta_Prueba_Web.Controllers
{
    public class ClienteController : Controller
    {
        public IActionResult ListadoClientes()
        {
            return View();
        }
    }
}
