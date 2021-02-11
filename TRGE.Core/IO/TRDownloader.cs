using System;
using System.IO;
using System.Net;

namespace TRGE.Core
{
    public static class TRDownloader
    {
        private const string _resourceURLBase = "https://raw.githubusercontent.com/lahm86/TRGameflowEditor/main/";

        public static event EventHandler<TRDownloadEventArgs> ResourceDownloading;

        internal static bool Download(string urlPath, string targetFile)
        {
            string url = _resourceURLBase + urlPath;

            TRDownloadEventArgs args = new TRDownloadEventArgs
            {
                URL = url,
                TargetFile = targetFile
            };

            ResourceDownloading?.Invoke(null, args);

            try
            {
                HttpWebRequest req = WebRequest.CreateHttp(url);
                using (WebResponse response = req.GetResponse())
                using (Stream receiveStream = response.GetResponseStream())
                using (FileStream ouputStream = File.OpenWrite(targetFile))
                {
                    args.DownloadLength = response.ContentLength;
                    args.Status = TRDownloadStatus.Downloading;
                    ResourceDownloading?.Invoke(null, args);

                    byte[] buffer = new byte[1024];
                    int size;
                    while ((size = receiveStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (args.IsCancelled)
                        {
                            break;
                        }
                        ouputStream.Write(buffer, 0, size);
                        args.DownloadProgress += size;
                        args.DownloadDifference = size;
                        ResourceDownloading?.Invoke(null, args);
                    }

                    if (!args.IsCancelled)
                    {
                        args.Status = TRDownloadStatus.Completed;
                    }
                }
            }
            catch (Exception e)
            {
                args.Exception = e;
                args.Status = TRDownloadStatus.Failed;
            }

            ResourceDownloading?.Invoke(null, args);

            if (File.Exists(targetFile) && (args.IsCancelled || args.Status == TRDownloadStatus.Failed))
            {
                File.Delete(targetFile);
            }

            return args.Status == TRDownloadStatus.Completed;
        }
    }
}