using System.CommandLine;
using System.Text.Json;

namespace SolutionFilterGenerator;

public class CreateFilterCommand : RootCommand
{
    public CreateFilterCommand()
    {
        Description = "Solution filter generator\n" +
                      "Example of excluding test projects: 'slnf-gen MySolution.sln --exclude **/*Test*' which creates MySolution.slnf";

        var solutionFileArgument = new Argument<FileInfo>("path to sln")
        {
            Description = "File path to the solution (.sln) file",
            Arity = ArgumentArity.ExactlyOne
        };

        var excludeOption = new Option<string[]>("--exclude", "-e")
        {
            Description = "Exclusion glob(s)",
            DefaultValueFactory = _ => [],
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        var includeOption = new Option<string[]>("--include", "-i")
        {
            Description = "Inclusion glob(s)",
            DefaultValueFactory = _ => ["**/*"],
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        var outputOption = new Option<string?>("--output-path", "-o")
        {
            Description = "Optional path of output .slnf file. Defaults to same directory and file name as .sln file but different file extension",
            DefaultValueFactory = _ => null,
            Arity = ArgumentArity.ZeroOrOne
        };

        Add(solutionFileArgument);
        Add(excludeOption);
        Add(includeOption);
        Add(outputOption);

        SetAction(result => Handle(result.GetRequiredValue(solutionFileArgument), result.GetRequiredValue(includeOption),
            result.GetRequiredValue(excludeOption), result.GetValue(outputOption)));
    }

    private void Handle(FileInfo solutionFile, string[] includes, string[] excludes, string? outputPath)
    {
        var slnf = FilterGenerator.Generate(solutionFile, includes, excludes, outputPath);
        var slnfJson = JsonSerializer.Serialize(slnf,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });
        var slnfPath = outputPath ?? $"{solutionFile.FullName}f";
        File.WriteAllText(slnfPath, slnfJson);

        Console.WriteLine($"Generated slnf file with {slnf.Solution.Projects.Length} projects at {slnfPath}:\n{string.Join('\n', slnf.Solution.Projects)}");
    }
}