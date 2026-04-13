using System;

namespace Kingkode.Chronos.Clock.Services
{
    public interface IDateTimeProvider 
    {
        DateTimeOffset Now { get; }
        DateTimeOffset UtcNow { get; }
    }
}
