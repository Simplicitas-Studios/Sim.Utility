namespace Sim.Utility.Editor
{
    public readonly struct PackagePathInfo
    {
        public string PackagePath { get; }

        public string PluginPath { get; }

        public PackagePathInfo(string packagePath, string pluginPath)
        {
            PackagePath = packagePath;
            PluginPath = pluginPath;
        }
    }
}
