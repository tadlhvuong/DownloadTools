using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace DownloadTools
{
    public class HttpHeader
    {
        public string name;
        public string value;
    }

    public class HttpContent
    {
        public int size;
        public string mimeType;
        public string text;
    }

    public class LogRequest
    {
        public string method;
        public string url;
        public HttpHeader[] headers;
    }

    public class LogResponse
    {
        public int status;
        public HttpHeader[] headers;
        public HttpContent content;
    }

    public class LogEntry
    {
        public LogRequest request;
        public LogResponse response;
    }

    public class ChromeLog
    {
        public LogEntry[] entries;
    }

    public class Downloader
    {
        private string outDir;
        private string srcUrl;

        private WebClient webClient = new WebClient();

        public Downloader(string outDir, string srcUrl)
        {
            this.outDir = outDir;
            this.srcUrl = srcUrl;
        }

        public void DoDownloadJson(string jsonFile)
        {
            string allText = File.ReadAllText(jsonFile);    
            JObject obj = JObject.Parse(allText);
            ChromeLog log = (obj["log"]).ToObject<ChromeLog>();
            foreach (var item in log.entries)
            {
                string itemUrl = item.request.url;
                if (itemUrl.StartsWith(srcUrl))
                    DownloadLink(itemUrl);
            }
        }

        public void DoDownloadText(string txtFile)
        {
            string[] allLines = File.ReadAllLines(txtFile);
            foreach (var line in allLines)
            {
                var itemUrl = srcUrl + line;
                DownloadLink(itemUrl);
            }
        }

        private void DownloadLink(string link)
        {
            Uri fileUri = new Uri(link);
            string fileName = outDir + fileUri.AbsolutePath.Replace("/", "\\");
            if (File.Exists(fileName))
                return;

            CreateDir(fileName);

            try
            {
                Console.Write("Downloading {0}... ", fileUri.AbsolutePath);
                webClient.DownloadFile(fileUri, fileName);
                Console.WriteLine("done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed! Error: {0}", ex.Message);
            }
        }

        private void CreateDir(string path)
        {
            try
            {
                string dirPath = Path.GetDirectoryName(path) + "\\";
                int nPos = outDir.Length;
                while (nPos > 0 && nPos < dirPath.Length)
                {
                    string subDir = dirPath.Substring(0, nPos);
                    if (!Directory.Exists(subDir))
                        Directory.CreateDirectory(subDir);

                    nPos = dirPath.IndexOf('\\', nPos + 1);
                }
            }
            catch(System.IO.PathTooLongException e)
            {
                return;
            }
        }
    }
}
