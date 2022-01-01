using Model.Interfaces;

namespace Model
{
    public class ConfiguredProduct : IConfigId
    {
        public string ConfigurationName { get; set; }
        public List<Option> Options { get; set; } = new List<Option>();
        public float Price { get; set; }
        public int ConfiguratorId { get; set; }
    }
}
