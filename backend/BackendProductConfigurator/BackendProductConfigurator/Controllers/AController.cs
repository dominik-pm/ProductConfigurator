using BackendProductConfigurator.MediaProducers;
using BackendProductConfigurator.Validation;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Interfaces;
using System.Net;

namespace BackendProductConfigurator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public abstract class AController<T, K> : ControllerBase where T : class
    {
        public List<T> entities;

        public AController()
        {
            if(AValuesClass.Configurators.Count == 0)
            {
                AValuesClass.SetValues();
            }
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            // Pass the handler to httpclient(from you are calling api)
            HttpClient client = new HttpClient(clientHandler);
        }


        // GET: api/<Controller>
        [HttpGet]
        public virtual IEnumerable<T> Get()
        {
            Response.Headers["Accept-Language"] = Request.Headers.ContentLanguage; //Richtige Sprache holen
            return entities;
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public virtual T Get(K id)
        {
            Response.Headers["Accept-Language"] = Request.Headers.ContentLanguage; //Richtige Sprache holen
            return entities.Find(entity => (entity as IIndexable).Id.Equals(id));
        }

        // POST api/<Controller>
        [HttpPost]
        public virtual void Post([FromBody] T value)
        {
            entities.Add(value);
            AValuesClass.PostValue<T>(value);
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
            entities.Remove(entities.Find(entity => (entity as IIndexable).Id.Equals(id)));
        }
    }

    public class configurationController : AController<Configurator, string>
    {
        public configurationController() : base()
        {
            entities = AValuesClass.Configurators;
        }

        private void AddConfigurator(Configurator value)
        {
            entities.Add(value);
            AValuesClass.ConfiguratorsSlim.Add(value);
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public override Configurator Get(string id)
        {
            Response.Headers["Accept-Language"] = Request.Headers.ContentLanguage; //Richtige Sprache holen
            return entities.Find(entity => entity.ConfigId.Equals(id));
        }

        // POST api/<Controller>
        [HttpPost]
        public override void Post([FromBody] Configurator value)
        {
            AddConfigurator(value);
            AValuesClass.PostValue<Configurator>(value);
        }
    }
    public partial class productsController : AController<ConfiguratorSlim, string>
    {
        public productsController() : base()
        {
            entities = AValuesClass.ConfiguratorsSlim;
        }
        
        // GET: /products
        [HttpGet]
        public override IEnumerable<ConfiguratorSlim> Get()
        {
            Response.Headers["Accept-Language"] = Request.Headers.ContentLanguage; //Richtige Sprache holen
            return entities;
        }
    }
    public partial class configuredProductsController : AController<ConfiguredProduct, string>
    {
        public configuredProductsController() : base()
        {
            entities = AValuesClass.ConfiguredProducts;
        }

        // POST: /products
        [Route("/products/{configId}")]
        [HttpPost]
        public void Post([FromBody] ConfiguredProduct value, string configId)
        {
            EValidationResult validationResult;
            validationResult = ValidationMethods.ValidateConfiguration(value, AValuesClass.Configurators.Find(config => config.ConfigId == configId).OptionGroups);
            if (validationResult == EValidationResult.ValidationPassed)
            {
                validationResult = ValidationMethods.ValidatePrice(value, AValuesClass.Configurators.Find(config => config.ConfigId == configId).Rules);
            }
            new Thread(() =>
            {
                EmailProducer.SendEmail(value, validationResult);
            }).Start();
            new Thread(() =>
            {
                if(validationResult == EValidationResult.ValidationPassed)
                    PdfProducer.GeneratePDF(value, configId);
            }).Start();
            entities.Add(value);
            AValuesClass.PostValue<ConfiguredProduct>(value);
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
            entities.Add(value);
            AValuesClass.PostValue<Account>(value);
        }
    }
    public partial class savedConfigsController : AController<ProductSaveExtended, string>
    {
        public savedConfigsController() : base()
        {
            entities = AValuesClass.SavedProducts;
        }

        // GET: /account/configuration
        [Route("/account/allorderedconfigurations")]
        [HttpGet]
        public override List<ProductSaveExtended> Get()
        {
            Response.Headers["Accept-Language"] = Request.Headers.ContentLanguage; //Richtige Sprache holen
            return entities;
        }

        // GET: /account/configuration
        [Route("/account/configuration")]
        [HttpGet]
        public List<ProductSave> GetSavedConfigs()
        {
            Response.Headers["Accept-Language"] = Request.Headers.AcceptLanguage; //Richtige Sprache holen
            return entities.Cast<ProductSave>().ToList();
        }

        // POST: /account/configuration
        [Route("/account/configuration/{configId}")]
        [HttpPost]
        public void Post([FromBody] ProductSaveSlim value, string configId)
        {
            string description, name;
            description = AValuesClass.Configurators.Find(con => con.ConfigId == configId).Description;
            name = AValuesClass.Configurators.Find(con => con.ConfigId == configId).Name;
            entities.Add(new ProductSaveExtended() { ConfigId = configId, Date = DateTime.Now, Description = description, Name = name, Options = value.Options, SavedName = value.SavedName, Status = EStatus.Ordered.ToString(), User = new Account() { UserName = "scherzert", UserEmail="test@now.com"} });
        }

        // DELETE api/<Controller>/5
        [HttpDelete("{id}")]
        public override void Delete(string id)
        {
            entities.Remove(entities.Find(entity => entity.SavedName.Equals(id)));
        }
    }

    #region RedactedAPIs

    //Um APIs für eine andere Methoden frei zu machen führen diese Methoden ins nichts

    public partial class configuredProductsController : AController<ConfiguredProduct, string>
    {
        [Route("/redactedConfiguredProducts")]
        [HttpPost]
        public override void Post([FromBody] ConfiguredProduct value) { }
    }
    public partial class productsController : AController<ConfiguratorSlim, string>
    {
        [Route("/redactedProducts")]
        [HttpPost]
        public override void Post([FromBody] ConfiguratorSlim value) { }
    }

        #endregion
    }