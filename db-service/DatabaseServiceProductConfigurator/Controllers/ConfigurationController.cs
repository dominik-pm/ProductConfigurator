using DatabaseServiceProductConfigurator.Models;
using DatabaseServiceProductConfigurator.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace DatabaseServiceProductConfigurator.Controllers {

    [EnableCors()]
    [Route("db/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase {

        private readonly IConfigurationService _configurationService;
        private readonly ILanguageService _languageService;

        public ConfigurationController(IConfigurationService configurationService, ILanguageService languageService) {
            _configurationService = configurationService;
            _languageService = languageService;
        }

        [HttpGet]
        public IActionResult Get() {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = _languageService.HandleLanguageInput(lang);

            List<ProductSaveExtended> toReturn = _configurationService.GetConfigurations(lang);

            if ( !toReturn.Any() )
                return NoContent();
            return Ok(toReturn);
        }

        [HttpGet("{id}")]
        public IActionResult GetById( string id ) {
            ProductSaveExtended? toReturn = _configurationService.GetConfiguredProductById(id);
            if ( toReturn == null )
                return NotFound();
            return Ok(toReturn);
        }

        [HttpPost]
        public IActionResult Post( [FromBody] ProductSaveExtended config ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = _languageService.HandleLanguageInput(lang);

            _configurationService.SaveConfiguration(config, lang);

            return Accepted();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete( int id ) {
            _configurationService.DeleteConfiguration(id);

            return Accepted();
        }

    }
}
