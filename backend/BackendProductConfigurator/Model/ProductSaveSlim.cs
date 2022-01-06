using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public  class ProductSaveSlim
    {
        public string SavedName { get; set; }
        public List<Option> Options { get; set; } = new List<Option>();
    }
    public enum EStatus
    {
        Ordered = 0, Saved
    }
}
