using System.IO;
using System.Text.RegularExpressions;

namespace Sim.Utility.CodeGenerator
{
    internal static class NamespaceHelper
    {
        internal static bool TryExtractNamespaceFromNearbyFiles(string targetDirectory, out string namespaceName)
        {
            var directoryInfo = new DirectoryInfo(targetDirectory);

            while (directoryInfo != null)
            {
                var csFiles = directoryInfo.GetFiles("*.cs", SearchOption.TopDirectoryOnly);

                foreach (var file in csFiles)
                {
                    if (TryExtractNameSpace(file.FullName, out namespaceName))
                    {
                        return true;
                    }
                }

                directoryInfo = directoryInfo.Parent;
            }

            namespaceName = string.Empty;
            return false;
        }

        private static bool TryExtractNameSpace(string file, out string namespaceName)
        {
            string content = File.ReadAllText(file);

            var match = Regex.Match(content, @"namespace\s+([A-Za-z0-9_.]+)");

            if (match.Success)
            {
                namespaceName = match.Groups[1].Value;
                return true;
            }

            namespaceName = string.Empty;
            return false;
        }
    }
}
