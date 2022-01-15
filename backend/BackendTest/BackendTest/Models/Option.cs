namespace BackendTest.Models
{
    public class Option : AProduct
    {
        public Option(string name, string description, List<string> images, string id)
        {
            Name = name;
            Description = description;
            Images = images;
            Id = id;
        }
        public override string Name { get; set; } = "Overriden Name";
        public override string Description { get; set; } = "Overriden Desc.";
        public override List<string> Images { get; set; } = new List<string> { "Overriden Imagepath1", "Overriden Imagepath2"};
        public override string Id { get; set; } = "Overriden Id";
    }
}
