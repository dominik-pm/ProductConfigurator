using BackendProductConfigurator.MediaProducers;
using BackendProductConfigurator.Validation;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enumerators;

namespace BackendProductConfigurator.Controllers
{
    [Route("configuredProducts")]
    public partial class ConfiguredProductsController : AController<ConfiguredProduct, string>
    {
        public ConfiguredProductsController() : base()
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

            SavedConfigsController scc = new SavedConfigsController();
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
}
