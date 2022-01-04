using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class Configuration {
        public Configuration() {
            Bookings = new HashSet<Booking>();
            ConfigurationHasOptionFields = new HashSet<ConfigurationHasOptionField>();
        }

        public int Id { get; set; }
        public string ProductNumber { get; set; } = null!;
        public int? Customer { get; set; }

        [JsonIgnore]
        public virtual Product ProductNumberNavigation { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<ConfigurationHasOptionField> ConfigurationHasOptionFields { get; set; }
    }
}
