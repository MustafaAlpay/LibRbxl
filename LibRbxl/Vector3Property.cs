﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibRbxl
{
    public class Vector3Property : Property<Vector3>
    {
        public Vector3Property(string name, Vector3 value) : base(name, PropertyType.Vector3, value)
        {
        }
    }
}
