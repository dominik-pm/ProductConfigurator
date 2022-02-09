using Microsoft.AspNetCore.Mvc;
using Model;

namespace BackendProductConfigurator.Controllers
{
    [Route("products")]
    public partial class ProductsController : AController<ConfiguratorSlim, string>
    {
        public ProductsController() : base()
        {
            entities = AValuesClass.ConfiguratorsSlim;
        }

        // GET: /products
        [HttpGet]
        public override IEnumerable<ConfiguratorSlim> Get()
        {
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            return entities[GetAccLang(Request)];
        }
    }
}
