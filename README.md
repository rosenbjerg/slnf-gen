# A dotnet tool for generating solution filter (.slnf) files based on globs

[![NuGet Badge](https://buildstats.info/nuget/SolutionFilterGenerator)](https://www.nuget.org/packages/SolutionFilterGenerator/)
[![codecov](https://codecov.io/gh/rosenbjerg/slnf-gen/branch/main/graph/badge.svg)](https://codecov.io/gh/rosenbjerg/slnf-gen)

```
Description:
  Solution filter generator
  Example of excluding test projects: 'slnf-gen MySolution.sln --exclude **/*Test*' which creates MySolution.slnf

Usage:
  slnf-gen <path to sln> [options]

Arguments:
  <path to sln>  File path to the solution (.sln) file

Options:
  -e, --exclude <exclude>          Exclusion glob(s)
  -i, --include <include>          Inclusion glob(s) [default: **/*]
  -o, --output-path <output-path>  Optional path of output .slnf file. Defaults to same directory and file name as .sln file but different file extension
  --version                        Show version information
  -?, -h, --help                   Show help and usage information
```
