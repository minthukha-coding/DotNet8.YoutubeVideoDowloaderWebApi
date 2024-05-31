namespace DotNet8.YoutubeVideoDowloaderWebApi.Models;

public class DownloadRequestModel
{
    public string OutputFilePath { get; set; }
    public List<string> VideoUrls { get; set; }
}