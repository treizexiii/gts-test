using Core.Events;
using Core.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Query;

public static class QueryDependencies
{
    public static void AddQueryDependencies(this IServiceCollection services)
    {
        services.TryAddSingleton<IBlastRepository, InMemoryBlastRepository>();
        services.TryAddSingleton<IHolesRepository, InMemoryHolesRepository>();
        services.TryAddSingleton<IEventStore, InMemoryEventStore>();
        services.AddScoped<IQuery, Query>();
    }
}