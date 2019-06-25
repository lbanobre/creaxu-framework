using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Creaxu.Framework.Services
{
    public interface IStripeService
    {
        UsageRecord AddUsageRecord(string subscriptionItemId, int quantity);
        Subscription CancelSubscription(string stripeSubscriptionId);
        Subscription Change(string subscriptionId, string subscriptionItemId, string overageSubscriptionItemId, string monthlyPlanId, string overagePlanId);
        Subscription GetSubscription(string subscriptionId);
        Invoice GetUpcomingInvoice(string customerId);
        Subscription Subscribe(string email, string name, string sourceToken, string monthlyPlanId, string overagePlanId);
    }

    public class StripeService : IStripeService
    {
        private readonly IConfiguration _configuration;

        public StripeService(IConfiguration configuration)
        {
            _configuration = configuration;

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public Subscription Subscribe(string email, string name, string source, string monthlyPlanId, string overagePlanId)
        {
            var customerService = new CustomerService();
            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = email,
                Description = name,
                Source = source
            });

            var subscriptionService = new SubscriptionService();

            var items = new List<SubscriptionItemOption> {
                new SubscriptionItemOption { PlanId = monthlyPlanId },
                new SubscriptionItemOption { PlanId = overagePlanId }
            };

            var subscription = subscriptionService.Create(new SubscriptionCreateOptions
            {
                CustomerId = customer.Id,
                Items = items,
            });

            return subscription;
        }

        public Subscription GetSubscription(string subscriptionId)
        {
            var subscriptionService = new SubscriptionService();

            return subscriptionService.Get(subscriptionId);
        }

        public Invoice GetUpcomingInvoice(string customerId)
        {
            var invoiceService = new InvoiceService();

            var upcomingInvoiceOptions = new UpcomingInvoiceOptions()
            {
                CustomerId = customerId
            };

            return invoiceService.Upcoming(upcomingInvoiceOptions);
        }

        public UsageRecord AddUsageRecord(string subscriptionItemId, int quantity)
        {
            var usageRecordService = new UsageRecordService();

            var usageRecordOptions = new UsageRecordCreateOptions()
            {
                Quantity = quantity,
                Timestamp = DateTime.UtcNow
            };

            return usageRecordService.Create(subscriptionItemId, usageRecordOptions);
        }

        public Subscription Change(string subscriptionId, string subscriptionItemId, string overageSubscriptionItemId, string monthlyPlanId, string overagePlanId)
        {
            var subscriptionService = new SubscriptionService();

            var items = new List<SubscriptionItemUpdateOption> {
                new SubscriptionItemUpdateOption {
                    Id = subscriptionItemId,
                    PlanId = monthlyPlanId,
                },
                new SubscriptionItemUpdateOption {
                    Id = overageSubscriptionItemId,
                    PlanId = overagePlanId,
                }
            };
            var options = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                Items = items,
            };

            return subscriptionService.Update(subscriptionId, options);
        }

        public Subscription CancelSubscription(string stripeSubscriptionId)
        {
            var subscriptionService = new SubscriptionService();

            return subscriptionService.Cancel(stripeSubscriptionId, new SubscriptionCancelOptions { InvoiceNow = true });
        }
    }
}
