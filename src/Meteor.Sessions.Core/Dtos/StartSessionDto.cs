namespace Meteor.Sessions.Core.Dtos;

public record struct StartSessionDto
{
    public string Username;
    public string Password;
    public string Domain;
    public string IpAddress;
    public string DeviceName;
}