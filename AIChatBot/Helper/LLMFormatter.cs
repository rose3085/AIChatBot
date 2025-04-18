using AIChatBot.DTO;
using AIChatBot.Interface;
using GroqSharp;
using GroqSharp.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AIChatBot.Helper
{
    public class LLMFormatter : ILLMFormatter
    {

        public readonly IConfiguration _configuration;
        public LLMFormatter(IConfiguration configuration)
        {
           _configuration = configuration; 
        }

        public async Task<string> FormatAudio(AudioDto request)
        {
           
            var question = await TranscribeAudio(request.Audio);
            var requestModel = new MessageDto()
            { Question = question };
           var response = await FormatMessage(requestModel);
            if (response != null)
            {
                return response;
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task<string> FormatMessage(MessageDto request)
        {
            var apiKey = _configuration["LLM:ApiKey"];
            var apiModel = _configuration["LLM:ApiModelChat"];

            IGroqClient groqClient = new GroqClient(apiKey, apiModel)
               .SetTemperature(0.5)  // randomness of response   0 = more deterministic , 1= random
               .SetMaxTokens(1024) // limits the output length
               .SetTopP(1) //
               .SetStop("NONE"); // tells the model when to stop 

            var prompt = new StringBuilder();
            var question = request.Question;
            prompt.AppendLine($"User's Question: {question}");
            var response = await groqClient.CreateChatCompletionAsync(
             new Message
             {
                 Role = MessageRoleType.System,
                 Content =
                     "You are a helpful assistant designed to help customers to solve their problem and answer to any of their queries.."
             },
             new Message
             {
                 Role = MessageRoleType.Assistant,
                 Content =
                     "Based on the provided question give meaningful instruction to user to solve their problem"
             },
             new Message { Role = MessageRoleType.User, Content = prompt.ToString() }
         );

            return response;
        }

        public async Task<string> TranscribeAudio(IFormFile audio)
        {
            var apiKey = _configuration["LLM:ApiKey"];
            var apiModel = _configuration["LLM:ApiModelAudio"];

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            using var content = new MultipartFormDataContent();

            var streamContent = new StreamContent(audio.OpenReadStream());
            content.Add(streamContent, "file", audio.FileName);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("audio/m4a");
            content.Add(new StringContent(apiModel), "model");

            var response = await client.PostAsync("https://api.groq.com/openai/v1/audio/transcriptions", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Groq transcription failed: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var transcription = doc.RootElement.GetProperty("text").GetString();

            return transcription;
        }
    }
}
