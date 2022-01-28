using Model.Interfaces;
using System.Text.Json.Serialization;

namespace Model
{
    public class ConfiguredProduct
    {
        public string ConfigurationName { get; set; } = "";
        public List<Option> Options { get; set; } = new List<Option>();
        public float Price { get; set; }
        public string SelectedModel { get; set; }
    }
}
