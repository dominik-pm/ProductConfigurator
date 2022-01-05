using DatabaseServiceProductConfigurator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseServiceProductConfigurator.Controllers {
    [Route("db/[controller]")]
    [ApiController]
    public class LanguageController : AController<ELanguage, string> {

        static product_configuratorContext context = new product_configuratorContext();

        public LanguageController() : base (context) { }

    }
}
