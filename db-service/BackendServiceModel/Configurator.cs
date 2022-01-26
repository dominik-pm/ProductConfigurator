using Model.Interfaces;

namespace Model
{
    public class Configurator : ConfiguratorSlim
    {
        public List<Option> Options { get; set; }
        public Rules Rules { get; set; }
        public List<OptionGroup> OptionGroups { get; set; }
        public List<OptionSection> OptionSections { get; set; }
    }
}