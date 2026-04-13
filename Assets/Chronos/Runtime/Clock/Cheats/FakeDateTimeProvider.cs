using System;
using Kingkode.Chronos.Clock.Services;

namespace Kingkode.Chronos.Clock.Cheats
{
    public sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        private TimeSpan manipulatedTime = TimeSpan.Zero;

        public DateTimeOffset Now => DateTimeOffset.Now.Add(manipulatedTime);

        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow.Add(manipulatedTime);

        public FakeDateTimeProvider() { }
        
        public FakeDateTimeProvider(TimeSpan manipulatedTime)
        {
            this.manipulatedTime = manipulatedTime;
        }

        public void AddSeconds(double value) => manipulatedTime += TimeSpan.FromSeconds(value);
        public void AddMinuts(double value) => manipulatedTime += TimeSpan.FromMinutes(value);
        public void AddHours(double value) => manipulatedTime += TimeSpan.FromHours(value);
        public void AddDays(double value) => manipulatedTime += TimeSpan.FromDays(value);
        public void Reset() => manipulatedTime = TimeSpan.Zero;
    }
}
