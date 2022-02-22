using Microsoft.AspNetCore.Mvc;
using Model;

namespace BackendProductConfigurator.Controllers
{
    [Route("account")]
    public class AccountController : AController<Account, int>
    {
        public AccountController() : base()
        {
            entities = ValuesClass.Accounts;
        }
    }
}
