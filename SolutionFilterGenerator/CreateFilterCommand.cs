using System.CommandLine;
using System.Text.Json;

namespace SolutionFilterGenerator;

public class CreateFilterCommand : RootCommand
{
    public CreateFilterCommand()
    {
        base.Name = "slnf-gen";
        base.Description = "Solution filter generator\nExample of excluding test projects: 'slnf-gen MySolution.sln --exclude **/*Test*' which creates MySolution.slnf";

        var solutionFileArgument = new Argument<FileInfo>(name: "path to sln", description: "File path to the solution (.sln) file");

        var excludeOption = new Option<string[]>(name: "--exclude", description: "Exclusion glob(s)");
        excludeOption.AddAlias("-e");

        var includeOption = new Option<string[]>(name: "--include", description: "Inclusion glob(s)");
        includeOption.SetDefaultValue(new[] { "**/*" });
        includeOption.AddAlias("-i");

        var outputOption = new Option<string?>(name: "--output-path", description: "Optional path of output .slnf file. Defaults to same directory and file name as .sln file but different file extension");
        outputOption.AddAlias("-o");

        AddArgument(solutionFileArgument);
        AddOption(excludeOption);
        AddOption(includeOption);
        AddOption(outputOption);

        this.SetHandler(Handle, solutionFileArgument, includeOption, excludeOption, outputOption);
    }

    private void Handle(FileInfo solutionFile, string[] includes, string[] excludes, string? outputPath)
    {
        var slnf = FilterGenerator.Generate(solutionFile, includes, excludes, outputPath);
        var slnfJson = JsonSerializer.Serialize(slnf, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });
        var slnfPath = outputPath ?? $"{solutionFile.FullName}f";
        File.WriteAllText(slnfPath, slnfJson);

        Console.WriteLine($"Generated slnf file with {slnf.Solution.Projects.Length} projects at {slnfPath}:\n{string.Join('\n', slnf.Solution.Projects)}");
    }
}