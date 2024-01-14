namespace Kinashy.ArrestSearchWebAPI.Data
{
    public class PdfResult : IResult
    {
        private byte[] _data;
        public PdfResult(byte[] data) => _data = data;
        Task IResult.ExecuteAsync(HttpContext httpContext)
        {
            using var ms = new MemoryStream(_data);
            httpContext.Response.ContentType = "application/pdf";
            ms.Position = 0;
            return ms.CopyToAsync(httpContext.Response.Body);
        }
    }
    static class PdfResultExtension
    {
        public static IResult Pdf<T>(this IResultExtensions _, byte[] data) =>
            new PdfResult(data);
    }
}
