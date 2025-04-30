using AIChatBot.DTO;

namespace AIChatBot.Interface
{
    public interface IImageFormatter
    {
        Task<string> ImageInterpert(ImageDto image);
    }
}
