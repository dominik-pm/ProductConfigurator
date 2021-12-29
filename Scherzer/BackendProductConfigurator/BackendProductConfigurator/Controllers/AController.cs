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
            if(AValuesClass.Products.Count == 0)
            {
                AValuesClass.SetValues();
            }
        }


        // GET: api/<Controller>
        [HttpGet]
        public IEnumerable<T> Get()
        {
            Response.Headers["Content-language"] = Request.Headers.ContentLanguage; //nach richtiger Sprache abgleichen
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
        public configurationController():base()
        {
            entities = AValuesClass.ProductConfig as List<ProductConfig>;
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public override ProductConfig Get(int id)
        {
            return entities.Find(entity => (entity as IProductId).ProductId.Equals(id));
        }
    }
    public class productsController : AController<Product, int>
    {
        public productsController() : base()
        {
            entities = AValuesClass.Products as List<Product>;
        }

        // POST api/<Controller>
        [HttpPost]
        public override void Post([FromBody] Product value)
        {
            AValuesClass.Products.Add(value); //Controller wird bei jeder Anfrage neu instanziert --> Externe Klasse mit statischen Listen wird vorerst benötigt
            entities.Append(value);
        }
    }
    public class productsSlimController : AController<ProductSlim, int>
    {
        public productsSlimController() : base()
        {
            entities = AValuesClass.ProductsSlim as List<ProductSlim>;
        }
    }
    public class usersController : AController<User, int>
    {
        public usersController() : base()
        {
            entities = AValuesClass.Users as List<User>;
        }

        // POST api/<Controller>
        [HttpPost]
        public override void Post([FromBody] User value)
        {
            AValuesClass.Users.Add(value); //Controller wird bei jeder Anfrage neu instanziert --> Externe Klasse mit statischen Listen wird vorerst benötigt
            entities.Append(value);
        }
    }
}