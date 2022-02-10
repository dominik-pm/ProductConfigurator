using BackendProductConfigurator.MediaProducers;
using BackendProductConfigurator.Validation;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enumerators;
using Model.Interfaces;
using Model.Wrapper;

namespace BackendProductConfigurator.Controllers
{
    [Route("configuration")]
    public partial class ConfigurationController : AController<Configurator, string>
    {
        public ConfigurationController() : base()
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
            Dictionary<string, Configurator> configurators = AValuesClass.GenerateConfigurator(value);

            //EValidationResult validationResult = ValidationMethods.ValidateConfigurator(configurator);
            //if (validationResult == EValidationResult.ValidationPassed)
            //{
            //    AddConfigurator(configurator);
            //    AValuesClass.PostValue<Configurator>(configurator, GetAccLang(Request));
            //}
        }
        public override void Delete(string id)
        {
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            entities[GetAccLang(Request)].Remove(entities[GetAccLang(Request)].Find(entity => (entity as IConfigId).ConfigId.Equals(id)));
            AValuesClass.ConfiguratorsSlim[GetAccLang(Request)].Remove(AValuesClass.ConfiguratorsSlim[GetAccLang(Request)].Find(entity => (entity as IConfigId).ConfigId.Equals(id)));
            AValuesClass.DeleteValue<ConfigurationDeleteWrapper>(GetAccLang(Request), new ConfigurationDeleteWrapper() { ConfigId = id });
        }
    }
}
