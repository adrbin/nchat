using System.IO;
using FluentAssertions;

namespace NChat.Application.Tests;

public sealed class AppHostConfigurationTests
{
    [Fact]
    public void AppHost_ShouldConfigureExplicitPostgresPassword_WhenUsingDataVolume()
    {
        var programPath = Path.Combine(GetRepositoryRoot(), "src", "NChat.AppHost", "Program.cs");
        var programSource = File.ReadAllText(programPath);

        programSource.Should().Contain(".WithDataVolume(");
        programSource.Should().Contain("\"postgres-password\"");
        programSource.Should().Contain("AddParameterFromConfiguration(");
        programSource.Should().Contain("\"Parameters:postgres-password\"");
        programSource.Should().Contain("password: postgresPassword");
    }

    private static string GetRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "NChat.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root from test output directory.");
    }
}
