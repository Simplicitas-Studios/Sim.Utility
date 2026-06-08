

using System;

namespace Sim.Dispositio.Shared
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IdUsageAttribute : Attribute
    {
        public IdUsage Usage { get; }

        public IdUsageAttribute(IdUsage usage)
        {
            Usage = usage;
        }
    }
}
