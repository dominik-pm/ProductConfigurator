using DatabaseServiceProductConfigurator.Models;
using DatabaseServiceProductConfigurator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace DatabaseServiceProductConfigurator.Controllers {
    [Route("db/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase {

        static product_configuratorContext context = new product_configuratorContext();

        [HttpGet("GetBuyableProducts")]
        public IActionResult GetBuyableProducts() {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = LanguageService.HandleLanguageInput(lang);

            List<object> products = ProductService.GetBuyableProducts(lang);

            if ( products.Count == 0 )
                return NoContent();

            return Ok(products);
        }

        [HttpGet]
        public IActionResult GetAllProducts() {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = LanguageService.HandleLanguageInput(lang);

            List<Configurator> products = ProductService.getAllConfigurators(lang);
            if ( products.Count == 0 )
                return NoContent();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);   // Get the wanted language out of the Header
            lang = LanguageService.HandleLanguageInput(lang);

            object? product = ProductService.GetConfiguratorByProductNumber(id, lang);
            if ( product == null )
                return NotFound();
            return Ok(product);
        }
    }
}
