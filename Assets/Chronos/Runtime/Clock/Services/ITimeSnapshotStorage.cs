using Kingkode.Chronos.Clock.Persistences;

namespace Kingkode.Chronos.Clock.Services
{
    public interface ITimeSnapshotStorage
    {
        void Save(GameClockSnapshot snapshot);
        bool TryLoad(out GameClockSnapshot snapshot);
        void Clear();
    }
}
