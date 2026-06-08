namespace Sim.Utility.PropertyModel {
    public interface IProperty<T> : IProperty
    {
        IPropertyKey<T> Key { get; }

        T Value { get; }

        bool HasValue { get; }
    }

    public interface IProperty { }
}
