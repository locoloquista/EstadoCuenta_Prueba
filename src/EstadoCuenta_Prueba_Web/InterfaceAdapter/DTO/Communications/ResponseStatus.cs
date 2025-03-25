using System.Net;

namespace InterfaceAdapter.DTO.Communications
{
    public class ResponseStatus
    {
        public HttpStatusCode httpStatusCode { get; set; }
        public string Message { get; set; }
    }
}
