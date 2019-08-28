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
        Task<LoginLink> CreateLoginLinkAsync(string accountId, string redirectUrl);
        Task<Customer> CreateCustomerAsync(string token, string email, string name);
        Task<StripeList<Card>> GetCardListAsync(string customerId);
        Task<Source> AttachAsync(string customerId, string token);
        Task<Charge> CreateChargeAsync(string customerId, string source, string destination, string description, decimal totalAmount, decimal applicationFeeAmount, Dictionary<string, string> metadata);
        Task<Charge> CreateCaptureAsync(string chargeId, decimal amount, decimal applicationFeeAmount);
        Task<StripeList<Payout>> GetPayoutsAsync(string accountId);
        Task<List<string>> GetPayoutTransactionsAsync(string accountId, string payoutId);
        Task<Refund> CreateRefundAsync(string chargeId);
        Task<OAuthToken> CreateOAuthTokenAsync(string code);
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


        public async Task<LoginLink> CreateLoginLinkAsync(string accountId, string redirectUrl)
        {
            var options = new LoginLinkCreateOptions
            {
                RedirectUrl = redirectUrl
            };

            var service = new LoginLinkService();
            return await service.CreateAsync(accountId, options);
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

        public async Task<Charge> CreateChargeAsync(string customerId, string source, string destination, string description, decimal totalAmount, decimal applicationFeeAmount, Dictionary<string, string> metadata)
        {
            var options = new ChargeCreateOptions
            {
                CustomerId = customerId,
                Source = source,
                Description = description,
                Amount = (long)(totalAmount * 100),
                ApplicationFeeAmount = (long)(applicationFeeAmount * 100),
                TransferData = new ChargeTransferDataOptions { Destination = destination },
                Currency = "usd",
                Capture = false,
                Metadata = metadata
            };

            var service = new ChargeService();
            return await service.CreateAsync(options);
        }

        public async Task<Charge> CreateCaptureAsync(string chargeId, decimal amount, decimal applicationFeeAmount)
        {
            var options = new ChargeCaptureOptions
            {
                Amount = (long)(amount * 100),
                ApplicationFeeAmount = (long)(applicationFeeAmount * 100)
            };

            var service = new ChargeService();
            return await service.CaptureAsync(chargeId, options);
        }

        public async Task<StripeList<Payout>> GetPayoutsAsync(string accountId)
        {
            var payoutService = new PayoutService();

            return await payoutService.ListAsync(new PayoutListOptions { Limit = 100 }, new RequestOptions { StripeAccount = accountId });
        }

        public async Task<List<string>> GetPayoutTransactionsAsync(string accountId, string payoutId)
        {
            var result = new List<string>();

            var balanceTransactionService = new BalanceTransactionService();
            var chargeService = new ChargeService();
            var transferService = new TransferService();

            var requestOptions = new RequestOptions { StripeAccount = accountId };

            var transactions = await balanceTransactionService.ListAsync(new BalanceTransactionListOptions { PayoutId = payoutId, Type = "payment", Limit = 100 }, requestOptions);
            foreach (var transaction in transactions)
            {
                var payment = await chargeService.GetAsync(transaction.SourceId, null, requestOptions);
                var transfer = await transferService.GetAsync(payment.SourceTransferId);

                result.Add(transfer.SourceTransactionId);
            }

            return result;
        }

        public async Task<Refund> CreateRefundAsync(string chargeId)
        {
            var options = new RefundCreateOptions
            {
                ChargeId = chargeId,
            };

            var service = new RefundService();
            return await service.CreateAsync(options);
        }

        public async Task<OAuthToken> CreateOAuthTokenAsync(string code)
        {
            var options = new OAuthTokenCreateOptions
            {
                ClientSecret = StripeConfiguration.ApiKey,
                Code = code,
                GrantType = "authorization_code"
            };

            var service = new OAuthTokenService();
            return await service.CreateAsync(options);
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
