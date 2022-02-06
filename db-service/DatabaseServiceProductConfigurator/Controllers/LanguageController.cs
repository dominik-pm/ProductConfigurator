using DatabaseServiceProductConfigurator.Models;
using DatabaseServiceProductConfigurator.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseServiceProductConfigurator.Controllers {

    [EnableCors()]
    [Route("db/[controller]")]
    [ApiController]
    public class LanguageController : ControllerBase {

        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService) {
            _languageService = languageService;
        }

        [HttpGet]
        public ActionResult<List<string>> Get() {
            List<string> list = _languageService.GetAllLanguages();
            if ( list.Count == 0 )
                return NoContent();
            return Ok(list);
        }

    }
}
