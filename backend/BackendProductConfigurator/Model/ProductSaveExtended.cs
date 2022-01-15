using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ProductSaveExtended : ProductSave
    {
        public Account User { get; set; }
    }
}
