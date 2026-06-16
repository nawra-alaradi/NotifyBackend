using System.Diagnostics.Contracts;

namespace NotifyBackend.Utils
{
    public class ApiResponse
    {
        public required bool Success { get; set; }
        public required string Message { get; set; }
        public required int DataCount { get; set; }
        public required dynamic? Data { get; set; }

        //public ApiResponse(bool success, string message, int count, dynamic data)
        //{
        //    Success = success;
        //    Message = message;
        //    DataCount = count;
        //    Data = data;
        //}

    }
}
