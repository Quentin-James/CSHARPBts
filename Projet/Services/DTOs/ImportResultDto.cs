namespace Services.DTOs
{
    public class ImportResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ImportedCount { get; set; }
        public List<string> Errors { get; set; } = new();
        public bool HasErrors => Errors.Count > 0;
    }
} 