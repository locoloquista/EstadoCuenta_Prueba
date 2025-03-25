namespace InterfaceAdapter.DTO.Communications
{
    public class GenericResponse<T>
    {
        public ResponseStatus Status { get; set; }
        public T Item { get; set; }
    }
}
