namespace health_app.Common
{
    public class ErrorEnvelope
    {
        public int Status { get; set; }
        public string Code { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public IDictionary<string, string[]>? Errors { get; set; }
        public string TraceId { get; set; } = default!;
        public string Path { get; set; } = default!;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}
