using Core.Events;
using Core.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Commands;

public static class CommandDependencies
{
    public static void AddCommandsDependencies(this IServiceCollection services)
    {
        services.TryAddSingleton<IBlastRepository, InMemoryBlastRepository>();
        services.TryAddSingleton<IHolesRepository, InMemoryHolesRepository>();
        services.TryAddSingleton<IEventStore, InMemoryEventStore>();
        services.AddScoped<ICommands, Commands>();
    }
}