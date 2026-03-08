using Microsoft.Extensions.DependencyInjection;
using SurveySystem.Application.Interfaces;
using SurveySystem.Application.UseCases;

namespace SurveySystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}
