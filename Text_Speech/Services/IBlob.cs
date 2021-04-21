using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Text_Speech.Services
{
    public  interface IBlob
    {
        Task Upload(IFormFile model);
        Task<byte[]> Download(IFormFile file);
    }
}
