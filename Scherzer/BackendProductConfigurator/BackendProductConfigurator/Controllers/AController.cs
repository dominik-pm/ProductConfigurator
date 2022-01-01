using BackendProductConfigurator.MediaProducers;
using BackendProductConfigurator.Validation;
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
            if(AValuesClass.ConfiguredProducts.Count == 0)
            {
                AValuesClass.SetValues();
            }
        }


        // GET: api/<Controller>
        [HttpGet]
        public virtual IEnumerable<T> Get()
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

    public class configurationController : AController<Configurator, int>
    {
        private List<ProductSlim> productSlims = new List<ProductSlim>();
        public configurationController():base()
        {
            entities = AValuesClass.Configurators;
            productSlims = AValuesClass.ProductsSlim;
        }

        // GET: api/<Controller>
        [HttpGet]
        public List<ProductSlim> Get()
        {
            Response.Headers["Content-language"] = Request.Headers.ContentLanguage; //nach richtiger Sprache abgleichen
            return productSlims;
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public override Configurator Get(int id)
        {
            return entities.Find(entity => (entity as IConfigId).ConfiguratorId.Equals(id));
        }
    }
    public class productsController : AController<ConfiguredProduct, int>
    {
        public productsController() : base()
        {
            entities = AValuesClass.ConfiguredProducts;
        }

        // POST api/<Controller>
        [HttpPost]
        public override void Post([FromBody] ConfiguredProduct value)
        {
            AValuesClass.ConfiguredProducts.Add(value); //Controller wird bei jeder Anfrage neu instanziert --> Externe Klasse mit statischen Listen wird vorerst benötigt
            new Thread(() =>
            {
                EValidationResult validationResult;
                validationResult = ValidationMethods.ValidateConfiguration(value, AValuesClass.Configurators.Find(config => config.Id == value.ConfiguratorId).OptionGroups);
                if(validationResult == EValidationResult.ValidationPassed)
                {
                    validationResult = ValidationMethods.ValidatePrice(value, AValuesClass.Configurators.Find(config => config.Id == value.ConfiguratorId).Dependencies);
                }
                EmailProducer.SendEmail(value, validationResult);
            }).Start();
            new Thread(() =>
            {
                PdfProducer.GeneratePDF(value);
            }).Start();
            entities.Append(value); //later here without configuratorId => new { ... }
        }
    }
    public class accountController : AController<Account, int>
    {
        public accountController() : base()
        {
            entities = AValuesClass.Accounts;
        }

        // POST /account
        [HttpPost]
        public override void Post([FromBody] Account value)
        {
            AValuesClass.Accounts.Add(value); //Controller wird bei jeder Anfrage neu instanziert --> Externe Klasse mit statischen Listen wird vorerst benötigt
            entities.Append(value);
        }
    }
    public class savedConfigsController : AController<ProductSave, int>
    {
        public savedConfigsController() : base()
        {
            entities = AValuesClass.SavedProducts;
        }

        // GET: /account/configuration
        [Route("/account/configuration")]
        [HttpGet]
        public override List<ProductSave> Get()
        {
            Response.Headers["Content-language"] = Request.Headers.ContentLanguage; //nach richtiger Sprache abgleichen
            return entities;
        }

        // POST: /account/configuration
        [Route("/account/configuration")]
        [HttpPost]
        public void Post([FromBody] ProductSaveSlim value)
        {
            AValuesClass.SavedProducts.Add(new ProductSave(value)); //Controller wird bei jeder Anfrage neu instanziert --> Externe Klasse mit statischen Listen wird vorerst benötigt
            entities.Append(value);
        }
    }
}