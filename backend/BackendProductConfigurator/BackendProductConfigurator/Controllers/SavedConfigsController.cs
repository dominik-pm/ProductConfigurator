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
                    return entities[GetAccLang(Request)];

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
                return entities[GetAccLang(Request)].Where(x => x.User.IsSameUser(account)).Cast<ProductSave>().ToList();
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
                Configurator configurator = ValuesClass.Configurators[GetAccLang(Request)].Where(con => con.ConfigId == configId).First();
                description = configurator.Description;
                name = configurator.Name;
                ProductSaveExtended temp = new ProductSaveExtended() { ConfigId = configId, Date = DateTime.Now, Description = description, Name = name, Options = value.Options, SavedName = value.SavedName, Status = EStatus.saved.ToString(), User = ValuesClass.FillAccountFromToken(Request.Headers["Authorization"]) };
                entities[GetAccLang(Request)].Add(temp);
                ValuesClass.PostValue(temp, GetAccLang(Request));
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
                entities[GetAccLang(Request)].Add(value);
                ValuesClass.PostValue<ProductSaveExtended>(value, GetAccLang(Request));
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

                entities[GetAccLang(Request)].Remove(entities[GetAccLang(Request)].Where(entity => entity.ConfigId == id && entity.SavedName == requestBody.SavedName).First());
                ValuesClass.DeleteValue<SavedConfigWrapper>(GetAccLang(Request), new SavedConfigDeleteWrapper() { ConfigId = id, SavedName = requestBody.SavedName, UserEmail = account.UserEmail });
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
