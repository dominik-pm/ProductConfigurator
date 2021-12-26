using Model.Interfaces;

namespace Model
{
    public class Product : ProductSlim, IImageable
    {
        public virtual List<string> Images { get; set; } = new List<string> { "Abstract Imagepath1", "Abstract Imagepath 2"};
        public List<Option> Options { get; set; }
        public float Price { get; set; }
    }
}
