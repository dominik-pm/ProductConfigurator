﻿using System.Text.Json.Serialization;

namespace Model
{
    public class ProductDependencies
    {
        public float BasePrice { get; set; }
        public List<string> DefaultOptions { get; set; } = new List<string>();
        public Dictionary<string, List<string>> ReplacementGroups { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Requirements { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Incompabilities { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> GroupRequirements { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, float> PriceList { get; set; } = new Dictionary<string, float>();
    }
}