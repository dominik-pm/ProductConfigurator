﻿using BackendProductConfigurator.MediaProducers;
using BackendProductConfigurator.Validation;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enumerators;
using Model.Interfaces;
using Model.Wrapper;
using System.Net;

namespace BackendProductConfigurator.Controllers
{
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

    
    
    [Route("account")]
    public class AccountController : AController<Account, int>
    {
        public AccountController() : base()
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
    [Route("savedConfigs")]
    public partial class SavedConfigsController : AController<ProductSaveExtended, string>
    {
        public SavedConfigsController() : base()
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
            return entities[GetAccLang(Request)].Where(x => x.User.IsSameUser(account)).ToList();
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

    public partial class ConfiguredProductsController : AController<ConfiguredProduct, string>
    {
        [Route("/redactedConfiguredProducts")]
        [HttpPost]
        [NonAction]
        public override void Post([FromBody] ConfiguredProduct value) { }
    }
    public partial class ProductsController : AController<ConfiguratorSlim, string>
    {
        [Route("/redactedProducts")]
        [HttpPost]
        [NonAction]
        public override void Post([FromBody] ConfiguratorSlim value) { }
    }
    public partial class ConfigurationController : AController<Configurator, string>
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
    public partial class SavedConfigsController : AController<ProductSaveExtended, string>
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