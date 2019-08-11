using ChatProject.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ChatProject.Server
{
    public partial class Program
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddDomain();
            services.AddTransient<RunningServer>();     
        }

        public static async Task Run()
        {
            var running = GetService<RunningServer>();
            await running.StartListening();
        }
    }
}
