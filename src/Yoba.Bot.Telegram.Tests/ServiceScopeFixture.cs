using Microsoft.Extensions.DependencyInjection;

namespace Yoba.Bot.Tests;

public class ServiceScopeFixture : IDisposable
{
    public IServiceScope Scope { get; }

    public ServiceScopeFixture()
    {
        Scope = Setup.GetScope();
    }

    public void Dispose()
    {
        Scope?.Dispose();
    }
}