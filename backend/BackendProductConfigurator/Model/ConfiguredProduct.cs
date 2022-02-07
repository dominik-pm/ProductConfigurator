using Model.Interfaces;
using System.Text.Json.Serialization;

namespace Model
{
    public class ConfiguredProduct
    {
        public string ConfigurationName { get; set; } = "";
        public List<string> Options { get; set; } = new List<string>();
        public float Price { get; set; }
        public string Model { get; set; }
    }
}
