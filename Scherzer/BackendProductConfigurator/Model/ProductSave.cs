using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ProductSave : ProductSaveSlim, INameable, IIndexable<int>, IDescribable
    {
        public ProductSave(ProductSaveSlim pslim)
        {
            base.SavedName = pslim.SavedName;
            base.Options = pslim.Options;
            this.Name = "";
            this.Description = "";
            this.Status = EStatus.Ordered.ToString();
        }
        public ProductSave() { }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}
