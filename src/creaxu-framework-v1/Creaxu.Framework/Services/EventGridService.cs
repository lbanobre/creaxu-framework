using Microsoft.Azure;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Creaxu.Framework.Services
{
   public interface IEventGridService
   {
      void SendEvents(List<EventGridEvent> events);
   }


   public class EventGridService: IEventGridService
   {
      private readonly EventGridClient _client=null;
      private readonly string _eventGridTopicEndpoint;

      public EventGridService(IConfiguration configuration)
      { 
         _eventGridTopicEndpoint = configuration["EventGrid:TopicEndpoint"];
         var topicKey = configuration["EventGrid:key"];

         var topicCredentials = new TopicCredentials(topicKey);
         _client = new EventGridClient(topicCredentials);
      }

      public void SendEvents(List<EventGridEvent> events)
      {
         try
         {
            var topicHostname = new Uri(_eventGridTopicEndpoint).Host;
            var result = _client.PublishEventsWithHttpMessagesAsync(topicHostname, events).GetAwaiter().GetResult();

            if (result.Response.IsSuccessStatusCode)
               return;
         }
         catch (Exception e)
         {
            throw e;
         }
      }
   }
}
