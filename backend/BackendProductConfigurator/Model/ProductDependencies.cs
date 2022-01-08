using System.Text.Json.Serialization;

namespace Model
{
    public class ProductDependencies
    {
        public float BasePrice { get; set; }
        public List<string> DefaultOptions { get; set; }
        public Dictionary<string, List<string>> ReplacementGroups { get; set; }
        public Dictionary<string, List<string>> Requirements { get; set; }
        public Dictionary<string, List<string>> Incompabilities { get; set; }
        public Dictionary<string, List<string>> GroupRequirements { get; set; }
        public Dictionary<string, float> PriceList { get; set; }
    }
}