using Kingkode.Chronos.Clock.Configurations;
using Kingkode.Chronos.Clock.Infrasturctures;
using Kingkode.Chronos.Clock.Services;
using UnityEngine;

namespace Kingkode.Chronos.Clock.Cheats
{
    public class GameClockCheat : MonoBehaviour
    {
        private FakeDateTimeProvider serverDateTimeProvider = new();
        private DefaultDateTimeProvider realDateTimeProvider = new();
        private DefaultSystemTickProvider realSystemTickProvider = new();

        private FakeDateTimeProvider fakeLocalDateTimeProvider = new();
        private FakeSystemTickProvider fakeSystemTickProvider = new();

        private ITimeSnapshotStorage timeSnapshotStorage;
        private IClock clock;

        private CheatStorage storage = new();

        private void Awake()
        {
            serverDateTimeProvider.AddMinuts(storage.ServerDateTime.Value);
            fakeLocalDateTimeProvider.AddHours(storage.LocalDateTime.Value);
            fakeSystemTickProvider.AddTicks(storage.SystemTick.Value);

            gameObject.AddComponent<CheatGameClockDebugOverlay>();

            ChronosBootstrapper.Instance.OnRegisterServices.AddListener((services) =>
            {
                services.Register<IDateTimeProvider>(fakeLocalDateTimeProvider);
                services.Register<ISystemTickProvider>(fakeSystemTickProvider);
                services.Register(new ServerTimeSyncOptions
                {
                    SyncOnAppFocus = false,
                    SyncOnAppStart = false,
                });

                services.Register(this);
                services.Register(serverDateTimeProvider);
                services.Register(realDateTimeProvider);
                services.Register(realSystemTickProvider);
            });

            ChronosBootstrapper.Instance.OnServicesInitialized.AddListener((services) =>
            {
                timeSnapshotStorage = services.Resolve<ITimeSnapshotStorage>();
                clock = services.Resolve<IClock>();
            });
        }

        public void ClearCache()
        {
            storage.ServerDateTime = null;
            storage.LocalDateTime = null;
            storage.SystemTick = null;

            timeSnapshotStorage.Clear();
        }

        public void SynceWithServer()
        {
            clock.SyncWithServer(serverDateTimeProvider.UtcNow.ToUnixTimeMilliseconds());
        }

        public void CheatServerDateTime(int m)
        {
            serverDateTimeProvider.AddMinuts(m);

            var loadedHour = storage.ServerDateTime.Value;
            loadedHour += m;

            storage.ServerDateTime = loadedHour;
        }

        public void ResetServerDateTime()
        {
            serverDateTimeProvider.Reset();

            storage.ServerDateTime = null;
        }

        public void CheatLocalDateTime(int h)
        {
            fakeLocalDateTimeProvider.AddHours(h);

            var loadedHour = storage.LocalDateTime.Value;
            loadedHour += h;

            storage.LocalDateTime = loadedHour;
        }

        public void ResetLocalDateTime()
        {
            fakeLocalDateTimeProvider.Reset();

            storage.LocalDateTime = null;
        }

        public void CheatSystemTick(long t)
        {
            fakeSystemTickProvider.AddTicks(t);

            var loadedTicks = storage.SystemTick.Value;
            loadedTicks += t;

            storage.SystemTick = loadedTicks;
        }

        public void ResetSystemTick()
        {
            fakeSystemTickProvider.Reset();

            storage.SystemTick = null;
        }
    }
}
