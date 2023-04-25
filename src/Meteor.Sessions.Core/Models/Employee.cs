namespace Meteor.Sessions.Core.Models;

public class Employee
{
    public int Id { get; set; }

    public string EmailAddress { get; set; } = string.Empty;

    public bool Active { get; set; }
}