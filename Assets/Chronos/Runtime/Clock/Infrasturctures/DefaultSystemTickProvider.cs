using System.Diagnostics;
using Kingkode.Chronos.Clock.Services;

namespace Kingkode.Chronos.Clock.Infrasturctures
{
    public sealed class DefaultSystemTickProvider : ISystemTickProvider
    {
        public long GetTimestamp() => Stopwatch.GetTimestamp();

        public long Frequency => Stopwatch.Frequency;
    }
}
