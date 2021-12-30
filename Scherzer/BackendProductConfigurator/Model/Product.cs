using Model.Interfaces;

namespace Model
{
    public class Product : ProductSlim, IImageable, IConfigId
    {
        public virtual List<string> Images { get; set; } = new List<string>();
        public List<Option> Options { get; set; } = new List<Option>();
        public float Price { get; set; }
        public int ConfigId { get; set; }
    }
}
