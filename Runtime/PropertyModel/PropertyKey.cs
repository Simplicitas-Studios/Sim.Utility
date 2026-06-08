namespace Sim.Utility.PropertyModel
{
    public static class PropertyKey
    {
        public static IPropertyKey<T> From<T>(string name)
        {
            return new PropertyKeyImpl<T>(name);
        }
    }
}
