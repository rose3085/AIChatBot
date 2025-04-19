using AIChatBot.DTO;

namespace AIChatBot.Interface
{
    public interface IResponseService
    {
        Task<ServiceResponse> AddKeywordAndResponse(AddResponseDto request);
        Task<string> GetResponseForKeyword(string keyword);
    }
}
