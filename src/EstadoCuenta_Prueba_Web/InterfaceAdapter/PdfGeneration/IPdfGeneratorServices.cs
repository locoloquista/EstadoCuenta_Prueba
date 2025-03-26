namespace InterfaceAdapter.PdfGeneration
{
    public interface IPdfGeneratorServices
    {
        Task<byte[]> ExportReport<T>(T model, string viewName);
    }
}
