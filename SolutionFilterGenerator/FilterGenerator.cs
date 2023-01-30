using Microsoft.Build.Construction;
using Microsoft.Extensions.FileSystemGlobbing;
using SolutionFilterGenerator.Models;

namespace SolutionFilterGenerator;

public static class FilterGenerator
{
    public static SolutionFilter Generate(FileInfo solutionFile, string[] includes, string[] excludes, string? outputPath)
    {
        var matcher = new Matcher();
        if (!includes.Any())
            matcher.AddInclude("**/*.*");
        matcher.AddIncludePatterns(includes);
        matcher.AddExcludePatterns(excludes);

        var solution = SolutionFile.Parse(solutionFile.FullName);
        var projectPaths = solution.ProjectsInOrder.Select(p => Path.GetFullPath(p.AbsolutePath));
        var filteredProjects = matcher.Match(solutionFile.DirectoryName!, projectPaths).Files
            .Select(p => p.Path)
            .ToArray();

        var relativeSlnPath = Path.GetRelativePath(outputPath ?? solutionFile.DirectoryName!, solutionFile.FullName);
        return new SolutionFilter(new Solution(relativeSlnPath, filteredProjects));
    }
}