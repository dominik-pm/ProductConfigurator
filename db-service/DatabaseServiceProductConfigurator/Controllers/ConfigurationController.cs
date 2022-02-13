using DatabaseServiceProductConfigurator.Models;
using DatabaseServiceProductConfigurator.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Wrapper;

namespace DatabaseServiceProductConfigurator.Controllers {

    [EnableCors()]
    [Route("db/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase {

        private readonly IConfigurationService _configurationService;
        private readonly ILanguageService _languageService;

        public ConfigurationController( IConfigurationService configurationService, ILanguageService languageService ) {
            _configurationService = configurationService;
            _languageService = languageService;
        }

        [HttpGet]
        public ActionResult<List<ProductSaveExtended>> Get() {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = _languageService.HandleLanguageInput(lang);

            List<ProductSaveExtended> toReturn = _configurationService.GetConfigurations(lang);

            return Ok(toReturn);
        }

        [HttpGet("byWrapper")]
        public ActionResult<ProductSaveExtended> GetById( SavedConfigDeleteWrapper wrapper ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = _languageService.HandleLanguageInput(lang);

            ProductSaveExtended? toReturn = _configurationService.GetConfiguredProductById(lang, wrapper);

            if ( toReturn == null )
                return NotFound();
            return Ok(toReturn);
        }

        [HttpPost]
        public ActionResult Post( [FromBody] ProductSaveExtended config ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = _languageService.HandleLanguageInputCreate(lang);

            _configurationService.SaveConfiguration(config, lang);

            return Ok();
        }

        [HttpDelete]
        public ActionResult Delete( SavedConfigDeleteWrapper wrapper ) {
            _configurationService.DeleteConfiguration(wrapper);

            return Ok();
        }

        [HttpPut("{savedName}")]
        public ActionResult Put( [FromBody] ProductSaveExtended config, string savedName ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = _languageService.HandleLanguageInputCreate(lang);

            _configurationService.UpdateConfiguration(config, lang, savedName);

            return Ok();
        }

    }
}
