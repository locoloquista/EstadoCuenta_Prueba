namespace InterfaceAdapter.DTO.Response
{
    public class ResponseListMessage<T>
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public List<T> Items { get; set; }

        public ResponseListMessage(string message, bool isSuccess, List<T> item)
        {
            Message = message;
            IsSuccess = isSuccess;
            Items = item;
        }
    }
}
