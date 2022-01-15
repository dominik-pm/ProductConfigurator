﻿using DatabaseServiceProductConfigurator.Models;
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

        static product_configuratorContext context = new product_configuratorContext();

        [HttpGet]
        public IActionResult Get() {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = LanguageService.HandleLanguageInput(lang);

            List<ConfiguredProduct> toReturn = ConfigurationService.GetConfiguredProducts(lang);

            if ( !toReturn.Any() )
                return NoContent();
            return Ok(toReturn);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id) {
            ConfiguredProduct? toReturn = ConfigurationService.GetConfiguredProductById(id);
            if ( toReturn == null )
                return NotFound();
            return Ok(toReturn);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ConfiguredProduct config) {
            bool worked = ConfigurationService.SaveConfiguredProduct(config);

            if ( !worked )
                return BadRequest();
            return Accepted();
        }

    }
}
