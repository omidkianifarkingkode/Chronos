using System.Diagnostics;
using Kingkode.Chronos.Clock.Services;

namespace Kingkode.Chronos.Clock.Cheats
{
    public sealed class FakeSystemTickProvider : ISystemTickProvider
    {
        private long manipulatedTicks;

        public long GetTimestamp() => Stopwatch.GetTimestamp() + manipulatedTicks;

        public long Frequency => Stopwatch.Frequency;


        public FakeSystemTickProvider() { }

        public FakeSystemTickProvider(long manipulatedTicks)
        {
            this.manipulatedTicks = manipulatedTicks;
        }

        public void AddTicks(long value) => manipulatedTicks += value;

        public void Reset() => manipulatedTicks = 0;
    }
}
