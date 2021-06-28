using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Text_Speech.Model;
using Text_Speech.Services;

namespace Text_Speech.Controller
{
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns if sucessful")]
    [SwaggerResponse((int)HttpStatusCode.NotFound, "Returns if is not found")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Returns image is not accepted")]

    [ApiController]
    [Route("api/ttd")]
    public class TextController : ControllerBase
    {
        private readonly IBlob blob;

        readonly static string computerVisionSubscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
        readonly static string computerVisonEndpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");
        readonly string computerVisionUriBase = computerVisonEndpoint + "vision/v3.0/ocr";

        FileInfo file;
     
        public TextController(IBlob blob)
        {
            this.blob = blob;
        }

        ///<param name="file"></param>
        /// <summary>
        /// A text image file either in png or jpeg
        /// </summary>
        /// 
        /// <returns>A string status</returns>
        [HttpPost]
        public async Task<ActionResult<string>> Post(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return this.StatusCode(StatusCodes.Status404NotFound, "file not found");
                }
                string mediatype = file.ContentType;
                if (mediatype != MimeMapping.KnownMimeTypes.Jpeg && mediatype != MimeMapping.KnownMimeTypes.Png)
                {
                    return this.StatusCode(StatusCodes.Status400BadRequest, "The image must be in jpeg or png");
                }

                await blob.Upload(file);
                var downloadedfile = await blob.Download(file);

                string analyzedDetails = await MakeAnalysisRequest(downloadedfile);
                Analysis analysis = JsonConvert.DeserializeObject<Analysis>(analyzedDetails);

                var sentence = Getwords(analysis);
                

                return Ok(sentence);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        
        }

        // To process the data on the images and retrieve the texts in string
        [NonAction]
        private async Task<string> MakeAnalysisRequest(byte[] file)
        {
            try
            {
                var client = GetClient(computerVisionSubscriptionKey);
                HttpResponseMessage httpResponse;
                using (ByteArrayContent Bytecontent = new ByteArrayContent(file))
                {
                    Bytecontent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponse = await client.PostAsync(computerVisionUriBase, Bytecontent);
                }
                string responseContent = await httpResponse.Content.ReadAsStringAsync();
                var stringContent = JToken.Parse(responseContent).ToString();
                return stringContent;
            }
            catch (Exception e)
            {
                
                return e.Message;
            }
        }
      
        [NonAction]
        private HttpClient GetClient(string key)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            return client;
        }

       
        [NonAction]
        private string Getwords(Analysis analysis)
        {
            var extractedWords = analysis.Region.SelectMany(data => data.Line.SelectMany(data => data.Word).Select(data => data.Text)).ToList();
            string compliedSentence = String.Join(" ", extractedWords);
            return compliedSentence;
        }

    }
}
