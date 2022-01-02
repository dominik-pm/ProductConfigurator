﻿namespace Model
{
    public class ProductDependencies
    {
        public ProductDependencies(int basePrice, List<string> defaultOptions, Dictionary<string, List<string>> replacementGroups, Dictionary<string, List<string>> requirements, Dictionary<string, List<string>> incompabilities, Dictionary<string, float> priceList)
        {
            BasePrice = basePrice;
            DefaultOptions = defaultOptions;
            ReplacementGroups = replacementGroups;
            Requirements = requirements;
            Incompabilities = incompabilities;
            PriceList = priceList;
        }
        public int BasePrice { get; set; }
        public List<string> DefaultOptions { get; set; }
        public Dictionary<string, List<string>> ReplacementGroups { get; set; }
        public Dictionary<string, List<string>> Requirements { get; set; }
        public Dictionary<string, List<string>> Incompabilities { get; set; }
        public Dictionary<string, float> PriceList { get; set; }
    }
}