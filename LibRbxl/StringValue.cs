﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibRbxl
{
    public class StringValue : Instance
    {
        public override string ClassName => "StringValue";

        public string Value { get; set; }
    }
}