namespace InterfaceAdapter.DTO.Response
{
    public class ResponseMessage<T>
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public T Item { get; set; }

        public ResponseMessage(string message, bool isSuccess, T item)
        {
            Message = message;
            IsSuccess = isSuccess;
            Item = item;
        }
    }
}
