using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ProductSave : ProductSaveSlim, INameable, IDescribable, IConfigId
    {
        public int ConfigId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
    }
}
