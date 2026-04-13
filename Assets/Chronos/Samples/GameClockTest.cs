using Kingkode.Chronos.Clock.Cheats;
using Kingkode.Chronos.Clock.Configurations;
using Kingkode.Chronos.Clock.Infrasturctures;
using Kingkode.Chronos.Clock.Services;
using UnityEngine;

namespace Assets.Chronos.Samples
{
    public class GameClockTest : MonoBehaviour
    {
        public ChronosClock clock;

        public DefaultDateTimeProvider dateTimeProvider = new();
        public DefaultSystemTickProvider systemTickProvider = new();

        public ITimeSnapshotStorage storage = new DefaultTimeSnapshotStorage();

        public FakeDateTimeProvider fakeDateTimeProvider = new();
        public FakeSystemTickProvider fakeSystemTickProvider = new();

        private void Awake()
        {
            fakeDateTimeProvider.AddHours(PlayerPrefs.GetInt("h", 0));
            long.TryParse(PlayerPrefs.GetString("t", "0"), out var loadedTicks);
            fakeSystemTickProvider.AddTicks(loadedTicks);

            clock = new ChronosClock(storage, fakeDateTimeProvider, fakeSystemTickProvider, UnityEngine.Debug.unityLogger, new GameClockOptions());
        }

        [ContextMenu("Clear Cache")]
        public void ClearCache()
        {
            PlayerPrefs.DeleteAll();
        }

        public void CheatDateTime(int h)
        {
            fakeDateTimeProvider.AddHours(h);

            var loadedHour = PlayerPrefs.GetInt("h", 0);
            loadedHour += h;

            PlayerPrefs.SetInt("h", loadedHour);
            PlayerPrefs.Save();
        }

        public void ResetDateTime()
        {
            fakeDateTimeProvider.Reset();

            PlayerPrefs.SetInt("h", 0);
            PlayerPrefs.Save();
        }

        public void CheatSystemTick(long t)
        {
            fakeSystemTickProvider.AddTicks(t);

            long.TryParse(PlayerPrefs.GetString("t", "0"), out var loadedTicks);
            loadedTicks += t;

            PlayerPrefs.SetString("t", loadedTicks.ToString());
            PlayerPrefs.Save();
        }

        public void ResetSystemTick()
        {
            fakeSystemTickProvider.Reset();

            PlayerPrefs.SetString("t", "0");
            PlayerPrefs.Save();
        }
    }
}
