using Model.Interfaces;

namespace Model
{
    public class Option : IIndexable, INameable, IDescribable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
