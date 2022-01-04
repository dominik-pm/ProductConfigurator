using DatabaseServiceProductConfigurator.Models;
using DatabaseServiceProductConfigurator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseServiceProductConfigurator.Controllers {
    [Route("db/[controller]")]
    [ApiController]
    public class ProductController : AController<Product, string> {

        static product_configuratorContext context = new product_configuratorContext();

        public ProductController() : base(context) { }

        [HttpGet("GetBuyableProducts")]
        public IActionResult GetBuyableProducts() {
            List<Product> products = ProductService.GetBuyableProducts();

            if ( products.Count == 0 )
                return NoContent();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public override IActionResult Get(string id) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);   // Get the wanted language out of the Header

            object? product = ProductService.GetWithOption(id, lang);
            if ( product == null )
                return NotFound();
            return Ok(product);
        }
    }
}
