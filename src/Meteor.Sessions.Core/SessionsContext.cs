using Meteor.Sessions.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Meteor.Sessions.Core;

public class SessionsContext : DbContext
{
    public SessionsContext(DbContextOptions<SessionsContext> options) : base(options)
    {
    }

    public DbSet<Session> Sessions => Set<Session>();
}