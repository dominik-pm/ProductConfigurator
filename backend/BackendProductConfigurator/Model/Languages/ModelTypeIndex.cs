﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Languages
{
    public class ModelTypeIndex
    {
        public string Id { get; set; }
        public List<string> Options { get; set; } = new List<string>();
    }
}
