using System.Text.RegularExpressions;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Sim.Dispositio.Shared
{
    public static class StringExtensions
    {
        public static string SubstrBeforeFirst(this string str, char c)
        {
            var idx = str.IndexOf(c);
            return idx != -1 ? str.Substring(0, idx) : str;
        }

        public static string SubstrAfterFirst(this string str, char c)
        {
            var idx = str.IndexOf(c);
            return idx != -1 ? str.Substring(idx + 1) : string.Empty;
        }

        public static string SubstrBeforeLast(this string str, char c)
        {
            var idx = str.LastIndexOf(c);

            return idx != -1 ? str.Substring(0, idx) : str;
        }

        public static string WithColor(this string str, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{str}</color>";
        }

        public static string WithColor(this string str, System.Drawing.Color color)
        {
            return $"<color={ToHtmlColorWithAlpha(color)}>{str}</color>";
        }

        private static string ToHtmlColorWithAlpha(System.Drawing.Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static string SanitizeForClassName(this string str)
        {
            return str.SanitizeForGenericIdentifier().ToPascalCase();
        }

        public static string SanitizeForVariableName(this string str)
        {
            return str.SanitizeForGenericIdentifier().ToCamelCase();
        }

        public static string SanitizeForProperty(this string str)
        {
            return str.SanitizeForGenericIdentifier().ToPascalCase();
        }

        private static string SanitizeForGenericIdentifier(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "Empty";
            }

            string sanitized = Regex.Replace(str, "[^a-zA-Z0-9_]", "");

            if (char.IsDigit(sanitized, 0))
            {
                sanitized = "_" + sanitized;
            }

            return sanitized;
        }

        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            str = str.Trim();

            if (str.Length == 1)
                return str.ToLower();

            return char.ToLower(str[0]) + str[1..];
        }

        public static string ToPascalCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            str = str.Trim();

            if (str.Length == 1)
                return str.ToUpper();

            return char.ToUpper(str[0]) + str[1..];
        }
    }
}
