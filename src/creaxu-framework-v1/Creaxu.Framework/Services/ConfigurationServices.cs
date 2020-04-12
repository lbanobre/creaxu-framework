using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Text;
using Microsoft.Azure;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Primitives;

namespace Creaxu.Framework.Services
{
  public class ConfigurationServices:IConfiguration
   {
      private static IConfiguration _configuration = null;
      private static IConfigurationRefresher _refresher = null;

      public ConfigurationServices()
      {
         var builder = new ConfigurationBuilder();


         builder.AddAzureAppConfiguration(options =>
         {
            options.Connect(AzureConfiguration.ConectionString)
               .ConfigureRefresh(refresh =>
               {
                  refresh.SetCacheExpiration(TimeSpan.FromHours(1));
               })
               .Select(KeyFilter.Any, LabelFilter.Null)
               .Select(KeyFilter.Any, AzureConfiguration.Environment); ;

            _refresher = options.GetRefresher();
         },true);

         _configuration = builder.Build();
      }

      public IConfigurationSection GetSection(string key)
      {
         return _configuration.GetSection(key);
      }

      public IEnumerable<IConfigurationSection> GetChildren()
      {
         return _configuration.GetChildren();
      }

      public IChangeToken GetReloadToken()
      {
         return _configuration.GetReloadToken();
      }

      public string this[string key]
      {
         get
         {
            _refresher.RefreshAsync();

            return _configuration[key];
         }

         set => _configuration[key] = value;
      }
   }

  public static class AzureConfiguration
  {
     public static string ConectionString { get; set; }

     public static string Environment { get; set; }
   }
}
