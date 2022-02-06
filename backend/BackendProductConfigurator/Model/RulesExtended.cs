using System.Text.Json.Serialization;

namespace Model
{
    public class RulesExtended : Rules
    {
        public Dictionary<string, List<string>> ReplacementGroups { get; set; } = new Dictionary<string, List<string>>();
    }
}