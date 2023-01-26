using System.CommandLine;
using System.Text.Json;
using Microsoft.Build.Construction;
using Microsoft.Extensions.FileSystemGlobbing;
using SolutionFilterGenerator.Models;

var solutionFileArgument = new Argument<FileInfo>(name: "path to sln", description: "File path to the solution (.sln) file");
var excludeOption = new Option<string[]>(name: "--exclude", description: "Exclusion glob(s)");
excludeOption.AddAlias("-e");

var includeOption = new Option<string[]>(name: "--include", description: "Inclusion glob(s)");
includeOption.SetDefaultValue(new[] { "**/*" });
includeOption.AddAlias("-i");

var outputOption = new Option<string?>(name: "--output-path", description: "Optional path of output .slnf file. Defaults to same directory and file name as .sln file but different file extension");
outputOption.AddAlias("-o");

var rootCommand = new RootCommand("Solution filter generator\nExample of excluding test projects: 'slnf-gen MySolution.sln --exclude **/*Test*' which creates MySolution.slnf") { Name = "slnf-gen" };
rootCommand.AddArgument(solutionFileArgument);
rootCommand.AddOption(excludeOption);
rootCommand.AddOption(includeOption);
rootCommand.AddOption(outputOption);

rootCommand.SetHandler((solutionFile, includes, excludes, outputPath) =>
    {
        var matcher = new Matcher();
        if (!includes.Any())
            matcher.AddInclude("**/*");
        matcher.AddIncludePatterns(includes);
        matcher.AddExcludePatterns(excludes);
        
        var solution = SolutionFile.Parse(solutionFile.FullName);
        var projectPaths = solution.ProjectsInOrder.Select(p => p.AbsolutePath);
        var filteredProjects = matcher.Match(solutionFile.DirectoryName!, projectPaths).Files
            .Select(p => p.Path.Replace('\\', '/').Replace('/', Path.DirectorySeparatorChar))
            .ToArray();

        var relativeSlnPath = Path.GetRelativePath(outputPath ?? solutionFile.DirectoryName!, solutionFile.FullName);
        var slnf = new SolutionFilter(new Solution(relativeSlnPath, filteredProjects));
        var slnfJson = JsonSerializer.Serialize(slnf, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
        var slnfPath = outputPath ?? $"{solutionFile.FullName}f";
        File.WriteAllText(slnfPath, slnfJson);

        Console.WriteLine($"Generated slnf file with {filteredProjects.Length} projects at {slnfPath}");
    },
    solutionFileArgument, includeOption, excludeOption, outputOption);

return await rootCommand.InvokeAsync(args);