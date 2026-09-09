using Org.BouncyCastle.Asn1;

namespace Portifolio.Server.DTOs
{
    public class BaseResponse<T> : BaseResponse
    {
        public T? Data { get; } = default(T);
        public BaseResponse(int statusCode,T? data = default, string? message = null) : base(statusCode,message)
        {
            Data = data;
        }
    }
    public class BaseResponse
    {
        public string Message { get; } = string.Empty;
        public int StatusCode { get; }
        public BaseResponse(int statusCode, string? message = null)
        {
            Message = message ?? string.Empty;
            StatusCode = statusCode;
        }
    }
}
