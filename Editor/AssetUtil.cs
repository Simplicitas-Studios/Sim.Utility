using UnityEditor;

namespace Sim.Utility.Editor
{
    public static class AssetUtil
    {

        public static T Load<T>(string assetPath, PackagePathInfo packagePathInfo) where T : UnityEngine.Object
        {
            string path = PackageManagerUtils.IsInstalledAsPackage(packagePathInfo)
                ? packagePathInfo.PackagePath
                : packagePathInfo.PluginPath;

            return AssetDatabase.LoadAssetAtPath<T>(path + assetPath);
        }

        public static string GetLocationAwarePackagePath(string path, PackagePathInfo packagePathInfo)
        {
            string startingPath = PackageManagerUtils.IsInstalledAsPackage(packagePathInfo)
                ? packagePathInfo.PackagePath
                : packagePathInfo.PluginPath;

            return startingPath + path;
        }
    }
}
