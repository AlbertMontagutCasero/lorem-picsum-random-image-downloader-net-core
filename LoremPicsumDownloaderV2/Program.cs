using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LoremPicsumDownloaderV2
{
    public class ImageStub
    {
        public string download_url;
        public string id;
    }

    public class Program
    {
        private static void Main(string[] args)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string finalDirectory = currentDirectory + "/Images";
            Directory.CreateDirectory(finalDirectory);

            new ImageSaver().DownloadAndSaveImages(finalDirectory);
        }

        public class ImageSaver
        {
            private string baseServerUrl = "https://picsum.photos/";
            private string imageEndpointUrl = "v2/list?page=2&limit=100";
            private HttpClient client = new HttpClient();

            public void DownloadAndSaveImages(string finalDirectory)
            {
                this.client.BaseAddress = new Uri(this.baseServerUrl);
                this.client.DefaultRequestHeaders.Accept.Clear();
                this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = this.client.GetAsync(this.imageEndpointUrl).Result;

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("{0} ({1})", (int) response.StatusCode, response.ReasonPhrase);
                    this.client.Dispose();

                    return;
                }

                this.ReadResponseAndStore(finalDirectory, response);
                this.client.Dispose();
            }

            private void ReadResponseAndStore(string finalDirectory, HttpResponseMessage response)
            {
                IEnumerable<ImageStub> dataObjects = response.Content.ReadAsAsync<IEnumerable<ImageStub>>().Result;

                using (WebClient client = new WebClient())
                {
                    foreach (ImageStub image in dataObjects)
                    {
                        client.DownloadFile(new Uri(image.download_url), finalDirectory + "/" + image.id + ".jpg");
                    }
                }
            }
        }
    }
}
