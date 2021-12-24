using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Interfaces;

namespace BackendProductConfigurator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public abstract class Controllers<T, K> : ControllerBase where T : class
    {
        public List<T> entities;

        public Controllers()
        {
            List<T> values = new List<T>(); ;
            if (typeof(T) == typeof(ProductConfig))
            {
                List<Option> options = new List<Option> { new Option("D150", "D150", "Fetter Diesel Motor", new List<string> { "youtube.com" }), new Option("D200", "D200", "Fetter Diesel Motor", new List<string> { "youtube.com" }), new Option("D250", "D250", "Fetter Diesel Motor", new List<string> { "youtube.com" }) };
                List<string> productImages = new List<string> { "google.com" };
                List<OptionGroup> optionGroups = new List<OptionGroup> { new OptionGroup("Color", "The exterior color of the product", "COLOR_GROUP", new List<string> { "RED", "WHITE", "GREEN" }), new OptionGroup("Motor Type", "The motor of your car", "MOTORTYPE_GROUP", new List<string> { "DIESEL", "PETROL", "ELECTRIC" }), new OptionGroup("Motor", "The selected Motor power", "MOTOR_GROUP", new List<string> { "D150", "D200", "D250" }) };
                List<OptionSection> optionSections = new List<OptionSection> { new OptionSection("Exterior", "EXTERIOR", new List<string> { "COLOR_GROUP" }), new OptionSection("Motor", "MOTOR_SECTION", new List<string> { "MOTORTYPE_GROUP", "MOTOR_GROUP" }) };
                ProductDependencies productDependencies = new ProductDependencies(50000, new List<string> { "RED", "DIESEL", "D150" }, new Dictionary<string, List<string>> { { "COLOR_GROUP", new List<string> { "ey", "wos" } } }, new Dictionary<string, List<string>> { { "D150", new List<string> { "DIESEL" } } }, new Dictionary<string, List<string>> { { "D150", new List<string> { "PETROL" } } }, new Dictionary<string, int> { { "D150", 1500 } });
                ProductConfig productConfig = new ProductConfig(0, "Alfa Romeo 159 Configurator", "Configurable Car", productImages, productDependencies, options, optionGroups, optionSections);

                values = new List<T> { productConfig as T };
            }
            if (typeof(T) == typeof(Product))
            {
                List<Option> optionsList = new List<Option> { new Option("Option1", "Erste Option", "Ka Ahnung wos des duat", new List<string> { "Zehner.net", "Cool.com" } ) };

                Product p1 = new Product() { Id = 0, Name = "Fetter Benz", Description = "Laut und groß", Images = new List<string> { "Zehner.net", "Cool.com"}, Options = optionsList };
                Product p2 = new Product() { Id = 1, Name = "Eleganter Alfa Romeo", Description = "Stylisch und erweckt", Images = new List<string> { "Zehner.net", "Cool.com" }, Options = optionsList };
                Product p3 = new Product() { Id = 2, Name = "Fetterer Benz", Description = "Umso lauter und größer", Images = new List<string> { "Zehner.net", "Cool.com" }, Options = optionsList };

                values = new List<T> { p1 as T, p2 as T, p3 as T };
            }
            if(typeof(T) == typeof(ProductSlim))
            {
                ProductSlim p1 = new ProductSlim() { Id = 0, Name = "Fetter Benz", Description = "Laut und groß" };
                ProductSlim p2 = new ProductSlim() { Id = 1, Name = "Eleganter Alfa Romeo", Description = "Stylisch und erweckt" };
                ProductSlim p3 = new ProductSlim() { Id = 2, Name = "Fetterer Benz", Description = "Umso lauter und größer" };

                values = new List<T> { p1 as T, p2 as T, p3 as T };
            }

            this.entities = values;
        }


        // GET: api/<Controller>
        [HttpGet]
        public IEnumerable<T> Get()
        {
            return entities;
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public T Get(K id)
        {
            return entities.Find(entity => (entity as IIndexable<K>).Id.Equals(id));
        }

        // POST api/<Controller>
        [HttpPost]
        public void Post([FromBody] T value)
        {
            entities.Append(value);

        }

        // PUT api/<Controller>/5
        [HttpPut("{id}")]
        public void Put(K id, [FromBody] T value)
        {
            Delete(id);
            Post(value);
        }

        // DELETE api/<Controller>/5
        [HttpDelete("{id}")]
        public void Delete(K id)
        {
            entities.Remove(entities.Find(entity => (entity as IIndexable<K>).Id.Equals(id)));
        }
    }

    public class configurationController : Controllers<ProductConfig, int>
    {
    }
    public class productsController : Controllers<Product, int>
    {
    }
    public class productsSlimController : Controllers<ProductSlim, int>
    {
    }
}
//Issue #9 und #10 anschauen