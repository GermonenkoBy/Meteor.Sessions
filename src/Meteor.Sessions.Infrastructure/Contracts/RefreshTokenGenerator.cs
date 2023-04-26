using System.Security.Cryptography;
using System.Text;
using Meteor.Sessions.Core.Contracts;

namespace Meteor.Sessions.Infrastructure.Contracts;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    private const int RandomPartBytesLength = 16;

    public string Generate(int customerId, int employeeId)
    {
        var now = DateTime.UtcNow;
        var sb = new StringBuilder();
        sb.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(customerId.ToString())));
        sb.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(employeeId.ToString())));
        sb.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(now.ToBinary().ToString())));
        sb.Append(Convert.ToBase64String(RandomNumberGenerator.GetBytes(RandomPartBytesLength)));
        return sb.ToString();
    }
}