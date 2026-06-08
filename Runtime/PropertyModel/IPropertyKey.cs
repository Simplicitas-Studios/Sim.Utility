namespace Sim.Utility.PropertyModel {
    public interface IPropertyKey<T> : IPropertyKey
    {

    }

    public interface IPropertyKey
    {
        string Name { get; }
    }
}
