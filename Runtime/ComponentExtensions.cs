using System.Linq;
using UnityEngine;

namespace Sim.Utility
{
    public static class ComponentExtensions
    {
        public static string GetPath(this Component component)
        {
            return string.Join("/",
                component.GetComponentsInParent<Transform>(true)
                    .Select(t => t.name)
                    .Reverse());
        }
    }
}
