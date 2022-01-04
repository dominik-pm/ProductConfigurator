using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class Picture
    {
        public int Id { get; set; }
        public string ProductNumber { get; set; } = null!;
        public string Url { get; set; } = null!;

        [JsonIgnore]
        public virtual Product ProductNumberNavigation { get; set; } = null!;
    }
}
