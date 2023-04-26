using Meteor.Common.Core.Exceptions;
using Meteor.Sessions.Core.Contracts;
using Meteor.Sessions.Core.Dtos;
using Meteor.Sessions.Core.Models;
using Meteor.Sessions.Core.Options;
using Meteor.Sessions.Core.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Meteor.Sessions.Core.Services;

public class SessionsService : ISessionsService
{
    private readonly ICustomersClient _customersClient;

    private readonly IEmployeesClient _employeesClient;

    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    private readonly IOptionsSnapshot<SessionsOptions> _sessionsOptions;

    private readonly SessionsContext _context;

    public SessionsService(
        ICustomersClient customersClient,
        IEmployeesClient employeesClient,
        IRefreshTokenGenerator refreshTokenGenerator,
        IOptionsSnapshot<SessionsOptions> sessionsOptions,
        SessionsContext context
    )
    {
        _customersClient = customersClient;
        _employeesClient = employeesClient;
        _refreshTokenGenerator = refreshTokenGenerator;
        _sessionsOptions = sessionsOptions;
        _context = context;
    }

    public async Task<Session> StartSessionAsync(StartSessionDto sessionDto)
    {
        var customer = await _customersClient.GetCustomerAsync(sessionDto.Domain);
        if (customer is null)
        {
            throw new MeteorException("Organization with the provided domain was not found.");
        }

        if (!customer.Active)
        {
            throw new MeteorException("Organization account is inactive.");
        }

        var employee = await _employeesClient.GetEmployeeAsync(customer.Id, sessionDto.Username);
        if (employee is null)
        {
            throw new MeteorException("Login and/or password are invalid.");
        }

        if (!employee.Active)
        {
            throw new MeteorException("User account is inactive.");
        }

        var valid = await _employeesClient.VerifyPasswordAsync(customer.Id, employee.Id, sessionDto.Password);
        if (!valid)
        {
            throw new MeteorException("Login and/or password are invalid.");
        }

        var session = await _context.Sessions.FirstOrDefaultAsync(
            s => s.CustomerId == customer.Id
                 && s.EmployeeId == employee.Id
                 && s.IpAddress == sessionDto.IpAddress
        );

        var now = DateTimeOffset.UtcNow;
        if (session is null)
        {
            session = new()
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                EmployeeId = employee.Id,
                IpAddress = sessionDto.IpAddress,
                CreateDate = now,
            };
            _context.Sessions.Add(session);
        }

        session.Token = _refreshTokenGenerator.Generate(customer.Id, employee.Id);
        session.DeviceName = sessionDto.DeviceName;
        session.LastRefreshDate = now;
        session.ExpireDate = now.Add(_sessionsOptions.Value.DefaultDuration);

        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<Session> RefreshTokenAsync(Guid id)
    {
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == id);
        if (session is null)
        {
            throw new MeteorNotFoundException("Session not found.");
        }

        return await RefreshTokenAsync(session);
    }

    public async Task<Session> RefreshTokenAsync(string token)
    {
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token);
        if (session is null)
        {
            throw new MeteorNotFoundException("Token is invalid or a relation session is expired.");
        }

        return await RefreshTokenAsync(session);
    }

    private async Task<Session> RefreshTokenAsync(Session session)
    {
        if (session.ExpireDate < DateTimeOffset.UtcNow)
        {
            throw new MeteorException("Session is expired.");
        }

        var customer = await _customersClient.GetCustomerAsync(session.CustomerId);
        if (customer is null)
        {
            throw new MeteorException("Organization with the provided domain was not found.");
        }

        if (!customer.Active)
        {
            throw new MeteorException("Organization account is inactive.");
        }

        var employee = await _employeesClient.GetEmployeeAsync(customer.Id, session.EmployeeId);
        if (employee is null)
        {
            throw new MeteorException("Login and/or password are invalid.");
        }

        if (!employee.Active)
        {
            throw new MeteorException("User account is inactive.");
        }

        var now = DateTimeOffset.UtcNow;
        var sessionIsAboutToExpire = session.ExpireDate - now < _sessionsOptions.Value.ExtendDuration;

        // Slightly extend session so user is not logged off surprisingly
        if (sessionIsAboutToExpire)
        {
            session.ExpireDate = now.Add(_sessionsOptions.Value.ExtendDuration);
        }

        session.Token = _refreshTokenGenerator.Generate(customer.Id, employee.Id);

        await _context.SaveChangesAsync();
        return session;
    }

    public async Task TerminateSessionAsync(string token)
    {
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token);
        if (session is not null)
        {
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
        }
    }

    public async Task TerminateSessionAsync(Guid id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session is not null)
        {
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
        }
    }
}