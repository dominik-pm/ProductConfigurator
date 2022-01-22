using System.Text.Json.Serialization;

namespace Model
{
    public class Rules
    {
        public float BasePrice { get; set; }
        public List<ModelType> Models { get; set; } = new List<ModelType>();
        public Dictionary<string, List<string>> ReplacementGroups { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Requirements { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Incompatibilities { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> GroupRequirements { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, float> PriceList { get; set; } = new Dictionary<string, float>();
    }
}