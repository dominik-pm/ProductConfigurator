using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ProductSlim : IIndexable<int>, INameable, IDescribable
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "Abstract Product";
        public string Description { get; set; } = "Abstract Description";
    }
}
