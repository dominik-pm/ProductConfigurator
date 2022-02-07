using BackendProductConfigurator.MediaProducers;
using BackendProductConfigurator.Validation;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Interfaces;
using Model.Wrapper;
using System.Net;

namespace BackendProductConfigurator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public abstract class AController<T, K> : ControllerBase where T : class
    {
        public Dictionary<string, List<T>> entities;

        public AController()
        {
            if(AValuesClass.Configurators["de"].Count == 0)
            {
                AValuesClass.SetValues();
            }
        }


        // GET: api/<Controller>
        [HttpGet]
        public virtual IEnumerable<T> Get()
        {
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            return entities[GetAccLang(Request)];
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public virtual T Get(K id)
        {
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            return entities[GetAccLang(Request)].Find(entity => (entity as IIndexable).Id.Equals(id));
        }

        // POST api/<Controller>
        [HttpPost]
        public virtual void Post([FromBody] T value)
        {
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            entities[GetAccLang(Request)].Add(value);
            AValuesClass.PostValue<T>(value, GetAccLang(Request));
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
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            entities[GetAccLang(Request)].Remove(entities[GetAccLang(Request)].Find(entity => (entity as IIndexable).Id.Equals(id)));
        }

        public static string GetAccLang(HttpRequest request)
        {
            if(request.Headers.AcceptLanguage.ToString().Contains("-"))
                return request.Headers.AcceptLanguage.ToString().Split(",")[0].Trim('{').Split("-")[0];
            else
                return request.Headers.AcceptLanguage.ToString();
        }
    }

    public partial class configurationController : AController<Configurator, string>
    {
        public configurationController() : base()
        {
            entities = AValuesClass.Configurators;
        }

        private void AddConfigurator(Configurator value)
        {
            entities[GetAccLang(Request)].Add(value);
            AValuesClass.ConfiguratorsSlim[GetAccLang(Request)].Add(value);
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public override Configurator Get(string id)
        {
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            return entities[GetAccLang(Request)].Find(entity => entity.ConfigId.Equals(id));
        }

        // POST api/<Controller>
        [HttpPost]
        public void Post([FromBody] ConfiguratorPost value)
        {
            Configurator configurator = AValuesClass.GenerateConfigurator(value);

            EValidationResult validationResult = ValidationMethods.ValidateConfigurator(configurator);
            if(validationResult == EValidationResult.ValidationPassed)
            {
                AddConfigurator(configurator);
                AValuesClass.PostValue<Configurator>(configurator, GetAccLang(Request));
            }
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
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            return entities[GetAccLang(Request)];
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
            Configurator configurator = AValuesClass.Configurators[GetAccLang(Request)].Find(config => config.ConfigId == configId);
            validationResult = ValidationMethods.ValidateConfiguration(value, configurator);
            if (validationResult == EValidationResult.ValidationPassed)
            {
                validationResult = ValidationMethods.ValidatePrice(value, configurator.Rules);
            }
            if (validationResult == EValidationResult.ValidationPassed)
            {
                validationResult = ValidationMethods.ValidateSelectedModel(value, configurator);
            }
            //new Thread(() =>
            //{
            //    EmailProducer.SendEmail(value, validationResult);
            //}).Start();
            //new Thread(() =>
            //{
            //    validationResult = EValidationResult.ValidationPassed;
            //    if(validationResult == EValidationResult.ValidationPassed)
            //        PdfProducer.GeneratePDF(value, configId, Request);
            //}).Start();
            entities[GetAccLang(Request)].Add(value);

            //Account account = AValuesClass.FillAccountFromToken(Request.Headers["Authorization"]);

            savedConfigsController scc = new savedConfigsController();
            Account tempAccount = new Account() { UserName = "testUser", UserEmail = "test@user.com" };
            ProductSaveExtended temp = new ProductSaveExtended()
            {
                Status = EStatus.ordered.ToString(),
                Date = DateTime.Now,
                ConfigId = configurator.ConfigId,
                Name = configurator.Name,
                Description = configurator.Description,
                Options = value.Options,
                SavedName = value.ConfigurationName,
                User = tempAccount
            };
            scc.PostOrdered(temp, Request);
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
            entities[GetAccLang(Request)].Add(value);
            AValuesClass.PostValue<Account>(value, GetAccLang(Request));
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
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            return entities[GetAccLang(Request)];
        }

        // GET: /account/configuration
        [Route("/account/configurations")]
        [HttpGet]
        public List<ProductSaveExtended> GetSavedConfigs()
        {
            Account account = AValuesClass.FillAccountFromToken(Request.Headers["Authorization"]);

            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            return entities[GetAccLang(Request)];
        }

        // POST: /account/configuration
        [Route("/account/configurations/{configId}")]
        [HttpPost]
        public void Post([FromBody] ProductSaveSlim value, string configId)
        {
            string description, name;
            description = AValuesClass.Configurators[GetAccLang(Request)].Find(con => con.ConfigId == configId).Description;
            name = AValuesClass.Configurators[GetAccLang(Request)].Find(con => con.ConfigId == configId).Name;
            ProductSaveExtended temp = new ProductSaveExtended() { ConfigId = configId, Date = DateTime.Now, Description = description, Name = name, Options = value.Options, SavedName = value.SavedName, Status = EStatus.saved.ToString(), User = new Account() { UserName = "testUser", UserEmail = "test@user.com" } };
            entities[GetAccLang(Request)].Add(temp);
            AValuesClass.PostValue(temp, GetAccLang(Request));
        }

        [NonAction]
        public void PostOrdered(ProductSaveExtended value, HttpRequest Request)
        {
            entities[GetAccLang(Request)].Add(value);
            AValuesClass.PostValue<ProductSaveExtended>(value, GetAccLang(Request));
        }

        // DELETE api/<Controller>/5
        [Route("/account/configurations/{id}")]
        [HttpDelete]
        public void SavedConfigDelete([FromBody] SavedNameWrapper requestBody, string id)
        {
            entities[GetAccLang(Request)].Remove(entities[GetAccLang(Request)].Find(entity => entity.ConfigId == id && entity.SavedName == requestBody.SavedName));
            AValuesClass.DeleteValue<SavedConfigWrapper>(GetAccLang(Request), new SavedConfigWrapper() { ConfigId = id, SavedName = requestBody.SavedName });
        }
    }

    #region RedactedAPIs

    //Um APIs für eine andere Methoden frei zu machen führen diese Methoden ins nichts

    public partial class configuredProductsController : AController<ConfiguredProduct, string>
    {
        [Route("/redactedConfiguredProducts")]
        [HttpPost]
        [NonAction]
        public override void Post([FromBody] ConfiguredProduct value) { }
    }
    public partial class productsController : AController<ConfiguratorSlim, string>
    {
        [Route("/redactedProducts")]
        [HttpPost]
        [NonAction]
        public override void Post([FromBody] ConfiguratorSlim value) { }
    }
    public partial class configurationController : AController<Configurator, string>
    {
        // POST api/<Controller>
        [Route("/redactedConfigurator")]
        [HttpPost]
        [NonAction]
        public override void Post([FromBody] Configurator value)
        {
            EValidationResult validationResult = ValidationMethods.ValidateConfigurator(value);
            if (validationResult == EValidationResult.ValidationPassed)
            {
                AddConfigurator(value);
                AValuesClass.PostValue<Configurator>(value, GetAccLang(Request));
            }
        }
    }
    public partial class savedConfigsController : AController<ProductSaveExtended, string>
    {
        // DELETE api/<Controller>/5
        [Route("/redactedSavedConfigsController")]
        [HttpDelete]
        [NonAction]
        public override void Delete(string id)
        {
            entities[GetAccLang(Request)].Remove(entities[GetAccLang(Request)].Find(entity => entity.SavedName.Equals(id)));
        }
    }

    #endregion
}