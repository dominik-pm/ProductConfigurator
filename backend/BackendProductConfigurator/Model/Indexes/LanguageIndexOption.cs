using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Indexes
{
    public class LanguageIndexOption : IIndexable
    {
        public string Id { get; set; }

        public List<string> Options { get; set; } = new List<string>();
    }
}
