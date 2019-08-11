using ChatProject.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ChatProject.ClientSide
{
    public partial class Program
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddDomain();
            services.AddSingleton<RunningClient>();
        }

        public static async Task Run()
        {
            var running = GetService<RunningClient>();
            var client = await running.ConnectAsync();
            await running.RunningAsync(client);
        }
    }
}
