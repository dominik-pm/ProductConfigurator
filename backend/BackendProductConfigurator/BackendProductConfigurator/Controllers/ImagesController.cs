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

        [Route("/images/")]
        [HttpGet("{location}")]
        public ActionResult<byte[]> GetImageData(string location)
        {
            try
            {
                return System.IO.File.ReadAllBytes(location);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
