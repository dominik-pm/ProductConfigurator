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
                Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
                return GetImagesRec(@"../images");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private List<string> GetImagesRec(string path)
        {
            List<string> images = Directory.GetFiles(path, "*.jpg").ToList().Select(name => name.Replace(@"../images\", "").Replace('/', '*').Replace(@"\", "*")).ToList();

            foreach(var folder in Directory.GetDirectories(path))
            {
                images.AddRange(GetImagesRec($"{folder}"));
            }

            return images;
        }

        [Route("/images/{location}")]
        [HttpGet]
        public ActionResult<byte[]> GetImageData(string location)
        {
            try
            {
                byte[] imageData = System.IO.File.ReadAllBytes(@$"../images/{location.Replace('*', '/')}");
                return new FileContentResult(imageData, "image/jpg");
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
