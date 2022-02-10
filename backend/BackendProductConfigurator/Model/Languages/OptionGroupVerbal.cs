﻿using Model.Indexes;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Languages
{
    public class OptionGroupVerbal : NamedIndex, IDescribable
    {
        public string Description { get; set; }
    }
}
