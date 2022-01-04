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
            Configuration? toReturn = ConfigurationService.GetById(id);
            if ( toReturn == null )
                return NotFound();
            else
                return Ok(toReturn);
        }

        [HttpGet("getByCustomer/{customerId}")]
        public IActionResult GetByCustomer( int? customerId ) {
            List<Configuration> configurations = ConfigurationService.GetConfigurationsByCustomer(customerId);
            if(configurations.Count() == 0)
                return NoContent();
            return Ok(configurations);
        }

        [HttpGet("getByProduct/{productNumber}")]
        public IActionResult getbyProduct( string productNumber) {
            List<Configuration> configurations = ConfigurationService.GetByProductNumber(productNumber);
            if ( configurations.Count() == 0 )
                return NoContent();
            return Ok(configurations);
        }

    }
}
