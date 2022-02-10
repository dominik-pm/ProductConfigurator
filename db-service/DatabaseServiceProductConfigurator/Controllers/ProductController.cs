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
        public ActionResult<List<Configurator>> GetAllProducts() {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = _languageService.HandleLanguageInput(lang);

            List<Configurator> products = _productService.GetAllConfigurators(lang);
            if ( products.Count == 0 )
                return NoContent();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public ActionResult<Configurator> Get( string id ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);   // Get the wanted language out of the Header
            lang = _languageService.HandleLanguageInput(lang);

            Configurator? product = _productService.GetConfiguratorByProductNumber(id, lang);
            if ( product == null )
                return NoContent();

            return Ok(product);
        }

        [HttpPost]
        public ActionResult Post( [FromBody] Configurator config ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);   // Get the wanted language out of the Header
            lang = _languageService.HandleLanguageInput(lang);

            _productService.SaveConfigurator(config, lang);

            return Accepted();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete( string id ) {
            _productService.DeleteConfigurator(id);

            return Accepted();
        }

        [HttpPut]
        public ActionResult Put( [FromBody] Configurator product ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);   // Get the wanted language out of the Header
            lang = _languageService.HandleLanguageInput(lang);

            _productService.UpdateProduct(product, lang);

            return Accepted();
        }

    }
}
