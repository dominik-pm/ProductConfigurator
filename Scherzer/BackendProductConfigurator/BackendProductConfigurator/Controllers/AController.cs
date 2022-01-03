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
                AValuesClass.SetStaticValues();
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
            return entities.Find(entity => (entity as IIndexable<K>).ConfigId.Equals(id));
        }

        // POST api/<Controller>
        [HttpPost]
        public virtual void Post([FromBody] T value)
        {
            entities.Add(value);
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
            entities.Remove(entities.Find(entity => (entity as IIndexable<K>).ConfigId.Equals(id)));
        }
    }

    public class configurationController : AController<Configurator, int>
    {
        public configurationController() : base()
        {
            entities = AValuesClass.Configurators;
        }

        // POST api/<Controller>
        [HttpPost]
        public override void Post([FromBody] Configurator value)
        {
            entities.Add(value);
            AValuesClass.Configurators.Add(value);
            AValuesClass.ProductsSlim.Add(value);
        }
    }
    public partial class productsController : AController<ConfiguratorSlim, int>
    {
        public productsController() : base()
        {
            entities = AValuesClass.ProductsSlim;
        }
        
        // GET: /products
        [HttpGet]
        public override IEnumerable<ConfiguratorSlim> Get()
        {
            Response.Headers["Content-language"] = Request.Headers.ContentLanguage; //nach richtiger Sprache abgleichen
            return entities;
        }
    }
    public partial class configuredProductsController : AController<ConfiguredProduct, int>
    {
        public configuredProductsController() : base()
        {
            entities = AValuesClass.ConfiguredProducts;
        }

        // POST: /products
        [Route("/products/{configId}")]
        [HttpPost]
        public void Post([FromBody] ConfiguredProduct value, int configId)
        {
            //AValuesClass.ConfiguredProducts.Add(value); //Controller wird bei jeder Anfrage neu instanziert --> Externe Klasse mit statischen Listen wird vorerst benötigt
            new Thread(() =>
            {
                EValidationResult validationResult;
                validationResult = ValidationMethods.ValidateConfiguration(value, AValuesClass.Configurators.Find(config => config.ConfigId == configId).OptionGroups);
                if (validationResult == EValidationResult.ValidationPassed)
                {
                    validationResult = ValidationMethods.ValidatePrice(value, AValuesClass.Configurators.Find(config => config.ConfigId == configId).Dependencies);
                }
                EmailProducer.SendEmail(value, validationResult);
            }).Start();
            new Thread(() =>
            {
                PdfProducer.GeneratePDF(value, configId);
            }).Start();
            entities.Add(value); //later here without configuratorId => new { ... }
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
            //AValuesClass.Accounts.Add(value); //Controller wird bei jeder Anfrage neu instanziert --> Externe Klasse mit statischen Listen wird vorerst benötigt
            entities.Add(value);
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
        [Route("/account/configuration/{configId}")]
        [HttpPost]
        public void Post([FromBody] ProductSaveSlim value, int configId)
        {
            string description, name;
            description = AValuesClass.Configurators.Find(con => con.ConfigId == configId).Description;
            name = AValuesClass.Configurators.Find(con => con.ConfigId == configId).Name;
            //AValuesClass.SavedProducts.Add(new ProductSave(value)); //Controller wird bei jeder Anfrage neu instanziert --> Externe Klasse mit statischen Listen wird vorerst benötigt
            entities.Add(new ProductSave() { ConfigId = configId, Date = DateTime.Now, Description = description, Name = name, Options = value.Options, SavedName = value.SavedName, Status = EStatus.Ordered.ToString() });
        }
    }

    #region RedactedAPIs

    //Um APIs für eine andere Methoden frei zu machen führen diese Methoden ins nichts

    public partial class configuredProductsController : AController<ConfiguredProduct, int>
    {
        [Route("/redactedConfiguredProducts")]
        [HttpPost]
        public override void Post([FromBody] ConfiguredProduct value) { }
    }
        public partial class productsController : AController<ConfiguratorSlim, int>
    {
        [Route("/redactedProducts")]
        [HttpPost]
        public override void Post([FromBody] ConfiguratorSlim value) { }
    }

    #endregion
}