using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NAudio.Wave;
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
using System.Web;
using Text_Speech.Services;
using TextTranslations.Model;

namespace Text_Speech.Controllers
{
    [ApiController]
    [Route("api/tts")]
    public class TextController : ControllerBase
    {
        private readonly IBlob blob;

        readonly static string subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
        readonly static string endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");
        readonly string uriBase = endpoint + "vision/v3.0/ocr";


        string path = Path.GetFullPath("C:\\Users\\HP 650\\source\\repos\\Text_Speech\\Text_Speech\\Audio.mp3");
        FileInfo file;

        readonly static string speechKey = Environment.GetEnvironmentVariable("SPEECH_TRANSLATOR_SUBSCRIPTION_KEY");
        readonly static string speechEndpoint = Environment.GetEnvironmentVariable("SPEECH_TRANSLATOR_KEY");

      
        public TextController(IBlob blob)
        {
            this.blob = blob;
        }

        
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]

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
               var x = await TexttoSpeech(sentence);
                return Ok(x);
            }
            catch(Exception e) 
            { 
                return BadRequest(e.Message); 
            }
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

        private async Task<string> TexttoSpeech(string text)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", speechKey);
                client.DefaultRequestHeaders.Add("X-Microsoft-OutputFormat", "audio-48khz-96kbitrate-mono-mp3");
                client.DefaultRequestHeaders.Add("User-Agent", "TranslateSpeech");
                HttpResponseMessage httpResponse;
                await ReadandWriteText(text);
                
                using (StreamContent content = new StreamContent(file.OpenRead()))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/ssml+xml");
                    httpResponse = await client.PostAsync(speechEndpoint, content);
                }
                var contentStream =httpResponse.Content;
                using (var files = new FileStream(path, FileMode.Open))
                {
                    await contentStream.CopyToAsync(files);
                }
                /* var config = SpeechConfig.FromSubscription(speechKey, "centralus");
                 using var audio = AudioConfig.FromWavFileOutput(path);
                 using var synthesizer = new SpeechSynthesizer(config);
                var k= await synthesizer.SpeakTextAsync(file.FullName);*/

                //var x = GetRawMp3Frames(path);

                string s = "Sucessful";
                return s;
            }
            catch (Exception e)
            {

                return e.Message;
            }
        }
     
            private static byte[] GetRawMp3Frames(string filename)
        {
           
            using (MemoryStream output = new MemoryStream())
            {
                Mp3FileReader reader = new Mp3FileReader(filename);
                Mp3Frame frame;
                while ((frame = reader.ReadNextFrame()) != null)
                {
                    output.Write(frame.RawData, 0, frame.RawData.Length);
                }
                return output.ToArray();

            }
           
        }
        private async Task ReadandWriteText(string text)
        {
            string [] speaker = await blob.DownloadFile("Speaker.ssml");
            file = new FileInfo("Speakers");
            var speakers =   file.Create();
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < speaker.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(speaker[i]))
                {
                    s.Append(text);
                }
                else
                {
                    s.Append(speaker[i]);
                }
            }
            var str = s.ToString();
            var writer = new StreamWriter(speakers);
            writer.Write(str);
            writer.Close();
        }
        private HttpClient GetClient(string key)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            return client;
        }
        
       
    }
}
