using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseServiceProductConfigurator.Services {

    //public struct BookingStruct {
    //    public int id { get; set; }
    //    public int customer { get; set; }
    //    public int configId { get; set; }
    //    public ConfigStruct? config { get; set; }
    //}

    public class BookingService {

        static product_configuratorContext context = new product_configuratorContext();

        //public static List<object> GetBookingsByCustomer( int customerID ) => context.Bookings.Where(b => b.Customer == customerID).Select(b => new {
        //    id = b.Id,
        //    customer = b.Customer,
        //    config = b.ConfigId
        //}).ToList<object>();

        //public static BookingStruct? GetById( int id, string lang ) => context.Bookings
        //    .Where(b => b.Id.Equals(id))
        //    .Select(b => new BookingStruct {
        //        id = b.Id,
        //        customer = b.Customer,
        //        configId = b.ConfigId,
        //        config = ConfigurationService.GetById(b.ConfigId, lang)
        //    })
        //    .FirstOrDefault();

    }
}
