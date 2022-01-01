using Model.Interfaces;

namespace Model
{
    public class Configurator : ProductSlim
    {
        public Configurator(int id, string name, string description, List<string> images, ProductDependencies productDependencies, List<Option> options, List<OptionGroup> optionGroups, List<OptionSection> optionSections)
        {
            Id = id;
            Name = name;
            Description = description;
            Images = images;
            Dependencies = productDependencies;
            Options = options;
            OptionGroups = optionGroups;
            OptionSections = optionSections;
        }
        public List<Option> Options { get; set; } = new List<Option>();
        public ProductDependencies Dependencies { get; set; }
        public List<OptionGroup> OptionGroups { get; set; }
        public List<OptionSection> OptionSections { get; set; }
    }
}
