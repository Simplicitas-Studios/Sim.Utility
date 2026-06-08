namespace Sim.Utility.PropertyModel
{
    public sealed class PropertyImpl<T> : IProperty<T>
    {
        public IPropertyKey<T> Key { get; }

        public T Value { get; }

        public bool HasValue { get; }

        public PropertyImpl(IPropertyKey<T> key, T value)
        {
            Key = key;
            Value = value;
            HasValue = true;
        }

        public PropertyImpl(IPropertyKey<T> key)
        {
            Key = key;
            HasValue = false;
        }
    }
}
