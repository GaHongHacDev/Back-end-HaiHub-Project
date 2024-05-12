using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Infrastructure.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            //services.AddScoped<IYourService, YourServiceImplementation>();
            return services;
        }
    }
}
