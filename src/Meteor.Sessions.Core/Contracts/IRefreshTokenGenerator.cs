namespace Meteor.Sessions.Core.Contracts;

public interface IRefreshTokenGenerator
{
    public string Generate(int customerId, int employeeId);
}