using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class Booking
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public int? AccountId { get; set; }
#warning Not null! please change

        public virtual Account? Account { get; set; }
        public virtual Configuration Config { get; set; } = null!;
    }
}
