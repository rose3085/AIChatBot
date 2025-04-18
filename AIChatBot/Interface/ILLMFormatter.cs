using AIChatBot.DTO;

namespace AIChatBot.Interface
{
    public interface ILLMFormatter
    {
        Task<string> FormatMessage(MessageDto request);

    }
}
