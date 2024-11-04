using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Laminar.Implementation.Base.PluginLoading;

internal static class InbuiltPluginFinder
{
    private const string RelativePluginsPath = @"src\Plugins";

    public static IEnumerable<string> GetInbuiltPlugins(string path)
    {
        var solutionFileFolder = GetSolutionFileFolder(path);

        if (solutionFileFolder is null) yield break;
        
        var inbuiltPluginsFolder = Path.Combine(solutionFileFolder.FullName, RelativePluginsPath);
        foreach (var projectPath in Directory.EnumerateDirectories(inbuiltPluginsFolder))
        {
            var outputFromProjectFolder = GetOutputFromProjectFolder(projectPath);

            if (outputFromProjectFolder is null)
            {
                continue;
            }

            foreach (var dllPath in Directory.EnumerateFiles(outputFromProjectFolder, "*.dll"))
            {
                yield return dllPath;
            }
        }
    }

    private static string? GetOutputFromProjectFolder(string projectFolder)
    {
        var debugs = Path.Combine(projectFolder, @"bin\Debug");

        return Directory.Exists(debugs) ? Directory.EnumerateDirectories(debugs).FirstOrDefault() : null;
    }

    private static DirectoryInfo? GetSolutionFileFolder(string rootPath)
    {
        var directory = new FileInfo(rootPath).Directory;
        while (directory is not null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }

        return directory;
    }
}
