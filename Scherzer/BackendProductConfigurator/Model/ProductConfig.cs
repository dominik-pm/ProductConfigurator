using Model.Interfaces;

namespace Model
{
    public class ProductConfig : Product, IProductId
    {
        public ProductConfig(int id, string name, string description, List<string> images, ProductDependencies productDependencies, List<Option> options, List<OptionGroup> optionGroups, List<OptionSection> optionSections)
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
        public ProductDependencies Dependencies { get; set; }
        public List<OptionGroup> OptionGroups { get; set; }
        public List<OptionSection> OptionSections { get; set; }
        public int ProductId { get; set; }
    }
}
