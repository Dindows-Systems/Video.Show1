using System;
using System.Configuration;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Web;
using VideoShow.Data;
using VideoShow.Data.DataServices;

namespace VideoShow.Utility
{

    public static class SilverlightStreamingUpload
    {
        private static string ACCOUNTID
        { get { return ConfigurationManager.AppSettings["AccountId"].Trim(); } }

        private static string KEY
        { get { return ConfigurationManager.AppSettings["AccountKey"].Trim(); } }
        const long BUFFER_SIZE = 8192;

        private static void AddToZip(Package zip, string localFileName)
        {
            //Gets only filename, its will be the name in zip file
            string destFilename = ".\\" + Path.GetFileName(localFileName);
            Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
            PackagePart part = zip.CreatePart(uri, "");
            // Copy the data to the Document Part
            using (FileStream fileStream = new FileStream(localFileName, FileMode.Open, FileAccess.Read))
            {
                using (Stream dest = part.GetStream())
                {
                    CopyStream(fileStream, dest);
                }
            }
        }

        private static string ZipDir(string videoFileName, string workingDirectory)
        {
            string zipFilename = Path.GetTempFileName();
            //deletes tmp file
            if (File.Exists(zipFilename))
                File.Delete(zipFilename);
            //rename it to zip
            zipFilename = Path.ChangeExtension(zipFilename, "zip");
            if (File.Exists(zipFilename))
                File.Delete(zipFilename);
            using (Package zip = System.IO.Packaging.Package.Open(zipFilename, FileMode.Create))
            {
                AddToZip(zip, videoFileName);
                //Adds silverlight manifest file
                AddToZip(zip, System.IO.Path.Combine(workingDirectory, "manifest.xml"));
            }
            return zipFilename;
        }

        public static void Upload(string mediaFilename, string filesetName, int videoId, string workingDirectory, string thumbnailUrl)
        {
            string zipFilename = mediaFilename + ".zip";
            zipFilename = ZipDir(mediaFilename, workingDirectory);
            string filename = new System.IO.FileInfo(mediaFilename).Name;

            Uri silverlightStreamingUri = new Uri(string.Format("http://silverlight.services.live.com/{0}/{1}", ACCOUNTID, filesetName));
            System.Net.HttpWebRequest request = System.Net.WebRequest.Create(silverlightStreamingUri) as HttpWebRequest;
            request.Timeout = 20 * 60 * 1000;
            request.ReadWriteTimeout = 20 * 60 * 1000;

            //Note: Can't set authorization header with NetworkCredential or larger files time out
            //request.Credentials = new NetworkCredential(ACCOUNTID, KEY);
            byte[] userPass = System.Text.Encoding.Default.GetBytes(ACCOUNTID + ":" + KEY);
            string basic = "Basic " + Convert.ToBase64String(userPass);
            request.Headers["Authorization"] = basic; 

            request.PreAuthenticate = true;
            request.ContentType = System.Net.Mime.MediaTypeNames.Application.Zip;
            request.Method = WebRequestMethods.Http.Post;

            using (System.IO.FileStream inputStream = new System.IO.FileStream(zipFilename, FileMode.Open))
            {
                using (System.IO.Stream requestStream = request.GetRequestStream())
                {
                    CopyStream(inputStream, requestStream);
                    requestStream.Close();
                }
            }

            //Delete temp files in release mode.
            #if !DEBUG
            {
                System.IO.File.Delete(mediaFilename);
                System.IO.File.Delete(zipFilename);
            }
            #endif

            using (VideoShowDataContext DataContext = DataContextFactory.DataContext())
            {
                try
                {
                    System.Net.HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Video v = (from video in DataContext.Videos
                                   where video.VideoId == videoId
                                   select video).Single();
                        v.VideoUrl = string.Format("streaming:/{0}/{1}/{2}", ACCOUNTID, filesetName, filename);
                        v.PreviewUrl = string.Format("streaming:/{0}/{1}/{2}", ACCOUNTID, filesetName, filename);
                        v.ThumbnailUrl = thumbnailUrl;
                        v.StatusId = (int)VideoServices.VideoStatus.Complete; //Complete
                        DataContext.SubmitChanges();
                    }
                }
                catch (Exception ex)
                {
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.Trace.Warn("Upload", "Exception", ex);
                    }

                    Video v = (from video in DataContext.Videos
                               where video.VideoId == videoId
                               select video).Single();
                    v.StatusId = (int)VideoServices.VideoStatus.Failed; //Failed
                    DataContext.SubmitChanges();
                }
            }
        }

        private static void CopyStream(System.IO.FileStream inputStream, System.IO.Stream requestStream)
        {
            long bufferSize = inputStream.Length < BUFFER_SIZE ? inputStream.Length : BUFFER_SIZE;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            long bytesWritten = 0;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                requestStream.Write(buffer, 0, bytesRead);
                bytesWritten += bufferSize;
            }
        }
    }
}