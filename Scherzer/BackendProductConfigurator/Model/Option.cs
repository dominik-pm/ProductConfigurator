using Model.Interfaces;

namespace Model
{
    public class Option : IIndexable<string>, INameable, IDescribable
    {
        public Option(string id, string name, string description)
        {
            ConfigId = id;
            Name = name;
            Description = description;
        }
        public string ConfigId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
