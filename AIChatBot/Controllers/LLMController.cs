using AIChatBot.DTO;
using AIChatBot.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AIChatBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LLMController : ControllerBase
    {
        public readonly ILLMFormatter _llmFormatter;
        public LLMController(ILLMFormatter llmFormatter)
        {
            _llmFormatter = llmFormatter;   
        }

        [HttpPost("/postQuestions")]
        public async Task<IActionResult> PostChat([FromBody]MessageDto question)
        {
            if (ModelState.IsValid)
            {
                var result = await _llmFormatter.FormatMessage(question);
                return Ok(result);
            }
            else { return BadRequest(); }
        }
    }
}
