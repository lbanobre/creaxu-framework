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
    }
}
