namespace Sim.Utility.PropertyModel
{
    public interface IPropertySet : IReadOnlyPropertySet
    {
        public IPropertySet Add<T>(IPropertyKey<T> key, T value);

        public IPropertySet Update<T>(IPropertyKey<T> key, T value);
    }
}
