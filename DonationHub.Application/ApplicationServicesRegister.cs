using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DonationHub.Application
{
    public static class ApplicationServicesRegister
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            var assembly = typeof(ApplicationServicesRegister).Assembly;

            services.AddAutoMapper(c => { }, assembly);
            services.AddMediatR(c => c.RegisterServicesFromAssembly(assembly));
            return services;
        }
    }
}
