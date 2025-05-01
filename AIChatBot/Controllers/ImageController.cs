using AIChatBot.DTO;
using AIChatBot.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIChatBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageFormatter _imageFormatter;

        public ImageController(IImageFormatter imageFormatter)
        {
            _imageFormatter = imageFormatter;
        }

        [HttpPost("/postImage")]
        public async Task<IActionResult> PostImage([FromForm]ImageDto image)
        {
            if (ModelState.IsValid)
            {
                var result = await _imageFormatter.ImageInterpert(image);
                return Ok(result);
            }
            else { return BadRequest(); }
        }
    }
}
