using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SurveySystem.Application.Interfaces;
using SurveySystem.Application.UseCases;
using SurveySystem.Infrastructure.Persistance;
using SurveySystem.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurveySystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                  configuration.GetConnectionString("DefaultConnection"),
                     b =>
                        {
                            b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                            b.UseNetTopologySuite();
                        }));

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICsvParserService, CsvParserService>();
            services.AddScoped<ITaxCalculationService, TaxCalculationService>();
            
            return services;
        }
    }
}
