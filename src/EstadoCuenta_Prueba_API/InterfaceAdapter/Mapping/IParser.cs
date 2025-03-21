namespace InterfaceAdapter.Mapping
{
    public interface IParser
    {
        TResult Parse<TResult, TItem>(TItem mapping);
    }
}
