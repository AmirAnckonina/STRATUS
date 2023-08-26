namespace StratusApp.Models.Responses
{
    public class StratusResponse<T>
    {
        public StratusResponse() { }

        public StratusResponse(bool success, T data)
        {
            this.Success = success;
            this.Data = data;
        }

        public StratusResponse(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }

        public T? Data { get; set; }

        public bool Success { get; set; } = true;

        public string Message { get; set; } = string.Empty;

    }
}
