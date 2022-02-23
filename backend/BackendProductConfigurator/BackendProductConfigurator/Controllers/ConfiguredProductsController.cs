using BackendProductConfigurator.Exceptions;
using BackendProductConfigurator.MediaProducers;
using BackendProductConfigurator.Validation;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enumerators;
using Model.Wrapper;

namespace BackendProductConfigurator.Controllers
{
    [Route("configuredProducts")]
    public partial class ConfiguredProductsController : AController<ConfiguredProduct, string>
    {
        public ConfiguredProductsController() : base()
        {
            entities = ValuesClass.ConfiguredProducts;
        }

        [Route("/products/{configId}")]
        [HttpPost]
        public ActionResult Post([FromBody] ConfiguredProduct value, string configId)
        {
            try
            {
                EValidationResult validationResult;
                Configurator configurator = ValuesClass.Configurators[GetAccLang(Request)].Where(config => config.ConfigId == configId).First();
                validationResult = ValidationMethods.ValidateConfiguration(value, configurator);
                if (validationResult == EValidationResult.ValidationPassed)
                {
                    validationResult = ValidationMethods.ValidatePrice(value, configurator.Rules);
                }
                if (validationResult == EValidationResult.ValidationPassed)
                {
                    validationResult = ValidationMethods.ValidateSelectedModel(value, configurator);
                }
                new Thread(() =>
                {
                    try
                    {
                        EmailProducer.SendEmail(new EmailWrapper { Options = configurator.Options.Where(x => value.Options.Contains(x.Id)).ToList(), ConfiguredProduct = value }, validationResult, ValuesClass.FillAccountFromToken(Request.Headers["Authorization"]));
                    }
                    catch (Exception e) { throw e; }
                }).Start();
                if(validationResult != EValidationResult.ValidationPassed)
                {
                    throw new InvalidConfiguratorException($"Posted configurator failed the validation. Error: {validationResult.ToString()}");
                }
                new Thread(() =>
                {
                    try
                    {
                        validationResult = EValidationResult.ValidationPassed;
                        if (validationResult == EValidationResult.ValidationPassed)
                            PdfProducer.GeneratePDF(value, configId, Request);
                    }
                    catch (FileNotFoundException e)
                    {
                        //do something when image is not found
                    }
                }).Start();
                entities[GetAccLang(Request)].Add(value);

                Account account = ValuesClass.FillAccountFromToken(Request.Headers["Authorization"]);

                ProductSaveExtended temp = new ProductSaveExtended()
                {
                    Status = EStatus.ordered.ToString(),
                    Date = DateTime.Now,
                    ConfigId = configurator.ConfigId,
                    Name = configurator.Name,
                    Description = configurator.Description,
                    Options = value.Options,
                    SavedName = value.ConfigurationName,
                    User = account
                };
                return new SavedConfigsController().PostOrdered(temp, Request);
            }
            catch (InvalidConfiguratorException e)
            {
                return ValidationProblem(e.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
