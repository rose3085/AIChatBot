using AIChatBot.DTO;
using AIChatBot.Interface;
using GroqSharp;
using GroqSharp.Models;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace AIChatBot.Helper
{
    public class LLMFormatter : ILLMFormatter
    {

        public readonly IConfiguration _configuration;
        public LLMFormatter(IConfiguration configuration)
        {
           _configuration = configuration; 
        }

        public async Task<string> FormatMessage(MessageDto request)
        {
            var apiKey = _configuration["LLM:ApiKey"];
            var apiModel = _configuration["LLM:ApiModel"];

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
    }
}
