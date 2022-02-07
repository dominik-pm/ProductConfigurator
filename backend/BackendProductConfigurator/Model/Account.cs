using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Account
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public bool IsSameUser(Account temp)
        {
            if(this.UserName == temp.UserName)
                if(this.UserEmail == temp.UserEmail)
                    return true;
            return false;
        }
    }
}
