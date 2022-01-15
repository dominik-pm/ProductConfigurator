using System.Text.Json.Serialization;

namespace Model
{
    public class Rules
    {
        public float BasePrice { get; set; }
        public List<string> DefaultOptions { get; set; }
        public Dictionary<string, List<string>> ReplacementGroups { get; set; }
        public Dictionary<string, List<string>> Requirements { get; set; }
        public Dictionary<string, List<string>> Incompatibilities { get; set; }
        public Dictionary<string, List<string>> GroupRequirements { get; set; }
        public Dictionary<string, float> PriceList { get; set; }
    }
}