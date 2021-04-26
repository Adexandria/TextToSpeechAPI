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
using System.Text;
using System.Threading.Tasks;
using Text_Speech.Services;
using TextTranslations.Model;

namespace Text_Speech.Controllers
{
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns if sucessful")]
    [SwaggerResponse((int)HttpStatusCode.NotFound, "Returns if is not found")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Returns image is not accepted")]

    [ApiController]
    [Route("api/tts")]
    public class TextController : ControllerBase
    {
        private readonly IBlob blob;

        readonly static string subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
        readonly static string endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");
        readonly string uriBase = endpoint + "vision/v3.0/ocr";

        FileInfo file;

        readonly static string speechKey = Environment.GetEnvironmentVariable("SPEECH_TRANSLATOR_SUBSCRIPTION_KEY");
        readonly static string speechEndpoint = Environment.GetEnvironmentVariable("SPEECH_TRANSLATOR_KEY");

      
        public TextController(IBlob blob)
        {
            this.blob = blob;
        }

        ///<param name="file"></param>
        /// <summary>
        /// To convert a text image into an Audio File.The text image can either be in jpeg or png
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
                if(mediatype != MimeMapping.KnownMimeTypes.Jpeg && mediatype != MimeMapping.KnownMimeTypes.Png)
                { 
                return this.StatusCode(StatusCodes.Status400BadRequest, "The image must be in jpeg or png");
                }
                
                await blob.Upload(file);
                var downloadedfile = await blob.Download(file);
                
                string details = await MakeAnalysisRequest(downloadedfile);
                Analyze analyze = JsonConvert.DeserializeObject<Analyze>(details);

                var sentence = Getwords(analyze);
                var result = await TexttoSpeech(sentence);

                return Ok(result);
                }catch(Exception e) 
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
                var client = GetClient(subscriptionKey);
                HttpResponseMessage httpResponse;
                using (ByteArrayContent content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponse = await client.PostAsync(uriBase, content);
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
        //To get the audio file
        [NonAction]
        private async Task<string> TexttoSpeech(string text)
        {
            try
            {
                var client  = GetClient(speechKey);
                client.DefaultRequestHeaders.Add("X-Microsoft-OutputFormat", "audio-48khz-96kbitrate-mono-mp3");
                client.DefaultRequestHeaders.Add("User-Agent", "TranslateSpeech");
                HttpResponseMessage httpResponse;
                await ReadandWriteText(text);

                using (StreamContent content = new StreamContent(file.OpenRead()))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/ssml+xml");
                    httpResponse = await client.PostAsync(speechEndpoint, content);
                }
                var contentStream = await httpResponse.Content.ReadAsStreamAsync();
                await blob.UploadStream(contentStream);
                var url = blob.GetUri("Audio.mp3");
                return $"Sucessful, copy on this {url}";
            }
            catch (Exception e)
            {

                return e.Message;
            }
        }
        //To add the text into the ssml file
        [NonAction]   
        private async Task ReadandWriteText(string text)
        {
            // a ssml template file
            string [] speakerfile = await blob.DownloadFile("Speaker.ssml");

            file = new FileInfo("Speakers");
            var ssmlFile =   file.Create();

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < speakerfile.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(speakerfile[i]))
                {
                    builder.Append(text);
                }
                else
                {
                    builder.Append(speakerfile[i]);
                }
            }
            var writer = new StreamWriter(ssmlFile);
            writer.Write(builder.ToString());
            writer.Close();
        }

        //To get the client
        [NonAction]
        private HttpClient GetClient(string key)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            return client;
        }

        //To process the result gotten from the analysis and retrieve the words in a string format
        [NonAction]
        private string Getwords(Analyze analyze)
        {
            var words = analyze.Regions.SelectMany(e => e.Lines.SelectMany(e => e.Words).Select(s => s.Text)).ToList();
            string sentence = String.Join(" ", words);
            return sentence;
        }

    }
}
