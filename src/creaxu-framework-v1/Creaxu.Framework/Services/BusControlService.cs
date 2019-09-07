using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Creaxu.Framework.Services
{
    public interface IBusControlService
    {
        Task StartAsync();
        Task StopAsync();
    }

    public class BusControlService
    {
        private readonly IConfiguration _configuration;
        private IBusControl _bus;

        public BusControlService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private void ConfigureBus()
        {
            _bus = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                var host = cfg.Host(new Uri(_configuration["BusControl:Endpoint"]), h =>
                {
                    h.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(_configuration["BusControl:SharedAccessKeyName"], _configuration["BusControl:SharedAccessKey"]);
                });
            });
        }

        public async Task StartAsync()
        {
            ConfigureBus();

            await _bus.StartAsync();
        }

        public async Task StopAsync()
        {
            await _bus?.StopAsync(TimeSpan.FromSeconds(30));
        }
    }
}
