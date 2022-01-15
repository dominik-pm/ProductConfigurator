using BackendTest.Models.Interfaces;

namespace BackendTest.Models
{
    public abstract class AProduct : IIndexable, INameable, IDescribable, IImageable
    {
        public virtual string Name { get; set; } = "Abstract Product";
        public virtual string Description { get; set; } = "Abstract Description";
        public virtual List<string> Images { get; set; } = new List<string> { "Abstract Imagepath1", "Abstract Imagepath 2"};
        public virtual string Id { get; set; } = "Abstract Id";
        public List<Option> Options { get; set; }
    }
}
