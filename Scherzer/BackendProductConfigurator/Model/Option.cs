using Model.Interfaces;

namespace Model
{
    public class Option : Product, IIndexable<string>
    {
        public Option(string name, string description, List<string> images, string id)
        {
            Name = name;
            Description = description;
            Images = images;
            Id = id;
        }
    }
}
