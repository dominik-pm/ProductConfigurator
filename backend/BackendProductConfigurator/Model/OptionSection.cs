using Model.Interfaces;

namespace Model
{
    public class OptionSection : IIndexable, INameable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> OptionGroupIds { get; set; } = new List<string>();
    }
}
