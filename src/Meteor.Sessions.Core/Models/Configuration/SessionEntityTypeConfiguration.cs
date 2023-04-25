using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Meteor.Sessions.Core.Models.Configuration;

public class SessionEntityTypeConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.IpAddress).HasMaxLength(45);

        builder.HasIndex(s => new { s.EmployeeId, s.CustomerId, s.IpAddress }).IsUnique();
        builder.HasIndex(s => s.Token).IsUnique();
    }
}