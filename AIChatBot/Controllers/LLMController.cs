using AIChatBot.DTO;
using AIChatBot.Helper;
using AIChatBot.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AIChatBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LLMController : ControllerBase
    {
        public readonly ILLMFormatter _llmFormatter;
        private readonly TextTokenizer _tokenizer;

        public LLMController(ILLMFormatter llmFormatter, TextTokenizer tokenizer)
        {
            _llmFormatter = llmFormatter; 
            _tokenizer = tokenizer;
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
        [HttpPost("/postAudio")]
        public async Task<IActionResult> PostAudio([FromForm] AudioDto audio)
        {
            if (ModelState.IsValid)
            {
                var result = await _llmFormatter.FormatAudio(audio);
                return Ok(result);
            }
            else { return BadRequest(); }
        }

        [HttpPost("/tokenizer")]
        public async Task<IActionResult> TextTokenizer([FromBody] MessageDto question)
        {
            if (ModelState.IsValid)
            {
                var result = await _tokenizer.GetMatchedStrings(question);
                return Ok(result);
            }
            else { return BadRequest(); }
        }
    }
}
