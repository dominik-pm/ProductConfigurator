using Model.Interfaces;
using Model.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ModelType : ModelTypeVerbal, IIndexable
    {
        public string Id { get; set; }
        public List<string> Options { get; set; } = new List<string>();

    }
}
