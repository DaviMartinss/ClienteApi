using ClienteApi.Core.Domain.Enums;

namespace ClienteApi.Application.DTOs
{
    public class ApiErrorResponse
    {
        public ApiErrorCode ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
