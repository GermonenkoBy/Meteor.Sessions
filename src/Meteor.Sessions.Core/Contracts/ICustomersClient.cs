using Meteor.Sessions.Core.Models;

namespace Meteor.Sessions.Core.Contracts;

public interface ICustomersClient
{
    Task<Customer?> GetCustomerAsync(string domain);

    Task<CustomerSettings?> GetCustomerSettingsAsync(int customerId);
}