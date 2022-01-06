using Model.Interfaces;

namespace Model
{
    public class OptionGroup : IIndexable<int>, INameable, IDescribable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> OptionIds { get; set; } = new List<string>();
        public bool Required { get; set; }
    }
}
