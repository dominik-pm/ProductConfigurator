﻿using BackendProductConfigurator.MediaProducers;
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
        public Dictionary<string, List<T>> entities;

        public AController()
        {
            if(AValuesClass.Configurators["de"].Count == 0)
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

    public class configurationController : AController<Configurator, string>
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
        public override void Post([FromBody] Configurator value)
        {
            EValidationResult validationResult = ValidationMethods.ValidateConfigurator(value);
            if(validationResult == EValidationResult.ValidationPassed)
            {
                AddConfigurator(value);
                AValuesClass.PostValue<Configurator>(value, GetAccLang(Request));
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
            validationResult = ValidationMethods.ValidateConfiguration(value, AValuesClass.Configurators[GetAccLang(Request)].Find(config => config.ConfigId == configId));
            if (validationResult == EValidationResult.ValidationPassed)
            {
                validationResult = ValidationMethods.ValidatePrice(value, AValuesClass.Configurators[GetAccLang(Request)].Find(config => config.ConfigId == configId).Rules);
            }
            if (validationResult == EValidationResult.ValidationPassed)
            {
                validationResult = ValidationMethods.ValidateSelectedModel(value, AValuesClass.Configurators[GetAccLang(Request)].Find(config => config.ConfigId == configId));
            }
            new Thread(() =>
            {
                EmailProducer.SendEmail(value, validationResult);
            }).Start();
            new Thread(() =>
            {
                validationResult = EValidationResult.ValidationPassed;
                if(validationResult == EValidationResult.ValidationPassed)
                    PdfProducer.GeneratePDF(value, configId, Request);
            }).Start();
            entities[GetAccLang(Request)].Add(value);
            //AValuesClass.PostValue<ConfiguredProduct>(value, GetAccLang(Request));
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
        [Route("/account/configuration")]
        [HttpGet]
        public List<ProductSave> GetSavedConfigs()
        {
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            return entities.Cast<ProductSave>().ToList();
        }

        // POST: /account/configuration
        [Route("/account/configuration/{configId}")]
        [HttpPost]
        public void Post([FromBody] ProductSaveSlim value, string configId)
        {
            string description, name;
            description = AValuesClass.Configurators[GetAccLang(Request)].Find(con => con.ConfigId == configId).Description;
            name = AValuesClass.Configurators[GetAccLang(Request)].Find(con => con.ConfigId == configId).Name;
            ProductSaveExtended temp = new ProductSaveExtended() { ConfigId = configId, Date = DateTime.Now, Description = description, Name = name, Options = value.Options, SavedName = value.SavedName, Status = EStatus.Ordered.ToString(), User = new Account() { UserName = "scherzert", UserEmail = "test@now.com" } };
            entities[GetAccLang(Request)].Add(temp);
            AValuesClass.PostValue(temp, GetAccLang(Request));
        }

        // DELETE api/<Controller>/5
        [HttpDelete("{id}")]
        public override void Delete(string id)
        {
            entities[GetAccLang(Request)].Remove(entities[GetAccLang(Request)].Find(entity => entity.SavedName.Equals(id)));
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