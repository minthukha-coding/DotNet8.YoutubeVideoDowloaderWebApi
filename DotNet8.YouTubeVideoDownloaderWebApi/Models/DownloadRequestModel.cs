namespace DotNet8.YouTubeVideoDownloaderWebApi.Models;

public class DownloadRequestModel
{
    public string? OutputFilePath { get; set; }
    public List<string>? VideoUrls { get; set; }
}