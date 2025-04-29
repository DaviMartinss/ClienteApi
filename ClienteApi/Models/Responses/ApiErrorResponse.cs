using ClienteApi.Enums;

namespace ClienteApi.Models.Responses
{
    public class ApiErrorResponse
    {
        public ApiErrorCode ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
