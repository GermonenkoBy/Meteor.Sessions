namespace Meteor.Sessions.Core.Options;

public record SessionsOptions
{
    private int _defaultDurationMinutes;

    private int _extendDurationMinutes;

    public TimeSpan DefaultDuration { get; private set; }

    public TimeSpan ExtendDuration { get; private set; }

    public int DefaultDurationMinutes
    {
        get => _defaultDurationMinutes;
        set
        {
            _defaultDurationMinutes = value;
            DefaultDuration = TimeSpan.FromMinutes(value);
        }
    }

    public int ExtendDurationMinutes
    {
        get => _extendDurationMinutes;
        set
        {
            _extendDurationMinutes = value;
            ExtendDuration = TimeSpan.FromMinutes(value);
        }
    }
}