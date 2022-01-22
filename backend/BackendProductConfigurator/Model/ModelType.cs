using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ModelType : INameable, IDescribable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Options { get; set; } = new List<string>();

    }
}
