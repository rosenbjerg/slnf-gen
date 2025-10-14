using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.TestHost;
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
}