using System.CommandLine;
using SolutionFilterGenerator;

var rootCommand = new CreateFilterCommand();
return await rootCommand.InvokeAsync(args);