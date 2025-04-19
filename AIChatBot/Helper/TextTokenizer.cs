using AIChatBot.DTO;
using AIChatBot.Interface;
using FuzzySharp;
using Microsoft.ML;
using System.Security.Cryptography.Xml;
using System.Text;
using static AIChatBot.Helper.KeywordEnums;

namespace AIChatBot.Helper
{
    public class TextTokenizer : ITextTokenizer
    {
        private readonly IResponseService _responseService;

        public TextTokenizer(IResponseService responseService)
        {
            _responseService = responseService;
        }
        public async Task<TextTokens> Tokenizer(MessageDto msg)
        {
            var context = new MLContext();
            var emptyData = new List<MessageDto>();
            emptyData.Add(msg);
            var data = context.Data.LoadFromEnumerable(emptyData);
            var tokenization = context.Transforms.Text.TokenizeIntoWords("Tokens", "Question", separators: new[] { ' ' });
            var tokenModel = tokenization.Fit(data);
            var engine = context.Model.CreatePredictionEngine<MessageDto, TextTokens>(tokenModel);
            var tokens = engine.Predict(msg);
      
            return tokens;
        }

        public async Task<List<string>> FuzzySearch(TextTokens tokens)
        {
            var matchedEnums = new List<string>();
            var enumNames = Enum.GetNames(typeof(Keyword));
            foreach (var token in tokens.Tokens)
            {
                foreach (var name in enumNames)
                {
                    var cleanedEnum = name.ToLower();
                    var match = Fuzz.Ratio(token,cleanedEnum);
                    if (match > 80)
                    {

                        //var matched = (Keyword)Enum.Parse(typeof(Keyword), name);
                        matchedEnums.Add(name);

                    }
                }
            
            }
            return matchedEnums;
        
        }

        public async Task<List<string>> GetMatchedStrings(MessageDto msg)
        {
            var tokens = await Tokenizer(msg);
            if (tokens != null)
            {
                var matched = await FuzzySearch(tokens);
                return matched;
            }
            else { return null; }
        }


        public async Task<string> GetConcatResponseForMatchedStrings(List<string> request)
        {
            var stringBuilder = new StringBuilder();
            foreach (var keyword in request)
            {
                var response = await _responseService.GetResponseForKeyword(keyword);
                if (!string.IsNullOrWhiteSpace(response))
                {
                    stringBuilder.AppendLine(response);
                }
            }
            string finalResponse = stringBuilder.ToString();
            return finalResponse;
        }

        public async Task<string> GetFinalResponse(MessageDto msg)
        {
            var matchedStrings = await GetMatchedStrings(msg);
            if (!matchedStrings.Any())
            {
                var message = "You don't have any matched strings";
                return message;
            }
            else {
                var response = await GetConcatResponseForMatchedStrings(matchedStrings);
                if (response != null)
                {
                    return response;

                }
                else {
                    var message = "You don't have any response in the database";
                    return message;
                    }
            }
        }
    }
}
