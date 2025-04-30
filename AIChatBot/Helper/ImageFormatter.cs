using AIChatBot.DTO;
using AIChatBot.Interface;
using GroqSharp;
using GroqSharp.Models;
using Microsoft.Identity.Client;

namespace AIChatBot.Helper
{
    public class ImageFormatter : IImageFormatter
    {
        public readonly IConfiguration _configuration;
        public ImageFormatter(IConfiguration configuration)
        {
            _configuration = configuration;   
        }
        public async Task<string> ImageInterpert(ImageDto image)
        {
            var apiKey = _configuration["LLM:ApiKey"];
            var apiModel = _configuration["LLM:ApiModelVision"];
            IGroqClient groqClient = new GroqClient(apiKey, apiModel)
            .SetTemperature(0.2)  // randomness of response   0 = more deterministic , 1= random
            .SetMaxTokens(256) // limits the output length
            .SetTopP(1) //
            .SetStop("NONE");

            var imageBase64 = await ConvertToBase64UrlAsync(image.Image);

            var jsonStructure =$@"[
                {{
                    ""type"": ""text"",
                    ""text"": ""What's in this image?""
                }},
                {{
                    ""type"": ""image_url"",
                    ""image_url"": {{
                        ""url"": ""{imageBase64}""
                    }}
                }}]";


            var response = await groqClient.CreateChatCompletionAsync(
          new Message
          {
              Role = MessageRoleType.System,
              Content =
                  "You are a helpful assistant designed to help customers to solve their problem and answer to any of their queries related to any technical issues in their daily life like broken pipes or gas stove and others.If the question has matched strings and database response give solution based on the database response combined with the user question. If the question doesn't have any matched strings and database response give answer based on your understanding. The answers should be short but meaningful.If the question is not related to electrical , plumbing or any mechanical issue please don't give any solution , give some answer like sorry i'm not trained for that but only for solving electrial and mechanical problems  and give response in a single sentence but not a whole paragraph in such case.Also accept if the image is given and check what the image is about and help to analyese and solve if the image has above problems "
          },
          new Message
          {
              Role = MessageRoleType.Assistant,
              Content =
                  "Based on the provided question give meaningful instruction to user to solve their problem."
          },
          new Message { Role = MessageRoleType.User, Content = $"{jsonStructure}" }
            );

            return response;
        }



        public  async Task<string> ConvertToBase64UrlAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                var base64String = Convert.ToBase64String(imageBytes);

                // Determine the content type (e.g., image/png or image/jpeg)
                var contentType = imageFile.ContentType;
                return $"data:{contentType};base64,{base64String}";
            }
        }
    }
}
