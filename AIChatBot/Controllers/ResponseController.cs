using AIChatBot.DTO;
using AIChatBot.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIChatBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
        private readonly IResponseService _responseService;

        public ResponseController(IResponseService responseService)
        {
            _responseService = responseService;
        }

        [HttpPost("/addKeywordAndResponse")]
        public async Task<IActionResult> AddKeywordAndResponse(AddResponseDto request)
        {

            if (ModelState.IsValid)
            {
                var result = await _responseService.AddKeywordAndResponse(request);
                return Ok(result);
            }
            else { return BadRequest(); }

        }
        

    }
}
