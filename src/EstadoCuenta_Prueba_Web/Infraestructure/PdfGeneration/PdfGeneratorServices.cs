using InterfaceAdapter.PdfGeneration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Rotativa.AspNetCore;

namespace Infraestructure.PdfGeneration
{
    public class PdfGeneratorServices : IPdfGeneratorServices
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public PdfGeneratorServices(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public async Task<byte[]> ExportReport<T>(T model, string viewName)
        {
            try
            {
                var actionContext = GetActionContext();


                var viewResult = new ViewAsPdf(viewName, model)
                {
                    FileName = "Report.pdf",
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
                };

                var pdfBytes = await viewResult.BuildFile(actionContext);

                return pdfBytes;
            }
            catch(Exception ex)
            {
                return new byte[0];
            }
            
        }

        private ActionContext GetActionContext()
        {
            try
            {
                var httpContext = new ActionContext(_contextAccessor.HttpContext, _contextAccessor.HttpContext.GetRouteData(),
                                                        new ActionDescriptor());

                return httpContext;
            }
            catch(Exception ex)
            {
                return new ActionContext();
            }
            
        }
    }
}
