using System;
using System.Collections.Generic;

namespace Sim.Utility.CodeGenerator
{
    internal static class CodeFormatter
    {
        public static string Format(string input)
        {
            string[] lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var formattedLines = new List<string>();
            int indentLevel = 0;
            string indent = "    "; // 4 spaces

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // Remove whitespaces in otherwise empty line
                if (string.IsNullOrWhiteSpace(trimmedLine))
                {
                    formattedLines.Add("");
                    continue;
                }

                if (trimmedLine == "}")
                {
                    indentLevel = Math.Max(0, indentLevel - 1);

                    // Remove previous empty line if it exists
                    if (formattedLines.Count > 0 && string.IsNullOrWhiteSpace(formattedLines[^1]))
                    {
                        formattedLines.RemoveAt(formattedLines.Count - 1);
                    }
                }

                formattedLines.Add(new string(' ', indentLevel * indent.Length) + trimmedLine);

                if (trimmedLine.Trim().EndsWith("{", StringComparison.Ordinal))
                {
                    indentLevel++;
                }
            }

            return string.Join("\n", formattedLines);
        }
    }
}
