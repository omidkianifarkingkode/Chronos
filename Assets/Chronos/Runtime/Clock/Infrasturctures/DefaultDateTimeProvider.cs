using System;
using Kingkode.Chronos.Clock.Services;

namespace Kingkode.Chronos.Clock.Infrasturctures
{
    public sealed class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now => DateTimeOffset.Now;

        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
