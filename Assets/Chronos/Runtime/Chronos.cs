using Kingkode.Chronos.Clock.Services;
using Kingkode.Chronos.Scheduling;
using Kingkode.Chronos.Ticking.Services;

namespace Kingkode.Chronos
{
    public class Chronos
    {
        public static IClock Clock { get; internal set; }
        public static IActionScheduler Scheduler { get; internal set; }
        public static ITickProvider TickProvider { get; internal set; }
    }
}