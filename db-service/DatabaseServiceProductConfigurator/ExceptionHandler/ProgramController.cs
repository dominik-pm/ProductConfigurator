using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseServiceProductConfigurator.ExceptionHandler {

    [ApiController]
    public class ProgramController : ControllerBase {

        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleError() {

            var exceptionHandlerFeature =
                HttpContext.Features.Get<IExceptionHandlerFeature>()!;


            Console.WriteLine(exceptionHandlerFeature.Error.StackTrace);
            Console.WriteLine(exceptionHandlerFeature.Error.Message);

            return Problem();
        }

        [Route("/error-development")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleErrorDevelopment( [FromServices] IHostEnvironment hostEnvironment ) {
            if ( !hostEnvironment.IsDevelopment() ) {
                return NotFound();
            }

            var exceptionHandlerFeature =
                HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            Console.WriteLine(exceptionHandlerFeature.Error.StackTrace);
            Console.WriteLine(exceptionHandlerFeature.Error.Message);

            return Problem(
                detail: exceptionHandlerFeature.Error.StackTrace,
                title: exceptionHandlerFeature.Error.Message);
        }

    }
}
