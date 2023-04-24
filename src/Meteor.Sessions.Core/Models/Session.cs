using System.ComponentModel.DataAnnotations;
using Meteor.Sessions.Core.Models.Annotations;

namespace Meteor.Sessions.Core.Models;

public class Session
{
    public Guid Id { get; set; }

    public int EmployeeId { get; set; }

    public int CustomerId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Token is required.")]
    [StringLength(400, ErrorMessage = "Token max length is 400.")]
    public string Token { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Device name is required.")]
    [StringLength(100, ErrorMessage = "Max device name length is 100.")]
    public string DeviceName { get; set; } = string.Empty;

    [IpAddress(ErrorMessage = "Must be a valid IP address.")]
    public string IpAddress { get; set; } = string.Empty;

    public DateTimeOffset CreateDate { get; set; }

    public DateTimeOffset LastRefreshDate { get; set; }

    public DateTimeOffset ExpireDate { get; set; }
}