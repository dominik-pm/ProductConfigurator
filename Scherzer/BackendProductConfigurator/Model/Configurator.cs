using Model.Interfaces;

namespace Model
{
    public class Configurator : ProductSlim
    {
        public List<Option> Options { get; set; }
        public ProductDependencies Dependencies { get; set; }
        public List<OptionGroup> OptionGroups { get; set; }
        public List<OptionSection> OptionSections { get; set; }
    }
}