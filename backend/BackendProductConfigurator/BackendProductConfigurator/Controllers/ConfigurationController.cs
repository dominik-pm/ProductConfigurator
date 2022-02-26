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
            entities = ValuesClass.Configurators;
        }

        private void AddConfigurator(Configurator value)
        {
            AddConfigurator(value, GetAccLang(Request));
        }
        private void AddConfigurator(Configurator value, string language)
        {
            entities[language].Add(value);
        }

        [HttpGet("{id}")]
        public override ActionResult<Configurator> Get(string id)
        {
            try
            {
                Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
                return entities[GetAccLang(Request)].Where(entity => entity.ConfigId.Equals(id)).First();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [Route("/products")]
        [HttpGet]
        public List<ConfiguratorSlim> GetConfiguratorSlims()
        {
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            return entities[GetAccLang(Request)].Cast<ConfiguratorSlim>().ToList();
        }

        [HttpPost]
        public ActionResult Post([FromBody] ConfiguratorPost value)
        {
            try
            {
                Dictionary<string, Configurator> configurators = ValuesClass.GenerateConfigurator(value);
                Configurator configurator;

                configurator = ValuesClass.AdaptConfiguratorsOptionIds(configurators.Values.First());

                foreach(KeyValuePair<string, Configurator> configDict in configurators)
                {
                    EValidationResult validationResult = ValidationMethods.ValidateConfigurator(configDict.Value);
                    if (validationResult == EValidationResult.ConfiguratorInvalid)
                    {
                        return ValidationProblem("One or more errors found in the posted configurator");
                    }
                }

                AddConfigurator(configurator, configurators.Keys.First());
                ValuesClass.PostValue<Configurator>(configurator, configurators.Keys.First());
                configurators.Remove(configurators.Keys.First());

                foreach (KeyValuePair<string, Configurator> configDict in configurators)
                {
                    Configurator temp = ValuesClass.AdaptConfiguratorsOptionIds(configDict.Value);
                    AddConfigurator(temp, configDict.Key);
                    ValuesClass.PutValue<Configurator>(temp, configDict.Key);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public virtual ActionResult Put(string configId, [FromBody] ConfiguratorPost value)
        {
            Delete(configId);
            return Post(value);
        }
        [HttpDelete("{id}")]
        public override ActionResult Delete(string id)
        {
            try
            {
                Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
                entities[GetAccLang(Request)].Remove(entities[GetAccLang(Request)].Where(entity => (entity as IConfigId).ConfigId.Equals(id)).First());
                ValuesClass.DeleteValue<ConfigurationDeleteWrapper>(GetAccLang(Request), new ConfigurationDeleteWrapper() { ConfigId = id });
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
