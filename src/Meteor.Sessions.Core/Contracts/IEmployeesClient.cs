using Meteor.Sessions.Core.Models;

namespace Meteor.Sessions.Core.Contracts;

public interface IEmployeesClient
{
    Task<Employee?> GetEmployeeAsync(int customerId, string emailAddress);

    Task<bool> VerifyPasswordAsync(int customerId, int employeeId, string password);
}