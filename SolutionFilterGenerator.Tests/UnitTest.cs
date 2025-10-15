using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SolutionFilterGenerator.Tests;

public class Tests
{
    private FileInfo _solutionFile = null!;

    [SetUp]
    public void Setup()
    {
        _solutionFile = new FileInfo("../../../../SolutionFilterGenerator.sln");
    }

    [Test]
    public void DefaultIncludesAll()
    {
        var slnf = FilterGenerator.Generate(_solutionFile, [], [], null);
        Assert.That(slnf.Solution.Projects.Length, Is.EqualTo(2));
    }

    [Test]
    public void ExcludeAll()
    {
        var slnf = FilterGenerator.Generate(_solutionFile, [], ["**/*"], null);
        Assert.That(slnf.Solution.Projects.Length, Is.Zero);
    }

    [Test]
    public void ExcludeTests()
    {
        var slnf = FilterGenerator.Generate(_solutionFile, [], ["**/*Test*"], null);
        Assert.That(slnf.Solution.Projects.All(p => !p.Contains("Test")), Is.True);
        Assert.That(slnf.Solution.Projects.Length, Is.EqualTo(1));
    }

    [Test]
    public void IncludeOnlyTests()
    {
        var slnf = FilterGenerator.Generate(_solutionFile, ["**/*Test*"], [], null);
        Assert.That(slnf.Solution.Projects.All(p => p.Contains("Test")), Is.True);
        Assert.That(slnf.Solution.Projects.Length, Is.EqualTo(1));
    }

    [Test]
    public void CanInvokeWithOnlySln()
    {
        var command = new CreateFilterCommand();
        var parsed = command.Parse("test.sln");
        Assert.That(parsed.Errors, Is.Empty);
    }

    [Test]
    public async Task VerifyFullInvokeWithExclude()
    {
        var command = new CreateFilterCommand();
        var tempFile = Path.GetTempFileName();

        try
        {
            var parsed = command.Parse($"{_solutionFile.FullName} -e **/*Test* -o {tempFile}");
            var result = await parsed.InvokeAsync();
            var slnfContent = await File.ReadAllTextAsync(tempFile);

            Assert.That(result, Is.EqualTo(0));
            Assert.That(slnfContent, Contains.Substring("SolutionFilterGenerator/SolutionFilterGenerator.csproj"));
            Assert.That(slnfContent, Does.Not.Contain("SolutionFilterGenerator/SolutionFilterGenerator.Tests.csproj"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}