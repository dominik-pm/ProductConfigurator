using Model.Interfaces;

namespace Model
{
    public class OptionSection : IIndexable<int>, INameable
    {
        public OptionSection(string name, int id, List<string> optionGroupIds)
        {
            Name = name;
            Id = id;
            OptionGroupIds = optionGroupIds;
        }
        public string Name { get; set; }
        public int Id { get; set; }
        public List<string> OptionGroupIds { get; set; } = new List<string>();
    }
}
