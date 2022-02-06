using Model.Interfaces;

namespace Model
{
    public class Configurator : ConfiguratorBase
    {
        public RulesExtended Rules { get; set; }
        public List<OptionGroup> OptionGroups { get; set; }
    }
}