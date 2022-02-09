using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class Configuration
    {
        public Configuration()
        {
            Bookings = new HashSet<Booking>();
            ConfigurationHasOptionFields = new HashSet<ConfigurationHasOptionField>();
            ConfigurationsHasLanguages = new HashSet<ConfigurationsHasLanguage>();
        }

        public int Id { get; set; }
        public string ProductNumber { get; set; } = null!;
        public DateTime Date { get; set; }
        public int? AccountId { get; set; }
        public bool Visible { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Product ProductNumberNavigation { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<ConfigurationHasOptionField> ConfigurationHasOptionFields { get; set; }
        public virtual ICollection<ConfigurationsHasLanguage> ConfigurationsHasLanguages { get; set; }
    }
}
