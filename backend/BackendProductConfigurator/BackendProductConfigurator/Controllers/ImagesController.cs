using Microsoft.AspNetCore.Mvc;

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
                List<string> images = Directory.GetFiles("../images", "*.jpg").ToList();

                Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
                return images;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
