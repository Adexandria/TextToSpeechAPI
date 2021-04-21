using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Text_Speech.Services;
using TextTranslations.Model;

namespace Text_Speech.Controllers
{
    [ApiController]
    [Route("api/tts")]
    public class TextController : ControllerBase
    {
        private readonly IBlob blob;

        static string subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
        static string endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");
        string uriBase = endpoint + "vision/v3.0/ocr";


        static string speechKey = Environment.GetEnvironmentVariable("SPEECH_TRANSLATOR_SUBSCRIPTION_KEY");
        static string speechEndpoint = Environment.GetEnvironmentVariable("SPEECH_TRANSLATOR_KEY");
        static string firstpath = Path.GetFullPath("C:\\Users\\HP 650\\source\\repos\\TextTranslations\\Speaker.ssml");
        static string path = Path.GetFullPath("C:\\Users\\HP 650\\source\\repos\\TextTranslations\\Speakers.ssml");

      
        public TextController(IBlob blob)
        {
            this.blob = blob;
        }
        [HttpPost]
        public async Task<ActionResult<string>> Post(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return this.StatusCode(StatusCodes.Status404NotFound, "file not found");
                }
                await blob.Upload(file);
                var download = await blob.Download(file);
                string details = await MakeAnalysisRequest(download);
                Analyze analyze = JsonConvert.DeserializeObject<Analyze>(details);
                var words = analyze.regions.SelectMany(e => e.lines.SelectMany(e => e.words).Select(s => s.text)).ToList();
                string sentence = String.Join(" ", words);
                return Ok(sentence);
            }
            catch(Exception e) { return BadRequest(e.Message); }
        }
        [NonAction]
        private async Task<string> MakeAnalysisRequest(byte[] file)
        {

            try
            {
                var client = GetClient(subscriptionKey);
                string uri = uriBase;
                HttpResponseMessage httpResponse;
                using (ByteArrayContent content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponse = await client.PostAsync(uri, content);
                }
                string contentString = await httpResponse.Content.ReadAsStringAsync();
                var newContent = JToken.Parse(contentString).ToString();
                return newContent;


            }
            catch (Exception e)
            {
                
                return e.Message;
            }
        }
       
        
        private HttpClient GetClient(string key)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            return client;
        }
        
       
    }
}
