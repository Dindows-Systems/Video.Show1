using System;
using System.Configuration;
using System.IO;
using System.Threading;
using VideoShow.Streaming.Services;

namespace VideoShow.Utility
{
    public class VideoProcessor
    {
        public static void Process(string mediaFilename, string filesetName, int videoId)
        {
            //TODO: Determine working directory from System.Web.Hosting.HostingEnvironment.MapPath();
            string workingDirectory = new System.IO.FileInfo(mediaFilename).DirectoryName;
            VideoEncoderService encoder = new VideoEncoderService();
            encoder.MediaEncoderPath = ConfigurationManager.AppSettings["MediaEncoderPath"];
            string encodedOutput = encoder.Encode(mediaFilename, new Guid(filesetName), workingDirectory);

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(mediaFilename);
            string encodedVideoFilename = Path.ChangeExtension(Path.Combine(encodedOutput, fileInfo.Name),".wmv");
            string generatedThumbnailFilename = Path.ChangeExtension(encodedVideoFilename, null) + "_Thumb.jpg";
            
            if (File.Exists(encodedVideoFilename))
            {
                string thumbnailUrl = "/Silverlight/VideoGrid/images/thumbnail_placeholder.png";
                if (File.Exists(generatedThumbnailFilename))
                {
                    string thumbnailDirectory = workingDirectory.Replace("_temp_upload", "Images\\Thumbnails");
                    string thumbnailFilename = Path.Combine(thumbnailDirectory, new FileInfo(generatedThumbnailFilename).Name);
                    File.Copy(generatedThumbnailFilename, thumbnailFilename, true);
                    thumbnailUrl = "/images/thumbnails/" + (new FileInfo(thumbnailFilename).Name);
                }
                SilverlightStreamingUpload.Upload(encodedVideoFilename, filesetName, videoId, workingDirectory, thumbnailUrl);
            }
        }

        public static void StartBackgroundThread(System.Threading.ThreadStart threadStart)
        {
            if (threadStart != null)
            {
                Thread thread = new Thread(threadStart);
                thread.IsBackground = true;
                thread.Start();
            }
        }
    }
}
