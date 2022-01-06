using DatabaseServiceProductConfigurator.Models;
using DatabaseServiceProductConfigurator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseServiceProductConfigurator.Controllers {
    [Route("db/[controller]")]
    [ApiController]
    public class BookingController : AController<Booking, int> {

        static product_configuratorContext context = new product_configuratorContext();

        public BookingController () : base(context) { }

        [HttpGet("{id}")]
        public override IActionResult Get( int id ) {
            Request.Headers.TryGetValue("Accept-Language", out var lang);
            lang = LanguageService.HandleLanguageInput(lang);

            BookingStruct? toReturn = BookingService.GetById(id, lang);
            if ( toReturn == null )
                return NotFound();
            else
                return Ok(toReturn);
        }

        [HttpGet("getByCustomer/{customerID}")]
        public ActionResult GetBookingsByCustomer(int customerID) {
            List<object> bookings = BookingService.GetBookingsByCustomer(customerID);
            if ( bookings.Count == 0 )
                return NoContent();
            return Ok(bookings);
        }

    }
}
