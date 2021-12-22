using Model.Interfaces;

namespace Model
{
    public class Option : IIndexable<string>, INameable, IDescribable, IImageable
    {
        public Option(string id, string name, string description, List<string> images)
        {
            Name = name;
            Description = description;
            Images = images;
            Id = id;
        }

        public List<string> Images { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
