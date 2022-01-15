using BackendTest.Models.Interfaces;

namespace BackendTest.Models
{
    public class OptionGroup : IIndexable, INameable, IDescribable
    {
        public OptionGroup(string name, string desciption, string id, List<string> optionIds)
        {
            Name = name;
            Id = id;
            OptionIds = optionIds;
            Description = desciption;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> OptionIds { get; set; } = new List<string>();
    }
}
