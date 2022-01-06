using Model.Interfaces;

namespace Model
{
    public class OptionSection : IIndexable<string>, INameable
    {
        public OptionSection(string name, string id, List<string> optionGroupIds)
        {
            Name = name;
            Id = id;
            OptionGroupIds = optionGroupIds;
        }
        public string Name { get; set; }
        public string Id { get; set; }
        public List<string> OptionGroupIds { get; set; } = new List<string>();
    }
}
