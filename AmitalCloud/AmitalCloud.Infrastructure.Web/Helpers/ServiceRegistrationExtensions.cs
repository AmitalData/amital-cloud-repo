using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

public static class ServiceRegistrationExtensions
{
    public static void AddAllQueryServices(this IServiceCollection services, Assembly assembly)
    {
        var queryServiceTypes = assembly.GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Name.EndsWith("QueryService", StringComparison.OrdinalIgnoreCase));

        foreach (var type in queryServiceTypes)
        {
            services.AddScoped(type);
        }
    }
}
