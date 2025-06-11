using Microsoft.Extensions.DependencyInjection;
using QatarPaymentTest.Repositories.Interface;
using QatarPaymentTest.Repositories.Repos;
using QatarPaymentTest.Services.Implementation;
using QatarPaymentTest.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QatarPaymentTest.Extensions
{

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            // Register AutoMapper with assembly scanning
            services.AddAutoMapper(typeof(MappingProfile));

            // Register Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();

            // Register Services
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<ICustomFieldService, CustomFieldService>();

            return services;
        }
    }

}
