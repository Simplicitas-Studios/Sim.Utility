namespace Sim.Utility.PropertyModel
{
    public sealed class PropertyKeyImpl<T> : IPropertyKey<T>
    {
        public string Name { get; }

        internal PropertyKeyImpl(string name)
        {
            Name = name;
        }
    }
}
