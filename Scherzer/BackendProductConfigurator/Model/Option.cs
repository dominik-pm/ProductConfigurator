using Model.Interfaces;

namespace Model
{
    public class Option : IIndexable<string>, INameable, IDescribable, IImageable
    {
        public Option(string id, string name, string description, List<string> images)
        {
            Id = id;
            Name = name;
            Description = description;
            Images = images;
        }
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<string>? Images { get; set; }
    }
}
