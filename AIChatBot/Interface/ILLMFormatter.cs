using AIChatBot.DTO;

namespace AIChatBot.Interface
{
    public interface ILLMFormatter
    {
        Task<string> FormatMessage(MessageDto request);

        Task<string> FormatAudio(AudioDto request);
        Task<string> TranscribeAudio(IFormFile audio);

    }
}
