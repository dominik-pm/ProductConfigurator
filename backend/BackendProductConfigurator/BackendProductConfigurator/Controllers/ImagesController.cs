using BackendProductConfigurator.App_Code;
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
                return GetImagesRec(@$"{GlobalValues.ImagesFolder}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private List<string> GetImagesRec(string path)
        {
            List<string> images = Directory.GetFiles(path, "*.jpg").ToList();
            images.AddRange(Directory.GetFiles(path, "*.png").ToList());
            images.AddRange(Directory.GetFiles(path, "*.jpeg").ToList());
            images = images.Select(name => name.Replace($"{GlobalValues.ImagesFolder}\\", "").Replace('/', '*').Replace('\\', '*')).ToList();

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
                byte[] imageData = System.IO.File.ReadAllBytes(@$"{GlobalValues.ImagesFolder}/{location.Replace('*', '/')}");
                return new FileContentResult(imageData, location.EndsWith("jpg") ? "image/jpg" : (location.EndsWith("jpeg") ? "images/jpeg" : "image/png"));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
