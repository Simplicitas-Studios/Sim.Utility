namespace Sim.Utility.Editor.Editor
{
    internal static class StaticPackageInfo
    {
        public static PackagePathInfo SimUtility { get; } = new(
            @"Packages\com.sim.utility\",
            "Plugins/Sim.Utility/"
        );
    }
}
