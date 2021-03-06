using BackendProductConfigurator.App_Code;
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
            return Post(value, "");
        }

        [NonAction]
        public ActionResult Post([FromBody] ConfiguratorPost value, string oldConfigId)
        {
            try
            {
                Dictionary<string, Configurator> configurators = ValuesClass.GenerateConfigurator(value);

                foreach(KeyValuePair<string, Configurator> configDict in configurators)
                {
                    EValidationResult validationResult = ValidationMethods.ValidateConfigurator(configDict.Value);
                    if (validationResult == EValidationResult.ConfiguratorInvalid)
                    {
                        return ValidationProblem("One or more errors found in the posted configurator");
                    }
                }

                Task<ActionResult> task = Task<ActionResult>.Factory.StartNew(new Func<object?, ActionResult>((obj) =>
                {
                    try
                    {
                        List<Task> tasks = new List<Task>();
                        Dictionary<string, Configurator> configuratorsList = obj as Dictionary<string, Configurator>;
                        Configurator configurator = ValuesClass.AdaptConfiguratorsOptionIds(configuratorsList.Values.First(), oldConfigId);

                        foreach(KeyValuePair<string, Configurator> configDict in configuratorsList)
                        {
                            AddConfigurator(configDict.Value, configDict.Key);
                        }

                        ValuesClass.PostValue<Configurator>(configurator, configuratorsList.Keys.First());
                        configuratorsList.Remove(configuratorsList.Keys.First());

                        foreach (KeyValuePair<string, Configurator> configDict in configuratorsList)
                        {
                            tasks.Add(Task<ActionResult>.Factory.StartNew(new Func<object?, ActionResult>((confObj) =>
                            {
                                try
                                {
                                    KeyValuePair<string, Configurator> objPair = (KeyValuePair<string, Configurator>)confObj;
                                    Configurator temp = ValuesClass.AdaptConfiguratorsOptionIds(objPair.Value, oldConfigId);
                                    ValuesClass.PutValue<Configurator>(temp, configDict.Key);
                                    return Ok();
                                }
                                catch (Exception ex)
                                {
                                    return BadRequest(ex.Message);
                                }
                            }), configDict));
                        }
                        Task.WhenAll(tasks);
                        foreach(Task<ActionResult> task in tasks)
                        {
                            if (task.Result != Ok())
                                throw new Exception("Error while uploading");
                        }
                        return Ok();
                    }
                    catch (Exception ex)
                    { 
                        return BadRequest(ex.Message);
                    }
                }), configurators);
                if(task.Wait(GlobalValues.TimeOut))
                {
                    return task.Result;
                }
                else
                    return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{configId}")]
        public ActionResult Put(string configId, [FromBody] ConfiguratorPost value)
        {
            Delete(configId);
            return Post(value, configId);
        }
        [HttpDelete("{id}")]
        public override ActionResult Delete(string id)
        {
            try
            {
                Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
                foreach(string language in entities.Keys)
                {
                    entities[language].Remove(entities[language].Where(entity => (entity as IConfigId).ConfigId.Equals(id)).First());
                }
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
