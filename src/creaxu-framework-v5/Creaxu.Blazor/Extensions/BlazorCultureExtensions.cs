using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Creaxu.Blazor.Extensions
{
    public static class BlazorCultureExtensions
    {
        public static void AddBlazorCulture(this IServiceCollection services)
        {
            services.AddScoped<BlazorCulture>();
        }
        
        public static async Task SetDefaultCulture(this WebAssemblyHost host)
        {
            var blazorCulture = host.Services.GetRequiredService<BlazorCulture>();
            
            var result = await blazorCulture.GetBlazorCulture();
            
            var culture = result != null ? new CultureInfo(result) : new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}