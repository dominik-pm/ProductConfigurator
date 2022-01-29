using DatabaseServiceProductConfigurator.Models;
using DatabaseServiceProductConfigurator.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Newtonsoft.Json;
using System.Text.Json;

namespace DatabaseServiceProductConfigurator.Controllers {

    [EnableCors()]
    [Route("db/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase {

        static Product_configuratorContext context = new();

        [HttpGet]
        public IActionResult GetAllProducts() {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = LanguageService.HandleLanguageInput(lang);

            List<Configurator> products = ProductService.GetAllConfigurators(lang);
            if ( products.Count == 0 )
                return NoContent();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult Get( string id ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);   // Get the wanted language out of the Header
            lang = LanguageService.HandleLanguageInput(lang);

            object? product = ProductService.GetConfiguratorByProductNumber(id, lang);
            if ( product == null )
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public IActionResult Post( [FromBody] Configurator config ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);   // Get the wanted language out of the Header
            lang = LanguageService.HandleLanguageInput(lang);

            bool worked = ProductService.SaveConfigurator(config, lang);
            if ( !worked )
                return BadRequest();
            return Accepted();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete( string id ) {
            bool worked = ProductService.DeleteConfigurator(id);
            if ( !worked )
                return BadRequest();
            return Accepted();
        }

    }
}
