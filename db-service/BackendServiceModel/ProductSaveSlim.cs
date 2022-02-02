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
        public List<string> Options { get; set; } = new();
    }
    public enum EStatus
    {
        ordered = 0, saved
    }
}
