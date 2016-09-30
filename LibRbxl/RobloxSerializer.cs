﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LibRbxl.Instances;

namespace LibRbxl
{
    public class RobloxSerializer
    {
        private readonly RobloxDocument _document;
        private readonly Dictionary<Tuple<Type, string>, Property> _defaultPropertyCache = new Dictionary<Tuple<Type, string>, Property>(); 

        public RobloxSerializer(RobloxDocument document)
        {
            _document = document;
        }

        public PropertyCollection GetProperties(Instance instance)
        {
            var properties = new PropertyCollection();
            foreach (var prop in instance.GetType().GetProperties())
            {
                if (CheckNoSerialize(prop))
                    continue;
                var propertyType = GetPropertyType(prop);
                var propertyName = GetPropertyName(prop);
                var propertyValue = GetPropertyValue(instance, prop, propertyType);
                var robloxProperty = GetRobloxProperty(propertyValue, propertyName, propertyType);
                properties.Add(robloxProperty);
            }
            return properties;
        }

        public Property GetPropertyDefault(string propertyName, Type parentType, Property existing)
        {
            var cacheKey = new Tuple<Type, string>(parentType, propertyName);
            if (_defaultPropertyCache.ContainsKey(cacheKey))
            {
                return _defaultPropertyCache[cacheKey];
            }

            foreach (var propertyInfo in parentType.GetProperties())
            {
                if (propertyInfo.Name == propertyName)
                {
                    var attrs = propertyInfo.GetCustomAttributes<RobloxPropertyAttribute>();
                    if (attrs.Any())
                    {
                        
                    }
                }
            }
        }

        private object GetPropertyValue(Instance instance, PropertyInfo propertyInfo, PropertyType propertyType)
        {
            if (propertyType != PropertyType.Referent)
                return propertyInfo.GetValue(instance);
            else
                return _document.ReferentProvider.GetReferent(instance);
        }
        
        public void SetProperties(RobloxDocument document, Instance instance, PropertyCollection propertyCollection)
        {
            foreach (var property in propertyCollection)
            {
                var clrProperty = GetClrPropertyForRobloxProperty(instance, property);
                if (clrProperty != null)
                {
                    if (property.Type != PropertyType.Referent)
                        clrProperty.SetValue(instance, property.Get());
                    else
                    {
                        var referent = (int) property.Get();
                        if (referent != 0)
                            // It seems like in many cases 0 means no referent? For example, gui object's next selection property. TODO look into this.
                        {
                            var inst = (referent != -1) ? document.ReferentProvider.GetCached(referent) : null;
                            clrProperty.SetValue(instance, inst);
                        }
                    }
                }
                else
                {
                    instance.UnmanagedProperties.Add(property.Name, property);
                    Trace.WriteLine($"Found unmanaged property \"{property.Name}\" of type {property.Type} on object of class {instance.ClassName}.");
                }
            }
        }

        /// <summary>
        /// Looks for a matching CLR property on the given object. Properties with a matching RobloxPropertyAttribute take precedence. If no attribute match is found, a match by name is attempted. If no match is found, it returns null.
        /// </summary>
        /// <returns>
        /// Either the matching property or null.
        /// </returns>
        private PropertyInfo GetClrPropertyForRobloxProperty(RobloxObject robloxObject, Property property)
        {
            PropertyInfo match;

            if (robloxObject.PropertyCache.Get(property.Name, out match))
                return match;
                
            return null;
        }

        private bool CheckRobloxPropertyAttributeMatchesProperty(RobloxPropertyAttribute robloxPropertyAttribute, Property property)
        {
            return robloxPropertyAttribute.Type == property.Type;
        }

        private string GetPropertyName(PropertyInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes<RobloxPropertyAttribute>().ToArray();
            if (attrs.Any() && attrs.First().PropertyName != null)
                return attrs.First().PropertyName;
            return propertyInfo.Name;
        }

        private Property GetRobloxProperty(object value, string name, PropertyType type)
        {
            switch (type)
            {
                case PropertyType.String:
                    return new StringProperty(name, (string)value);
                case PropertyType.Boolean:
                    return new BoolProperty(name, (bool)value);
                case PropertyType.Int32:
                    return new Int32Property(name, (int)value);
                case PropertyType.Float:
                    return new FloatProperty(name, (float)value);
                case PropertyType.Double:
                    return new DoubleProperty(name, (double)value);
                case PropertyType.UDim2:
                    return new UDim2Property(name, (UDim2)value);
                case PropertyType.Ray:
                    return new RayProperty(name, (Ray)value);
                case PropertyType.Faces:
                    return new FacesProperty(name, (Faces)value);
                case PropertyType.Axis:
                    return new AxisProperty(name, (Axis)value);
                case PropertyType.BrickColor:
                    return new BrickColorProperty(name, (BrickColor)value);
                case PropertyType.Color3:
                    return new Color3Property(name, (Color3)value);
                case PropertyType.Vector2:
                    return new Vector2Property(name, (Vector2)value);
                case PropertyType.Vector3:
                    return new Vector3Property(name, (Vector3)value);
                case PropertyType.CFrame:
                    return new CFrameProperty(name, (CFrame)value);
                case PropertyType.Enumeration:
                    return new EnumerationProperty(name, (int)value);
                case PropertyType.Referent:
                    return new ReferentProperty(name, (int)value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private PropertyType GetPropertyType(PropertyInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes<RobloxPropertyAttribute>().ToArray();
            if (attrs.Any() && attrs.First().Type.HasValue)
                return attrs.First().Type.Value;
            if (propertyInfo.PropertyType == typeof (Axis))
                return PropertyType.Axis;
            if (propertyInfo.PropertyType == typeof (bool))
                return PropertyType.Boolean;
            if (propertyInfo.PropertyType == typeof (BrickColor))
                return PropertyType.BrickColor;
            if (propertyInfo.PropertyType == typeof (CFrame))
                return PropertyType.CFrame;
            if (propertyInfo.PropertyType == typeof (Color3))
                return PropertyType.Color3;
            if (propertyInfo.PropertyType == typeof (double))
                return PropertyType.Double;
            if (propertyInfo.PropertyType == typeof (Faces))
                return PropertyType.Faces;
            if (propertyInfo.PropertyType == typeof (float))
                return PropertyType.Float;
            if (propertyInfo.PropertyType == typeof (Ray))
                return PropertyType.Ray;
            if (propertyInfo.PropertyType == typeof (string))
                return PropertyType.String;
            if (propertyInfo.PropertyType == typeof (UDim2))
                return PropertyType.UDim2;
            if (propertyInfo.PropertyType == typeof (Vector2))
                return PropertyType.Vector2;
            if (propertyInfo.PropertyType == typeof (Vector3))
                return PropertyType.Vector3;
            if (propertyInfo.PropertyType.IsEnum)
                return PropertyType.Enumeration;
            if (propertyInfo.PropertyType.IsAssignableFrom(typeof(Instance)))
                return PropertyType.Referent;
            throw new ArgumentException("Property parentType does not match any Roblox property parentType.", nameof(propertyInfo));
        }

        private bool CheckNoSerialize(PropertyInfo propertyInfo)
        {
            return Attribute.IsDefined(propertyInfo, typeof (RobloxIgnoreAttribute));
        }
    }
}
