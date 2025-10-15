using SolutionFilterGenerator;

var rootCommand = new CreateFilterCommand();
var parsed = rootCommand.Parse(args);
return await parsed.InvokeAsync();