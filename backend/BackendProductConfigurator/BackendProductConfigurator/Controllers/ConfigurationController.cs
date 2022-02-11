﻿using BackendProductConfigurator.MediaProducers;
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
            AddConfigurator(value, GetAccLang(Request));
        }
        private void AddConfigurator(Configurator value, string language)
        {
            entities[language].Add(value);
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public override Configurator Get(string id)
        {
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            return entities[GetAccLang(Request)].Where(entity => entity.ConfigId.Equals(id)).First();
        }

        [Route("/products")]
        [HttpGet]
        public List<ConfiguratorSlim> GetConfiguratorSlims()
        {
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            return entities[GetAccLang(Request)].Cast<ConfiguratorSlim>().ToList();
        }

        // POST api/<Controller>
        [HttpPost]
        public void Post([FromBody] ConfiguratorPost value)
        {
            Dictionary<string, Configurator> configurators = AValuesClass.GenerateConfigurator(value);
            Configurator configurator;

            configurator = AValuesClass.AdaptConfiguratorsOptionIds(configurators.Values.First());
            AddConfigurator(configurator, configurators.Keys.First());
            AValuesClass.PostValue<Configurator>(configurator, configurators.Keys.First());
            configurators.Remove(configurators.Keys.First());

            foreach(KeyValuePair<string, Configurator> configDict in configurators)
            {
                configurator = AValuesClass.AdaptConfiguratorsOptionIds(configDict.Value);
                //EValidationResult validationResult = ValidationMethods.ValidateConfigurator(configDict.Value);
                //if (validationResult == EValidationResult.ValidationPassed)
                //{
                    AddConfigurator(configDict.Value, configDict.Key);
                    AValuesClass.PutValue<Configurator>(configDict.Value, configDict.Key);
                //}
            }
        }
        public override void Delete(string id)
        {
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            entities[GetAccLang(Request)].Remove(entities[GetAccLang(Request)].Where(entity => (entity as IConfigId).ConfigId.Equals(id)).First());
            AValuesClass.DeleteValue<ConfigurationDeleteWrapper>(GetAccLang(Request), new ConfigurationDeleteWrapper() { ConfigId = id });
        }

        [HttpGet]
        [Route("test")]
        public Configurator Test()
        {
            return AValuesClass.AdaptConfiguratorsOptionIds(Get("Golf"));
        }
    }
}
