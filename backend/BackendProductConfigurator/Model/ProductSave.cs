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
        public string ConfigId { get; set; } //Produktnummer
        public string Name { get; set; } //Produkt
        public string Description { get; set; } //Produkt
        public EStatus Status { get; set; }
        public DateTime Date { get; set; }
    }
}
