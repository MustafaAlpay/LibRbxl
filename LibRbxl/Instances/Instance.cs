﻿using System.Collections.Generic;

namespace LibRbxl.Instances
{
    public abstract class Instance
    {
        private Instance _parent;
        public bool Archivable { get; set; }

        [RobloxIgnore]
        public abstract string ClassName { get; }

        public string Name { get; set; }

        [RobloxIgnore]
        public Instance Parent
        {
            get { return _parent; }
            set
            {
                if (_parent == value)
                    return;
                var oldParent = _parent;
                _parent = value;
                oldParent?.Children.Remove(this);
                if (value != null && !value.Children.Contains(this))
                    _parent.Children.Add(this);
            }
        }

        [RobloxIgnore]
        public ChildCollection Children { get; }

        [RobloxIgnore]
        public int Referent { get; set; }
            
        [RobloxIgnore]
        public Dictionary<string, Property> UnmanagedProperties { get; } 

        protected Instance()
        {
            Children = new ChildCollection(this);
            UnmanagedProperties = new Dictionary<string, Property>();
        }
    }
}

