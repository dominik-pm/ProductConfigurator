using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseServiceProductConfigurator.Services {
    public class BookingService {

        static product_configuratorContext context = new product_configuratorContext();

        public static List<Booking> GetBookingsByCustomer( int customerID ) => context.Bookings.Where(b => b.Customer == customerID).ToList();

        public static object? GetById( int id, string lang ) => context.Bookings
            .Where(b => b.Id.Equals(id))
            .Select(b => new {
                Id = b.Id,
                Customer = b.Customer,
                ConfigId = b.ConfigId,
                Config = ConfigurationService.GetById(b.ConfigId, lang)
            })
            .FirstOrDefault();

    }
}
