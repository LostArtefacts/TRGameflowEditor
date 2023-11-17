using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace TRGE.Core
{
    public static class TRDownloader
    {
        private const string _resourceURL = "https://github.com/lahm86/TRGameflowEditor/releases/download/{0}/{1}";

        public static event EventHandler<TRDownloadEventArgs> ResourceDownloading;

        internal static bool Download(string urlPath, string targetFile, bool isCompressed)
        {
            string url = string.Format(_resourceURL, TRInterop.TaggedVersion, urlPath);

            TRDownloadEventArgs args = new TRDownloadEventArgs
            {
                URL = url,
                TargetFile = targetFile
            };

            ResourceDownloading?.Invoke(null, args);

            string tempFile = Path.GetTempFileName();

            try
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.CacheControl = new()
                {
                    NoCache = true
                };

                HttpResponseMessage response = client.Send(new(HttpMethod.Get, url));
                response.EnsureSuccessStatusCode();

                using Stream receiveStream = response.Content.ReadAsStream();
                using FileStream outputStream = File.OpenWrite(tempFile);

                args.DownloadLength = response.Content.Headers.ContentLength ?? 0;
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
                    outputStream.Write(buffer, 0, size);
                    args.DownloadProgress += size;
                    args.DownloadDifference = size;
                    ResourceDownloading?.Invoke(null, args);
                }

                if (!args.IsCancelled)
                {
                    args.Status = TRDownloadStatus.Committing;
                    ResourceDownloading?.Invoke(null, args);
                    if (isCompressed)
                    {
                        using FileStream fs = File.OpenRead(tempFile);
                        using GZipStream zs = new(fs, CompressionMode.Decompress, false);
                        using FileStream os = File.OpenWrite(targetFile);
                        zs.CopyTo(os);
                    }
                    else
                    {
                        File.Move(tempFile, targetFile);
                    }

                    args.Status = TRDownloadStatus.Completed;
                }
            }
            catch (Exception e)
            {
                args.Exception = e;
                args.Status = TRDownloadStatus.Failed;
            }

            ResourceDownloading?.Invoke(null, args);

            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }

            return args.Status == TRDownloadStatus.Completed;
        }
    }
}