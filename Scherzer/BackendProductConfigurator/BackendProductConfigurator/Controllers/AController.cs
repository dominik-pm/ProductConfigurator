using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Interfaces;

namespace BackendProductConfigurator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public abstract class AController<T, K> : ControllerBase where T : class
    {
        public List<T> entities;

        public AController()
        {
            if(AValuesClass.products.Count == 0)
            {
                AValuesClass.SetValues();
            }
            List<T> values = new List<T>();
            if (typeof(T) == typeof(ProductConfig))
            {
                values = AValuesClass.productConfig as List<T>;
            }
            if (typeof(T) == typeof(Product))
            {
                values = AValuesClass.products as List<T>;
            }
            if(typeof(T) == typeof(ProductSlim))
            {
                values = AValuesClass.productsSlim as List<T>;
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
        public virtual T Get(K id)
        {
            return entities.Find(entity => (entity as IIndexable<K>).Id.Equals(id));
        }

        // POST api/<Controller>
        [HttpPost]
        public virtual void Post([FromBody] T value)
        {
            entities.Append(value);
        }

        // PUT api/<Controller>/5
        [HttpPut("{id}")]
        public virtual void Put(K id, [FromBody] T value)
        {
            Delete(id);
            Post(value);
        }

        // DELETE api/<Controller>/5
        [HttpDelete("{id}")]
        public virtual void Delete(K id)
        {
            entities.Remove(entities.Find(entity => (entity as IIndexable<K>).Id.Equals(id)));
        }
    }

    public class configurationController : AController<ProductConfig, int>
    {
    }
    public class productsController : AController<Product, int>
    {
        [HttpPost]
        public override void Post([FromBody] Product value)
        {
            AValuesClass.products.Add(value); //Controller wird bei jeder Anfrage neu instanziert --> Externe Klasse mit statischen Listen wird benötigt
            entities.Append(value);
        }
    }
    public class productsSlimController : AController<ProductSlim, int>
    {
    }
}
//Issue #9 und #10 anschauen