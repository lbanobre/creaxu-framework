using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Creaxu.Blazor.Services;

namespace Creaxu.Blazor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddScoped<JwtAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>(
                provider => provider.GetRequiredService<JwtAuthenticationStateProvider>()
            );
            services.AddScoped<IJwtAuthenticationStateProvider, JwtAuthenticationStateProvider>(
               provider => provider.GetRequiredService<JwtAuthenticationStateProvider>()
            );

            return services;
        }

        public static IServiceCollection AddHttpService(this IServiceCollection services, string baseAddress)
        {
            services.AddSingleton(new HttpClient() { BaseAddress = new Uri(baseAddress), Timeout = TimeSpan.FromHours(1) });
            services.AddScoped<IHttpService, HttpService>();

            return services;
        }
    }
}
