namespace BTL.Areas.Admin.ViewModel
{
    public class Response
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public Response(bool success, string message = "")
        {
            Success = success;
            Message = message;
        }
        public static Response Ok() => new Response(true);
        public static Response Fail(string message) => new Response(false, message);
    }
    public class Response<T> : Response where T : class, new()
    {
        public T Data { get; private set; }
        public Response(bool success, string message = "") : base(success, message)
        {
        }
        public Response(T data, bool success, string message = "") : base(success, message)
        {
            Data = data;
        }

        public static Response<T> Ok(T data) => new Response<T>(data, true);
        public new static Response<T> Fail(string message) => new Response<T>(false, message);
    }
}