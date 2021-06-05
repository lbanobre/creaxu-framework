using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using Stripe;
using Stripe.Checkout;

namespace Creaxu.Core.Services
{
    public interface IStripeService
    {
        Task<Customer> CreateCustomerAsync(string name, string email, string phone);
        Task AddCreditCardAsync(string customerId, string cardholderName, string number, string cvc, long expMonth, long expYear, string zipCode);
        Task<PaymentMethod> GetDefaultCreditCardAsync(string customerId);
        Task RemoveAllCreditCards(string customerId);
        Task<Account> CreateAccountAsync(string email);
        Task<AccountLink> CreateAccountLinkAsync(string accountId, string refreshUrl, string returnUrl, StripeService.AccountLinkTypes type);
        Task<LoginLink> CreateLoginLinkAsync(string accountId);
        Task<Account> GetAccountAsync(string accountId);
        Task<Session> CreateCheckoutSession(string customerId, string successUrl, string cancelUrl);

        Task<PaymentIntent> CreateChargeAsync(string accountId, string customerId, string paymentMethodId,
            string description, decimal amount, decimal applicationFeeAmount,
            Dictionary<string, string> metadata = null);
    }

    public class StripeService : IStripeService
    {
        private readonly IConfiguration _configuration;

        public StripeService(IConfiguration configuration)
        {
            _configuration = configuration;

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public async Task<Customer> CreateCustomerAsync(string name, string email, string phone)
        {
            var options = new CustomerCreateOptions
            {
                Name = name,
                Email = email,
                Phone = phone
            };

            var service = new CustomerService();
            return await service.CreateAsync(options);
        }
        
        public async Task AddCreditCardAsync(string customerId, string cardholderName, string number, string cvc, long expMonth, long expYear, string zipCode)
        {
            var paymentMethodCreateOptions = new PaymentMethodCreateOptions
            {
                Type = "card",
                Card = new PaymentMethodCardOptions
                {
                    Number = number,
                    Cvc = cvc,
                    ExpMonth = expMonth,
                    ExpYear = expYear
                },

                BillingDetails = new PaymentMethodBillingDetailsOptions
                {
                    Name = cardholderName,
                    Address = new AddressOptions
                    {
                        PostalCode = zipCode
                    }
                }
            };

            var paymentMethodService = new PaymentMethodService();

            var paymentMethod = await paymentMethodService.CreateAsync(paymentMethodCreateOptions);

            var cards = await paymentMethodService.ListAsync(new PaymentMethodListOptions { Customer = customerId, Type = "card" });

            foreach (var card in cards)
            {
                await paymentMethodService.DetachAsync(card.Id);
            }

            var paymentMethodAttachOptions = new PaymentMethodAttachOptions
            {
                Customer = customerId
            };

            await paymentMethodService.AttachAsync(paymentMethod.Id, paymentMethodAttachOptions);
        }
        
        public async Task<PaymentMethod> GetDefaultCreditCardAsync(string customerId)
        {
            var options = new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = "card",
            };

            var service = new PaymentMethodService();

            var paymentMethods = await service.ListAsync(options);

            return paymentMethods.FirstOrDefault();
        }

        public async Task RemoveAllCreditCards(string customerId)
        {
            var paymentMethodService = new PaymentMethodService();

            var cards = await paymentMethodService.ListAsync(new PaymentMethodListOptions { Customer = customerId, Type = "card" });

            foreach (var card in cards)
            {
                await paymentMethodService.DetachAsync(card.Id);
            }
        }
        
        public async Task<Account> CreateAccountAsync(string email)
        {
            var options = new AccountCreateOptions
            {
                Type = "express",
                Email = email
            };

            var service = new AccountService();
            return await service.CreateAsync(options);
        }
        
        public async Task<AccountLink> CreateAccountLinkAsync(string accountId, string refreshUrl, string returnUrl, AccountLinkTypes type)
        {
            var options = new AccountLinkCreateOptions
            {
                Account = accountId,
                RefreshUrl = refreshUrl,
                ReturnUrl = returnUrl,
                Type = type == AccountLinkTypes.Onboarding ? "account_onboarding" : "account_update"
            };

            var service = new AccountLinkService();
            return await service.CreateAsync(options);;
        }

        public enum AccountLinkTypes
        {
            Onboarding,
            Update
        }
        
        public async Task<LoginLink> CreateLoginLinkAsync(string accountId)
        {
            var service = new LoginLinkService();
            return await service.CreateAsync(accountId);
        }
        
        public async Task<Account> GetAccountAsync(string accountId)
        {
            var service = new AccountService();
            return await service.GetAsync(accountId);
        }

        public async Task<Session> CreateCheckoutSession(string customerId, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions {
                PaymentMethodTypes = new List<string> {
                    "card",
                },
                Mode = "setup",
                Customer = customerId,
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
            };

            var service = new SessionService();
            return await service.CreateAsync(options);
        }
        
        public async Task<PaymentIntent> CreateChargeAsync(string accountId, string customerId, string paymentMethodId, string description, decimal amount, decimal applicationFeeAmount, Dictionary<string, string> metadata = null)
        {
            var options = new PaymentIntentCreateOptions
            {
                Description = description,
                Amount = (long)(amount * 100),
                ApplicationFeeAmount = (long)(applicationFeeAmount * 100),
                Currency = "usd",
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                Customer = customerId,
                PaymentMethod = paymentMethodId,
                Confirm = true,
                OffSession = true,
                Metadata = metadata,
                TransferData = new PaymentIntentTransferDataOptions()
                {
                    Destination = accountId
                }
            };
            
            var service = new PaymentIntentService();
            return await service.CreateAsync(options);         
        }
    }
}
