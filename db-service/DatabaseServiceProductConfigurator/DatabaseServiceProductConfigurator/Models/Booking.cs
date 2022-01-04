using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class Booking
    {
        public int Id { get; set; }
        public int Customer { get; set; }
        public int ConfigId { get; set; }

        public virtual Configuration Config { get; set; } = null!;
    }
}
