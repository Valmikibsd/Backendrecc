namespace CrudOperations
{
    public class Response
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public string? Data { get; set; }
    }

    public class Response<T>
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    public class ResponseList<T>
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public List<T>? Data { get; set; }
        public int TotalRecords { get; set; }
    }

    public class DynamicResponse
    {
        public bool? Status { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, object> NamedDataSets { get; set; } = new Dictionary<string, object>();
    }
}
