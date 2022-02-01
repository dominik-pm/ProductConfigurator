using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models {
    public partial class Booking {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public int AccountId { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Configuration Config { get; set; } = null!;
    }
}
