namespace InterfaceAdapter.DTO.Communications
{
    public class GenericListResponse<T>
    {
        public string message { get; set; }
        public bool isSuccess { get; set; }
        public List<T> Items { get; set; }
    }
}
