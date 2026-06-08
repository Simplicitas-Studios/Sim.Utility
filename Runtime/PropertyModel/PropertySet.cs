using System;
using System.Collections.Generic;

namespace Sim.Utility.PropertyModel
{
    public class PropertySet : IPropertySet
    {
        private readonly Dictionary<IPropertyKey, IProperty> _properties;

        public PropertySet()
        {
            _properties = new Dictionary<IPropertyKey, IProperty>();
        }

        public IProperty<T> Get<T>(IPropertyKey<T> key)
        {
            if (!_properties.TryGetValue(key, out var property))
            {
                property = Property.Empty(key);
            }
            return property as IProperty<T>;
        }

        public IPropertySet Add<T>(IPropertyKey<T> key, T value)
        {
            if (_properties.ContainsKey(key))
            {
                throw new ArgumentException($"The passed key '{key.Name}' is already present in the property set. Use update instead.", nameof(key));
            }

            _properties[key] = Property.From(key, value);
            return this;
        }

        public void Clear()
        {
            _properties.Clear();
        }

        public IPropertySet Update<T>(IPropertyKey<T> key, T value)
        {
            _properties[key] = Property.From(key, value);
            return this;
        }

        public static IPropertySet Empty => EmptyPropertySet.Instance;
    }
}
