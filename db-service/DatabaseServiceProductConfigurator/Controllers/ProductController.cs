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

        private readonly IProductService _productService;
        private readonly ILanguageService _languageService;

        public ProductController(IProductService productService, ILanguageService languageService) {
            _productService = productService;
            _languageService = languageService;
        }

        [HttpGet]
        public IActionResult GetAllProducts() {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = _languageService.HandleLanguageInput(lang);

            List<Configurator> products = _productService.GetAllConfigurators(lang);
            if ( products.Count == 0 )
                return NoContent();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult Get( string id ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);   // Get the wanted language out of the Header
            lang = _languageService.HandleLanguageInput(lang);

            object? product = _productService.GetConfiguratorByProductNumber(id, lang);
            if ( product == null )
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public IActionResult Post( [FromBody] Configurator config ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);   // Get the wanted language out of the Header
            lang = _languageService.HandleLanguageInput(lang);

            _productService.SaveConfigurator(config, lang);

            return Accepted();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete( string id ) {
            _productService.DeleteConfigurator(id);

            return Accepted();
        }

    }
}
