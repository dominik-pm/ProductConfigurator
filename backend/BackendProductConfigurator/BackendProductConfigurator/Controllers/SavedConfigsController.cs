using BackendProductConfigurator.App_Code;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enumerators;
using Model.Wrapper;

namespace BackendProductConfigurator.Controllers
{
    [Route("savedConfigs")]
    public partial class SavedConfigsController : AController<ProductSaveExtended, string>
    {
        public SavedConfigsController() : base()
        {
            entities = ValuesClass.SavedProducts;
        }

        [Route("/account/allorderedconfigurations")]
        [HttpGet]
        public override ActionResult<IEnumerable<ProductSaveExtended>> Get()
        {
            try
            {
                Account account = ValuesClass.FillAccountFromToken(Request.Headers["Authorization"]);

                Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
                if (account.IsAdmin)
                    return entities["NaL"].Where(x => x.Status == "ordered").ToList();

                throw new Exception("User from JWT is not an admin");
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Route("/account/configurations")]
        [HttpGet]
        public ActionResult<List<ProductSave>> GetSavedConfigs()
        {
            try
            {
                Account account = ValuesClass.FillAccountFromToken(Request.Headers["Authorization"]);

                Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
                return entities["NaL"].Where(x => x.User.IsSameUser(account)).Cast<ProductSave>().ToList();
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Route("/account/configurations/{configId}")]
        [HttpPost]
        public ActionResult Post([FromBody] ProductSaveSlim value, string configId)
        {
            try
            {
                string description, name;
                Configurator configurator;
                try
                {
                    configurator = ValuesClass.Configurators["en"].Where(con => con.ConfigId == configId).First();
                }
                catch (Exception)
                {
                    configurator = ValuesClass.Configurators[GlobalValues.Languages.First()].Where(con => con.ConfigId == configId).First();
                }
                description = configurator.Description;
                name = configurator.Name;
                ProductSaveExtended temp = new ProductSaveExtended() { ConfigId = configId, Date = DateTime.Now, Description = description, Name = name, Options = value.Options, SavedName = value.SavedName, Status = EStatus.saved.ToString(), User = ValuesClass.FillAccountFromToken(Request.Headers["Authorization"]) };
                entities["NaL"].Add(temp);
                ValuesClass.PostValue(temp, "NaL");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [NonAction]
        public ActionResult PostOrdered(ProductSaveExtended value, HttpRequest Request)
        {
            try
            {
                entities["NaL"].Add(value);
                ValuesClass.PostValue<ProductSaveExtended>(value, "NaL");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("/account/configurations/{id}")]
        [HttpDelete]
        public ActionResult SavedConfigDelete([FromBody] SavedNameWrapper requestBody, string id)
        {
            try
            {
                Account account = ValuesClass.FillAccountFromToken(Request.Headers["Authorization"]);

                entities["NaL"].Remove(entities["NaL"].Where(entity => entity.ConfigId == id && entity.SavedName == requestBody.SavedName).First());
                ValuesClass.DeleteValue<SavedConfigWrapper>("NaL", new SavedConfigDeleteWrapper() { ConfigId = id, SavedName = requestBody.SavedName, UserEmail = account.UserEmail });
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
