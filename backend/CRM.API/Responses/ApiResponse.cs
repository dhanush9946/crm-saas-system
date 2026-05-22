namespace CRM.API.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; } = default!;
        public string TraceId { get; set; } = string.Empty;

        
        public static ApiResponse<T> SuccessResponse(T data,string traceId)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                TraceId = traceId
            };
        }
    }
}
