using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace Creaxu.Framework.Services
{
    public interface IStripeService
    {
        Task<Customer> CreateCustomerAsync(string token, string email, string name);
        Task<StripeList<Card>> GetCardListAsync(string customerId);
        Task<Source> AttachAsync(string customerId, string token);
        Task<Session> CreateSessionAsync(string customerId, string email, string itemName, string itemDescription, decimal amount, long quantity, string successUrl, string cancelUrl);
        Task<Charge> CreateChargeAsync(string customerId, string source, string description, decimal amount);
        Task<Session> GetSessionAsync(string sessionId);
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

        public async Task<Customer> CreateCustomerAsync(string token, string email, string name)
        {
            var options = new CustomerCreateOptions
            {
                Source = token,
                Email = email,
                Name = name
            };

            var service = new CustomerService();
            return await service.CreateAsync(options);
        }

        public async Task<StripeList<Card>> GetCardListAsync(string customerId)
        {
            var service = new CardService();
            return await service.ListAsync(customerId);
        }

        public async Task<Source> AttachAsync(string customerId, string token)
        {
            var options = new SourceAttachOptions
            {
                Source = token,
            };
            var service = new SourceService();
            return await service.AttachAsync(customerId, options);
        }

        public async Task<Session> CreateSessionAsync(string customerId, string email, string itemName, string itemDescription, decimal amount, long quantity, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                CustomerId = customerId,
                CustomerEmail = customerId == null ? email : null,
                BillingAddressCollection = "required",
                PaymentMethodTypes = new List<string> {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions> {
                    new SessionLineItemOptions {
                        Name = itemName,
                        Description = itemDescription,
                        Amount = (long)(amount * 100),
                        Currency = "usd",
                        Quantity = quantity,
                    },
                },
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    //ApplicationFeeAmount
                    CaptureMethod = "manual",
                },
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
            };

            var service = new SessionService();

            return await service.CreateAsync(options);
        }

        public async Task<Charge> CreateChargeAsync(string customerId, string source, string description, decimal amount)
        {
            var options = new ChargeCreateOptions
            {
                CustomerId = customerId,
                Source = source,
                Description = description,
                Amount = (long)(amount * 100),
                Currency = "usd",
                Capture = false
            };

            var service = new ChargeService();
            return await service.CreateAsync(options);
        }

        public async Task<Session> GetSessionAsync(string sessionId)
        {
            var service = new SessionService();

            return await service.GetAsync(sessionId);
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
