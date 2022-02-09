using Microsoft.AspNetCore.Mvc;
using Model;

namespace BackendProductConfigurator.Controllers
{
    [Route("account")]
    public class AccountController : AController<Account, int>
    {
        public AccountController() : base()
        {
            entities = AValuesClass.Accounts;
        }

        // POST /account
        [HttpPost]
        public override void Post([FromBody] Account value)
        {
            entities[GetAccLang(Request)].Add(value);
            AValuesClass.PostValue<Account>(value, GetAccLang(Request));
        }
    }
}
