namespace Sim.Utility.PropertyModel
{
    public interface IReadOnlyPropertySet
    {
        public IProperty<T> Get<T>(IPropertyKey<T> key);
    }
}
