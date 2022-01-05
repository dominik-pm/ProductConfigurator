using DatabaseServiceProductConfigurator.Models;
using DatabaseServiceProductConfigurator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseServiceProductConfigurator.Controllers {
    [Route("db/[controller]")]
    [ApiController]
    public class ConfigurationController : AController<Configuration, int> {

        static product_configuratorContext context = new product_configuratorContext();

        public ConfigurationController() : base(context) { }

        // GET by ID
        [HttpGet("{id}")]
        public override IActionResult Get( int id ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);

            ConfigStruct? toReturn = ConfigurationService.GetById(id, lang);
            if ( toReturn == null )
                return NotFound();
            else
                return Ok(toReturn);
        }

        [HttpGet("getByCustomer/{customerId}")]
        public IActionResult GetByCustomer( int? customerId ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);

            List<ConfigStruct> configurations = ConfigurationService.GetConfigurationsByCustomer(customerId, lang);
            if(configurations.Count() == 0)
                return NoContent();
            return Ok(configurations);
        }

        [HttpGet("getByProduct/{productNumber}")]
        public IActionResult getbyProduct( string productNumber) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);

            List<ConfigStruct> configurations = ConfigurationService.GetByProductNumber(productNumber, lang);
            if ( configurations.Count() == 0 )
                return NoContent();
            return Ok(configurations);
        }

    }
}
