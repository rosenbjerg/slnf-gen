using System;
using System.IO;
using System.Linq;
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
        var slnf = FilterGenerator.Generate(_solutionFile, Array.Empty<string>(), Array.Empty<string>(), null);
        Assert.AreEqual(2, slnf.Solution.Projects.Length);
    }
    [Test]
    public void ExcludeAll()
    {
        var slnf = FilterGenerator.Generate(_solutionFile, Array.Empty<string>(), new []{ "**/*" }, null);
        Assert.AreEqual(0, slnf.Solution.Projects.Length);
    }

    [Test]
    public void ExcludeTests()
    {
        var slnf = FilterGenerator.Generate(_solutionFile, Array.Empty<string>(), new []{ "**/*Test*" }, null);
        Assert.True(slnf.Solution.Projects.All(p => !p.Contains("Test")));
        Assert.AreEqual(1, slnf.Solution.Projects.Length);
    }

    [Test]
    public void IncludeOnlyTests()
    {
        var slnf = FilterGenerator.Generate(_solutionFile, new []{ "**/*Test*" }, Array.Empty<string>(), null);
        Assert.True(slnf.Solution.Projects.All(p => p.Contains("Test")));
        Assert.AreEqual(1, slnf.Solution.Projects.Length);
    }
}