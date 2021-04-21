using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
}
    }

