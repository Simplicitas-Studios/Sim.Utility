namespace Sim.Utility.PropertyModel
{
    internal sealed class EmptyPropertySet : IPropertySet
    {
        public IProperty<T> Get<T>(IPropertyKey<T> key)
        {
            return Property.Empty(key);
        }

        public IPropertySet Add<T>(IPropertyKey<T> key, T value)
        {
            return new PropertySet()
                .Add(key, value);
        }

        public IPropertySet Update<T>(IPropertyKey<T> key, T value)
        {
            return new PropertySet()
                .Add(key, value);
        }

        public static EmptyPropertySet Instance { get; } = new EmptyPropertySet();
    }
}
