﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibRbxl
{
    public class ModuleScript : LuaSourceContainer
    {
        public override string ClassName => "ModuleScript";

        public string LinkedSource { get; set; }
        public string ScriptGuid { get; set; } // TODO Convert to actual guid
        public string Source { get; set; }
    }
}