using Kingkode.Chronos.Clock.Cheats;
using Kingkode.Chronos.Clock.Configurations;
using Kingkode.Chronos.Clock.Infrasturctures;
using Kingkode.Chronos.Clock.Services;
using UnityEngine;

namespace Kingkode.Chronos.Clock
{
    public sealed class ClockBootstrapper : MonoBehaviour
    {
        private bool _isStarted = false;

        [SerializeField] GameClockOptions _clockOptions;
        [SerializeField] ServerTimeSyncOptions _serverTimeSyncOptions;

        private IServerTimeSyncer _serverTimeSyncer;
        private IClock _clock;

        private void Awake()
        {
            ChronosBootstrapper.Instance.OnRegisterServices.AddListener(services =>
            {
                services.Register(_clockOptions);
                services.Register(_serverTimeSyncOptions);

                services.Register<IServerTimeSyncer, DefaultServerTimeSyncer>();

                services.Register<ITimeSnapshotStorage, DefaultTimeSnapshotStorage>();
                services.Register<IDateTimeProvider, DefaultDateTimeProvider>();
                services.Register<ISystemTickProvider, DefaultSystemTickProvider>();

                services.Register<IClock, ChronosClock>();
            });

            ChronosBootstrapper.Instance.OnServicesInitialized.AddListener(services =>
            {
                _serverTimeSyncer = services.Resolve<IServerTimeSyncer>();
                _clock = services.Resolve<IClock>();
                _serverTimeSyncOptions = services.Resolve<ServerTimeSyncOptions>();
                _clockOptions = services.Resolve<GameClockOptions>();
            });

            if (_clockOptions.CheatEnable)
                gameObject.AddComponent<GameClockCheat>();

            if (_clockOptions.OverlayEnable)
                gameObject.AddComponent<GameClockOverlay>();
        }

        private void Start()
        {
            _isStarted = true;

            if (_serverTimeSyncOptions.SyncOnAppStart)
                _serverTimeSyncer.Synce((unix) => _clock.SyncWithServer(unix));
        }

        private void OnApplicationPause(bool pause)
        {
            if (!_isStarted)
                return;

            if (!pause) if (_serverTimeSyncOptions.SyncOnAppFocus)
                _serverTimeSyncer.Synce((unix) => _clock.SyncWithServer(unix));
        }
    }
}