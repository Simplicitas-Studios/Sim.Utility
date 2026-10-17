using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class UtilityPlayerSmokeBuild
{
    public static void Build()
    {
        Directory.CreateDirectory("Logs");
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var root = new GameObject("Serialized GUID");
        root.AddComponent<GuidSmokeComponent>();
        EditorSceneManager.SaveScene(scene, "Assets/Smoke.unity");
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);

        var options = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Smoke.unity" },
            locationPathName = "Builds/UtilitySmoke.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.Development
        };
        BuildReport report = BuildPipeline.BuildPlayer(options);
        UnityEditor.Compilation.Assembly runtimeAssembly = CompilationPipeline.GetAssemblies(AssembliesType.Player)
            .Single(assembly => assembly.name == "Sim.Utility");
        string[] references = runtimeAssembly.assemblyReferences.Select(assembly => assembly.name)
            .Concat(runtimeAssembly.compiledAssemblyReferences.Select(Path.GetFileNameWithoutExtension))
            .Distinct().OrderBy(name => name).ToArray();
        var evidence = new PlayerBuildEvidence
        {
            unityVersion = Application.unityVersion,
            target = options.target.ToString(),
            backend = "Mono2x",
            result = report.summary.result.ToString(),
            totalErrors = report.summary.totalErrors,
            totalWarnings = report.summary.totalWarnings,
            utilityReferences = references,
            utilityDefines = runtimeAssembly.defines.OrderBy(value => value).ToArray(),
            executable = options.locationPathName
        };
        File.WriteAllText("Logs/u1a-player-build.json", JsonUtility.ToJson(evidence, true) + Environment.NewLine);

        if (report.summary.result != BuildResult.Succeeded ||
            references.Any(name => name.StartsWith("UnityEditor", StringComparison.Ordinal) ||
                name.StartsWith("Sim.Faciem", StringComparison.Ordinal)) ||
            runtimeAssembly.defines.Contains("UNITY_EDITOR"))
        {
            throw new InvalidOperationException("Standalone Utility player validation failed. See Logs/u1a-player-build.json.");
        }
    }

    [Serializable]
    private sealed class PlayerBuildEvidence
    {
        public string unityVersion;
        public string target;
        public string backend;
        public string result;
        public int totalErrors;
        public int totalWarnings;
        public string[] utilityReferences;
        public string[] utilityDefines;
        public string executable;
    }
}
