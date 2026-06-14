using Kingkode.Chronos.Clock.Cheats;
using Kingkode.Chronos.Clock.Configurations;
using Kingkode.Chronos.Clock.Infrasturctures;
using Kingkode.Chronos.Clock.Overlay;
using Kingkode.Chronos.Clock.Services;
using UnityEngine;

namespace Kingkode.Chronos.Clock
{
    public sealed class ClockBootstrapper : MonoBehaviour
    {
        private bool _isStarted = false;

        private GameClockOptions _clockOptions;
        private ServerTimeSyncOptions _serverTimeSyncOptions;

        private IServerTimeSyncer _serverTimeSyncer;
        private IClock _clock;

        private void Awake()
        {
            var chronos = FindAnyObjectByType<ChronosBootstrapper>();

            _clockOptions = chronos.settings.Clock;
            _serverTimeSyncOptions = chronos.settings.ServerTimeSync;

            chronos.OnRegisterServices.AddListener(services =>
            {
                services.Register(_clockOptions);
                services.Register(_serverTimeSyncOptions);

                services.Register<IServerTimeSyncer, DefaultServerTimeSyncer>();

                services.Register<ITimeSnapshotStorage, DefaultTimeSnapshotStorage>();
                services.Register<IDateTimeProvider, DefaultDateTimeProvider>();
                services.Register<ISystemTickProvider, DefaultSystemTickProvider>();

                services.Register<IClock, ChronosClock>();
            });

            chronos.OnServicesInitialized.AddListener(services =>
            {
                _serverTimeSyncer = services.Resolve<IServerTimeSyncer>();
                _clock = services.Resolve<IClock>();

                _isStarted = true;

                if (_serverTimeSyncOptions.SyncOnAppStart)
                    _serverTimeSyncer.Synce((unix) => _clock.SyncWithServer(unix));
            });

            if (_clockOptions.CheatEnable)
                gameObject.AddComponent<GameClockCheat>();

            if (_clockOptions.OverlayEnable)
                gameObject.AddComponent<GameClockOverlay>();
        }

        //private void Start()
        //{
        //    _isStarted = true;

        //    if (_serverTimeSyncOptions.SyncOnAppStart)
        //        _serverTimeSyncer.Synce((unix) => _clock.SyncWithServer(unix));
        //}

        private void OnApplicationPause(bool pause)
        {
            if (!_isStarted)
                return;

            if (!pause) if (_serverTimeSyncOptions.SyncOnAppFocus)
                _serverTimeSyncer.Synce((unix) => _clock.SyncWithServer(unix));
        }
    }
}