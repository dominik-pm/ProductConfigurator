using DatabaseServiceProductConfigurator.Models;
using DatabaseServiceProductConfigurator.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseServiceProductConfigurator.Controllers {

    [EnableCors()]
    [Route("db/[controller]")]
    [ApiController]
    public class LanguageController : ControllerBase {

        static Product_configuratorContext context = new Product_configuratorContext();

        [HttpGet]
        public IActionResult get() {
            List<string> list = LanguageService.GetAllLanguages();
            if ( list.Count == 0 )
                return NoContent();
            return Ok(list);
        }

    }
}
