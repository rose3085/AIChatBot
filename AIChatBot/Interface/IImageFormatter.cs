using AIChatBot.DTO;

namespace AIChatBot.Interface
{
    public interface IImageFormatter
    {
        Task<string> ImageInterpert(ImageDto image);
        Task<string> ConvertToBase64UrlAsync(IFormFile imageFile);
        Task<bool> CheckImageValidity(IFormFile imageFile);
       
    }
}
