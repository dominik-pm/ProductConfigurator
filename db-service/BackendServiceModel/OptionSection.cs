using Model.Interfaces;

namespace Model
{
    public class OptionSection : IIndexable, INameable
    {
        public OptionSection(string name, string id, List<string> optionGroupIds)
        {
            Name = name;
            Id = id;
            OptionGroupIds = optionGroupIds;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> OptionGroupIds { get; set; } = new List<string>();
    }
}
