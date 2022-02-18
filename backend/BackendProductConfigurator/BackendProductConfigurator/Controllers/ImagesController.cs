using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace BackendProductConfigurator.Controllers
{
    [ApiController]
    public class ImagesController : ControllerBase
    {
        [Route("/images")]
        [HttpGet]
        public ActionResult<List<string>> GetImages()
        {
            try
            {
                List<string> images = Directory.GetFiles(@"../images", "*.jpg").ToList().Select(name => name.Replace(@"/", "\\").Replace("../images/", "")).ToList();

                Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
                return images;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("/images/{location}")]
        [HttpGet]
        public ActionResult<byte[]> GetImageData(string location)
        {
            try
            {
                byte[] imageData = System.IO.File.ReadAllBytes(@$"..\images\{location}");
                return new FileContentResult(imageData, "image/jpg");
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }
    }
}
