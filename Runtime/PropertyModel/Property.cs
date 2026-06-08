namespace Sim.Utility.PropertyModel
{
    public static class Property
    {
        public static IProperty<T> From<T>(IPropertyKey<T> key, T value)
        {
            return new PropertyImpl<T>(key, value);
        }

        public static IProperty<T> Empty<T>(IPropertyKey<T> key)
        {
            return new PropertyImpl<T>(key);
        }
    }
}
