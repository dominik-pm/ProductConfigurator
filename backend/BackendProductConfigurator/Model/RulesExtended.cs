using Model.Indexes;
using System.Text.Json.Serialization;

namespace Model
{
    public class RulesExtended : Rules
    {
        public List<LanguageIndex> Models { get; set; } = new List<LanguageIndex>();
        public Dictionary<string, List<string>> ReplacementGroups { get; set; } = new Dictionary<string, List<string>>();
    }
}