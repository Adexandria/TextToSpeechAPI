using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Speech.Services
{
    public class Blob : IBlob { 
    private readonly BlobServiceClient _blobServiceClient;
    public Blob(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task Upload(IFormFile model)
    {
        var blobContainer = _blobServiceClient.GetBlobContainerClient("textimages");
        var blobClient = blobContainer.GetBlobClient(model.FileName);
        if (!blobClient.ExistsAsync().Result)
        {
           await blobClient.UploadAsync(model.OpenReadStream());
        }
        
    }
        public Uri GetUri(string file) 
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("textimages");
            var blobClient = blobContainer.GetBlobClient(file);

            if (blobClient.ExistsAsync().Result)
            {
                var s = blobClient.Uri;
                return s;
            }
            return null;
        }
        
       
        public async Task<byte[]> Download(IFormFile file) 
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("textimages");
            var blobClient = blobContainer.GetBlobClient(file.FileName);
            var download =  blobClient.Download();
            using (MemoryStream ms = new MemoryStream())
            {
                await download.Value.Content.CopyToAsync(ms);
                return ms.ToArray();
            }

        }

        public async Task<string[]> DownloadFile(string file)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("textimages");
            var blobClient = blobContainer.GetBlobClient(file);
            var files =  blobClient.Download();
            string[] result;
            using (MemoryStream ms = new MemoryStream())
            { 
                await files.Value.Content.CopyToAsync(ms);
                 result = Encoding.
                  ASCII.
                  GetString(ms.ToArray()).
                  Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            }

            return result;
        }
    }
    }

