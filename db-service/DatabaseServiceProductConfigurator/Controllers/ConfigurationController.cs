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

        [HttpGet]
        public IActionResult Get() {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = LanguageService.HandleLanguageInput(lang);

            List<ProductSaveExtended> toReturn = ConfigurationService.GetConfigurations(lang);

            if ( !toReturn.Any() )
                return NoContent();
            return Ok(toReturn);
        }

        [HttpGet("{id}")]
        public IActionResult GetById( string id ) {
            ProductSaveExtended? toReturn = ConfigurationService.GetConfiguredProductById(id);
            if ( toReturn == null )
                return NotFound();
            return Ok(toReturn);
        }

        [HttpPost]
        public IActionResult Post( [FromBody] ProductSaveExtended config ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = LanguageService.HandleLanguageInput(lang);

            bool worked = ConfigurationService.SaveConfiguration(config, lang);

            if ( !worked )
                return BadRequest();
            return Accepted();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete( int id ) {
            bool worked = ConfigurationService.DeleteConfiguration(id);

            if ( !worked )
                return BadRequest();
            return Accepted();
        }

    }
}
