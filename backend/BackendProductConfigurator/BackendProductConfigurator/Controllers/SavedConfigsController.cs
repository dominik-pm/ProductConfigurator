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
            entities = AValuesClass.SavedProducts;
        }

        // GET: /account/configuration
        [Route("/account/allorderedconfigurations")]
        [HttpGet]
        public override List<ProductSaveExtended> Get()
        {
            Account account = AValuesClass.FillAccountFromToken(Request.Headers["Authorization"]);

            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            if (account.IsAdmin)
                return entities[GetAccLang(Request)];

            return new List<ProductSaveExtended>();
        }

        // GET: /account/configuration
        [Route("/account/configurations")]
        [HttpGet]
        public List<ProductSave> GetSavedConfigs()
        {
            Account account = AValuesClass.FillAccountFromToken(Request.Headers["Authorization"]);

            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            return entities[GetAccLang(Request)].Where(x => x.User.IsSameUser(account)).Cast<ProductSave>().ToList();
        }

        // POST: /account/configuration
        [Route("/account/configurations/{configId}")]
        [HttpPost]
        public void Post([FromBody] ProductSaveSlim value, string configId)
        {
            string description, name;
            Configurator configurator = AValuesClass.Configurators[GetAccLang(Request)].Where(con => con.ConfigId == configId).First();
            description = configurator.Description;
            name = configurator.Name;
            ProductSaveExtended temp = new ProductSaveExtended() { ConfigId = configId, Date = DateTime.Now, Description = description, Name = name, Options = value.Options, SavedName = value.SavedName, Status = EStatus.saved.ToString(), User = AValuesClass.FillAccountFromToken(Request.Headers["Authorization"]) };
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
            Account account = AValuesClass.FillAccountFromToken(Request.Headers["Authorization"]);

            entities[GetAccLang(Request)].Remove(entities[GetAccLang(Request)].Where(entity => entity.ConfigId == id && entity.SavedName == requestBody.SavedName).First());
            AValuesClass.DeleteValue<SavedConfigWrapper>(GetAccLang(Request), new SavedConfigDeleteWrapper() { ConfigId = id, SavedName = requestBody.SavedName, UserEmail = account.UserEmail});
        }
    }
}
