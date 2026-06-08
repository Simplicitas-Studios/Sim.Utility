using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Sim.Utility.CodeGenerator
{
    public static class CodeGenerator
    {
        private const string AutoGenerationNotice = "// This file is auto generated. Any changes made to this file will be overriden.";

        public static void Generate(string templatePath, string outputPath, Dictionary<string, object> variables, out bool hadChanges)
        {
            try
            {
                if (!File.Exists(templatePath))
                {
                    throw new FileNotFoundException($"Template file doesn't exist: {templatePath}.");
                }

                string templateContent = File.ReadAllText(templatePath);

                templateContent = ParseTemplateParameters(templateContent, out var parameters);

                string generatedFile = $"\n{AutoGenerationNotice}\n{templateContent}";

                generatedFile = GenerateLoops(generatedFile, variables);
                generatedFile = InsertVariables(generatedFile, variables);

                generatedFile = HandleParameters(parameters, generatedFile, outputPath);

                generatedFile = CodeFormatter.Format(generatedFile);

                WriteFileIfChanged(outputPath, generatedFile, out hadChanges);
            }
            catch (Exception e)
            {
                hadChanges = false;
                Debug.LogError($"An error occurred while trying to generate code from template at {templatePath}: {e}");
            }
        }

        private static string ParseTemplateParameters(string file, out Dictionary<string, string> parameters)
        {
            parameters = new Dictionary<string, string>();

            var match = Regex.Match(file, @"---\s*\n(?<body>(.*\n)*?)\s*---", RegexOptions.Multiline);
            if (match.Success)
            {
                string body = match.Groups["body"].Value;
                foreach (var line in body.Split('\n'))
                {
                    var paramMatch = Regex.Match(line, @"^\s*(\w+)\s*:\s*(.+)\s*$");
                    if (paramMatch.Success)
                    {
                        parameters[paramMatch.Groups[1].Value.Trim()] = paramMatch.Groups[2].Value.Trim();
                    }
                }

                file = file.Remove(match.Index, match.Length).TrimStart();
            }

            return file;
        }


        private static string GenerateLoops(string file, Dictionary<string, object> variables)
        {
            return Regex.Replace(file, @"\{\{\s*foreach\s+(\w+)\s+in\s+(.*?)\s*\}\}([\s\S]*?)\{\{\s*end\s*\}\}", match =>
            {
                string iteratorVariable = match.Groups[1].Value;
                string listVariableName = match.Groups[2].Value.Trim();
                string loopContent = match.Groups[3].Value.TrimStart();

                loopContent = loopContent.Replace(iteratorVariable, $".{iteratorVariable}");

                if (listVariableName.StartsWith(".", StringComparison.Ordinal))
                {
                    listVariableName = listVariableName[1..];
                }

                if (EvaluateExpression(listVariableName, variables) is not IEnumerable enumerable)
                {
                    return string.Empty;
                }

                var result = new StringBuilder();

                foreach (object item in enumerable)
                {
                    var localContext = new Dictionary<string, object>(variables)
                    {
                        [iteratorVariable] = item
                    };

                    // Replace variables in loop body with new context
                    string evaluated = InsertVariables(loopContent, localContext);
                    result.Append(evaluated);
                }

                return result.ToString();
            });
        }

        private static string InsertVariables(string file, Dictionary<string, object> variables)
        {
            return Regex.Replace(file, @"\{\{\s*(.*?)\s*\}\}", match =>
            {
                string expression = match.Groups[1].Value;

                if (expression.StartsWith(".", StringComparison.Ordinal))
                {
                    string trimmedExpr = expression[1..];
                    object result = EvaluateExpression(trimmedExpr, variables);
                    return result?.ToString() ?? "NULL";
                }

                return match.Value;
            });
        }

        private static string AddAutoDetectedNameSpace(string file, string outputDirectory)
        {
            if (ContainsNameSpace(file) || !NamespaceHelper.TryExtractNamespaceFromNearbyFiles(outputDirectory, out string namespaceName))
            {
                return file;
            }

            if (file.Contains("{{ .namespace }}"))
            {
                return file.Replace("{{ .namespace }}", namespaceName);
            }

            var expectedNamespacePosition = Regex.Match(file, @"^(\s*)(public|internal)?\s*(static\s*)?(class|struct|interface|enum)\s+\w+", RegexOptions.Multiline);
            if (!expectedNamespacePosition.Success)
            {
                return file;
            }

            int insertIndex = expectedNamespacePosition.Index;

            while (insertIndex < file.Length && char.IsWhiteSpace(file[insertIndex]))
            {
                insertIndex++;
            }

            int lastBraceIndex = file.LastIndexOf('}');
            if (lastBraceIndex == -1)
            {
                return file;
            }

            string before = file[..insertIndex];
            string inside = file.Substring(insertIndex, lastBraceIndex - insertIndex + 1);
            string after = file[(lastBraceIndex + 1)..];

            return $"{before}namespace {namespaceName}\n{{\n{inside}\n}}{after}";
        }

        private static bool ContainsNameSpace(string file)
        {
            return Regex.IsMatch(file, @"^\s*namespace\s+[A-Za-z0-9_.]+", RegexOptions.Multiline);
        }

        private static string HandleParameters(Dictionary<string, string> parameters, string file, string outputPath)
        {
            foreach (var pair in parameters)
            {
                switch (pair.Key)
                {
                    case "generateNamespace":
                        if (pair.Value == "true")
                        {
                            file = AddAutoDetectedNameSpace(file, Path.GetDirectoryName(outputPath));
                        }

                        break;
                    default:
                        throw new ArgumentException($"Unknown template parameter: {pair.Value}.");
                }
            }

            return file;
        }

        private static object EvaluateExpression(string expression, Dictionary<string, object> variables)
        {
            try
            {
                object current;

                MatchCollection parts = Regex.Matches(expression, @"([a-zA-Z_][a-zA-Z0-9_]*)|\[(\d+)\]|\(\)");
                int partIndex;

                // Start from the root object (e.g. "myClass" in ".myClass.Name")
                if (parts.Count > 0 && variables.TryGetValue(parts[0].Value, out object root))
                {
                    current = root;
                    partIndex = 1;
                }
                else
                {
                    return $"UNDEFINED({expression})";
                }

                for (int i = partIndex; i < parts.Count; i++)
                {
                    string part = parts[i].Value;

                    if (Regex.IsMatch(part, @"^\[\d+\]$")) // Indexer
                    {
                        if (current is IEnumerable enumerable)
                        {
                            int index = int.Parse(part.Trim('[', ']'));
                            current = enumerable.Cast<object>().ElementAtOrDefault(index);
                        }
                        else
                        {
                            return $"INVALID_INDEX({part})";
                        }
                    }
                    else
                    {
                        // Parameterless method
                        if (i + 1 < parts.Count && parts[i + 1].Value == "()")
                        {
                            MethodInfo method = current?.GetType().GetMethods().FirstOrDefault(m => m.Name == part);

                            if (method == null)
                            {
                                return $"INVALID_METHOD({part})";
                            }

                            i++;
                            current = method.Invoke(current, null);
                        }
                        // Property or field
                        else
                        {
                            var prop = current?.GetType().GetProperty(part);
                            if (prop != null)
                            {
                                current = prop.GetValue(current);
                            }
                            else
                            {
                                var field = current?.GetType().GetField(part);
                                if (field != null)
                                {
                                    current = field.GetValue(current);
                                }
                                else
                                {
                                    return $"UNKNOWN_MEMBER({part})";
                                }
                            }
                        }
                    }
                }

                return current;
            }
            catch (Exception ex)
            {
                return $"ERROR({ex.Message})";
            }
        }

        private static void WriteFileIfChanged(string outputPath, string file, out bool hadChanges)
        {
            if (File.Exists(outputPath))
            {
                string existingFile = File.ReadAllText(outputPath);

                // Normalize line endings before comparison
                var existingWithoutCopyright = NormalizeLineEndings(existingFile);
                var fileWithoutCopyright = NormalizeLineEndings(file);

                if (existingWithoutCopyright == fileWithoutCopyright)
                {
                    hadChanges = false;
                    return;
                }
            }

            hadChanges = true;
            File.WriteAllText(outputPath, file);
        }

        static string NormalizeLineEndings(string text) =>
            text.Replace("\r\n", "\n").Replace("\r", "\n");
    }
}
