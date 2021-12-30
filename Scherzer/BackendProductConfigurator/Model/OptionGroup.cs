using Model.Interfaces;

namespace Model
{
    public class OptionGroup : IIndexable<string>, INameable, IDescribable
    {
        public OptionGroup(string name, string desciption, string id, List<string> optionIds, bool required)
        {
            Name = name;
            Id = id;
            OptionIds = optionIds;
            Description = desciption;
            Required = required;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> OptionIds { get; set; } = new List<string>();
        public bool Required { get; set; }
    }
}
