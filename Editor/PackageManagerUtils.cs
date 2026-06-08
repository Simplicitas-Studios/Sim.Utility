

using UnityEditor;

namespace Sim.Utility.Editor
{
    public static class PackageManagerUtils
    {
        private const string s_packagePath = "Packages/com.sim.dispositio";

        private static bool? s_isInstalled;

        public static bool IsInstalledAsPackage(PackagePathInfo packagePathInfo)
        {
            if (s_isInstalled.HasValue)
            {
                return s_isInstalled.Value;
            }

            s_isInstalled = AssetDatabase.IsValidFolder(packagePathInfo.PackagePath);

            return s_isInstalled.Value;
        }
    }
}
