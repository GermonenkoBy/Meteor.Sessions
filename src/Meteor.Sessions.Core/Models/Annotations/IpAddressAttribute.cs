using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Meteor.Sessions.Core.Models.Annotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IpAddressAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string str)
        {
            // Should be handled by Required attribute
            return true;
        }

        return IPAddress.TryParse(str, out _);
    }
}