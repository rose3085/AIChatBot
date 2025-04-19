using AIChatBot.Data;
using AIChatBot.DTO;
using AIChatBot.Interface;
using AIChatBot.Model;
using Azure.Core;
using Microsoft.EntityFrameworkCore;

namespace AIChatBot.Services
{
    public class ResponseService : IResponseService
    {
        private readonly DataContext _context;

        public ResponseService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse> AddKeywordAndResponse(AddResponseDto request)
        {
            try {
                var checkKeyword = await _context.Responses.AnyAsync(x => x.Keyword == request.Keyword);

                if (!checkKeyword)
                {
                    var id =  Guid.NewGuid().ToString();
                    var requestModel = new ResponseModel()
                    {
                        Id = id,
                        Keyword = request.Keyword,
                        Response = request.Response,
                    };
                    _context.Responses.Add(requestModel);

                    await _context.SaveChangesAsync();
                    return new ServiceResponse()
                    {
                        IsSuccess = true,
                        Message = "Keyword and responses added succssfully"
                    };
                }
                else {
                    return new ServiceResponse()
                    {
                        IsSuccess = false,
                        Message = "Couldn't add Keyword and responses"
                    };
                }
            
            }
            catch(Exception e) {

                return new ServiceResponse()
                { 
                    IsSuccess = false,
                    Message = $"{e.Message}"
                };
            }
        }

        public async Task<string> GetResponseForKeyword(string keyword)
        {
            try
            {
                var checkKeyword = await _context.Responses.Where(x => x.Keyword == keyword).FirstOrDefaultAsync();
                
                if (checkKeyword == null)
                {
                    return null;
                }
                else { 
                
                    var response = checkKeyword.Response;
                    return response;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
