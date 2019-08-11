using ChatProject.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatProject.Domain
{
    public static class DIExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddTransient<ConnectionDomainService>();
            services.AddTransient<MessageHandleDomainService>();
            return services;
        }
    }
}
