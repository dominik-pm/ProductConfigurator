using Model.Interfaces;

namespace Model
{
    public class OptionGroup : OptionGroupBase, INameable, IDescribable
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
