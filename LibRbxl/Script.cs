﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibRbxl
{
    public class Script : BaseScript
    {
        public override string ClassName => "Script";
        
        public string Source { get; set; }
    }
}
