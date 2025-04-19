using AIChatBot.DTO;

namespace AIChatBot.Interface
{
    public interface ITextTokenizer
    {
        Task<TextTokens> Tokenizer(MessageDto msg);
        Task<List<string>> FuzzySearch(TextTokens tokens);
        Task<List<string>> GetMatchedStrings(MessageDto msg);
        Task<string> GetConcatResponseForMatchedStrings(List<string> request);

        Task<string> GetFinalResponse(MessageDto msg);
    }
}
