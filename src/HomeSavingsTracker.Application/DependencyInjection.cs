using System.Reflection;
using FluentValidation;
using HomeSavingsTracker.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace HomeSavingsTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
