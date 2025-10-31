namespace ChatApp.Shared.Data.BlobStorage;

public class FileUploadResult
{
    public string? Status { get; set; }
    public bool IsError { get; set; }
    public string? StackTrace { get; set; }
    public string? FileName { get; set; }
    public string? FileUri { get; set; }
}