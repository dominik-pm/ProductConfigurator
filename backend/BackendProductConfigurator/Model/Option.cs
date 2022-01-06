using Model.Interfaces;

namespace Model
{
    public class Option : IIndexable<string>, INameable, IDescribable
    {
        public Option(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
