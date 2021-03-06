using BackendProductConfigurator.App_Code;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Interfaces;

namespace BackendProductConfigurator.Controllers
{
    [ApiController]
    public abstract class AController<T, K> : ControllerBase where T : class
    {
        public Dictionary<string, List<T>> entities;

        public AController()
        {
            if(DateTime.Now.Subtract(ValuesClass.LastDBFetch).TotalMinutes >= GlobalValues.MinutesBetweenFetches)
            {
                ValuesClass.SetValues();
                ValuesClass.LastDBFetch = DateTime.Now;
            }
        }

        // GET: api/<Controller>
        [HttpGet]
        public virtual ActionResult<IEnumerable<T>> Get()
        {
            Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
            return entities[GetAccLang(Request)];
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public virtual ActionResult<T> Get(K id)
        {
            try
            {
                Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
                return entities[GetAccLang(Request)].Where(entity => (entity as IIndexable).Id.Equals(id)).First();
            }
            catch(Exception ex)
            {
                return NotFound();
            }
        }

        // POST api/<Controller>
        [HttpPost]
        public virtual ActionResult Post([FromBody] T value)
        {
            try
            {
                Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
                entities[GetAccLang(Request)].Add(value);
                ValuesClass.PostValue<T>(value, GetAccLang(Request));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT api/<Controller>/5
        [HttpPut("{id}")]
        public virtual ActionResult Put(K id, [FromBody] T value)
        {
            Delete(id);
            return Post(value);
        }

        // DELETE api/<Controller>/5
        [HttpDelete("{id}")]
        public virtual ActionResult Delete(K id)
        {
            try
            {
                Response.Headers.AcceptLanguage = Request.Headers.AcceptLanguage;
                entities[GetAccLang(Request)].Remove(entities[GetAccLang(Request)].Where(entity => (entity as IIndexable).Id.Equals(id)).First());
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        public static string GetAccLang(HttpRequest request)
        {
            try
            {
                if(request.Headers.AcceptLanguage.ToString().Length > 0)
                {
                    if (request.Headers.AcceptLanguage.ToString().Contains('-'))
                        return request.Headers.AcceptLanguage.ToString().Split(",")[0].Trim('{').Split('-')[0];
                    else
                        return request.Headers.AcceptLanguage.ToString();
                }
                else
                    return GlobalValues.Languages.First();
            }
            catch
            {
                return GlobalValues.Languages.First();
            }
        }
    }

    #region RedactedAPIs

    //Um APIs für eine andere Methoden frei zu machen führen diese Methoden ins nichts

    public partial class ConfiguredProductsController : AController<ConfiguredProduct, string>
    {
        [Route("/redactedConfiguredProducts")]
        [HttpPost]
        [NonAction]
        public override ActionResult Post([FromBody] ConfiguredProduct value) { return NoContent(); }
    }
    public partial class ConfigurationController : AController<Configurator, string>
    {
        [Route("/redactedConfigurator")]
        [HttpPost]
        [NonAction]
        public override ActionResult Post([FromBody] Configurator value) { return NoContent(); }

        [Route("/redactedConfigurator")]
        [HttpPut]
        [NonAction]
        public override ActionResult Put(string configId, [FromBody] Configurator value) { return NoContent(); }
    }
    public partial class SavedConfigsController : AController<ProductSaveExtended, string>
    {
        [Route("/redactedSavedConfigsController")]
        [HttpDelete]
        [NonAction]
        public override ActionResult Delete(string id) { return NoContent(); }
    }

    #endregion
}