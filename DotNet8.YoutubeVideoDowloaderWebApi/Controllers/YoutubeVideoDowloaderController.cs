using DotNet8.YoutubeVideoDowloaderWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using YoutubeExplode;

namespace DotNet8.YoutubeVideoDownloaderWebApi.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class YoutubeVideoDownloaderController : ControllerBase
    {
        [HttpPost("download")]
        public async Task<IActionResult> DownloadVideos(DownloadRequestModel reqModel)
        {
            #region Check Requir Field

            if (reqModel.OutputFilePath is null)
            {
                return BadRequest("No video outputDirectory provided.");
            }

            if (reqModel.VideoUrls is null)
            {
                return BadRequest("No video URLs provided.");
            }

            #endregion

            #region Called Download YouTubeVideo Method To Dowload 

            try
            {
                foreach (var videoUrl in reqModel.VideoUrls)
                {
                    await DownloadYouTubeVideo(videoUrl, reqModel.OutputFilePath);
                }

                if (reqModel.VideoUrls.Count is 1)
                {
                    return Ok("Download completed video.");
                }
                return Ok("Download completed for all videos.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while downloading the videos: {ex.Message}");
            }

            #endregion

        }

        #region Download YouTube Video

        private async Task DownloadYouTubeVideo(string videoUrl, string outputDirectory)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoUrl);

            string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();

            if (muxedStreams.Any())
            {
                var streamInfo = muxedStreams.First();
                using var httpClient = new HttpClient();
                var stream = await httpClient.GetStreamAsync(streamInfo.Url);
                var datetime = DateTime.Now;

                string outputFilePath = Path.Combine(outputDirectory, $"{sanitizedTitle}.{streamInfo.Container}");
                using var outputStream = System.IO.File.Create(outputFilePath);
                await stream.CopyToAsync(outputStream);
            }
            else
            {
                throw new Exception($"No suitable video stream found for {video.Title}.");
            }

        }

        #endregion
    }
}
