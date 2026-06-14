using Kingkode.Chronos.Clock.Services;
using Kingkode.Chronos.Scheduling;
using Kingkode.Chronos.Ticking;
using Kingkode.Chronos.Ticking.Services;
using MiniDI;
using UnityEngine;
using UnityEngine.Events;

namespace Kingkode.Chronos
{
    [DefaultExecutionOrder(-10000)]
    public sealed class ChronosBootstrapper : MonoBehaviour
    {
        internal static ChronosBootstrapper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<ChronosBootstrapper>();

                if (_instance == null)
                    _instance = ChronosModuleFactory.GetOrCreateChronosBootstrapper();

                return _instance;
            }
        }
        private static ChronosBootstrapper _instance;

        [field: SerializeField] public UnityEvent<IServiceRegister> OnRegisterServices { get; private set; }
        [field: SerializeField] public UnityEvent<IServiceResolver> OnServicesInitialized { get; private set; }

        /// <summary>
        /// Resolved configuration: explicit asset > "ChronosSettings" in Resources > package defaults.
        /// Never null.
        /// </summary>
        internal ChronosSettings settings
        {
            get
            {
                if (_settings == null)
                    _settings = ChronosSettings.Resolve();

                return _settings;
            }
        }
        private ChronosSettings _settings;

        private ServiceContainer _container;
        private ILogger _logger;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            _logger = new Logger(Debug.unityLogger.logHandler)
            {
                logEnabled = settings.LogEnabled,
                filterLogType = settings.LogLevel
            };

            ChronosModuleFactory.Logger = _logger;
            ChronosModuleFactory.EnsureHierarchy(this, settings);
        }

        private void Start()
        {
            BuildContainer();
        }

#if UNITY_EDITOR
        //private void OnValidate()
        //{
        //    _settings = null;

        //    if (Application.isPlaying)
        //    {
        //        if (_logger != null)
        //        {
        //            _logger.logEnabled = settings.LogEnabled;
        //            _logger.filterLogType = settings.LogLevel;
        //        }

        //        return;
        //    }

        //    ChronosModuleFactory.EnsureHierarchy(this, settings);
        //}
#endif

        private void BuildContainer()
        {
            // The new Microsoft-style Builder pattern
            _container = ServiceContainer.Create("Chronos DI", _logger)

                // 1. REGISTRATION PHASE
                .ConfigureServices(services =>
                {
                    services.Register(_logger);
                    services.Register(settings);

                    // Allow external modules to register their dependencies
                    OnRegisterServices?.Invoke(services);
                })

                // 2. INITIALIZATION PHASE
                .OnInitialize(resolver =>
                {
                    // Map resolved services to your static facades
                    if (resolver.TryResolve<IClock>(out var clock))
                        Chronos.Clock = clock;

                    if (resolver.TryResolve<IActionScheduler>(out var scheduler))
                        Chronos.Scheduler = scheduler;

                    if (resolver.TryResolve<ITickProvider>(out var tickProvider))
                        Chronos.TickProvider = tickProvider;

                    // Notify external modules that services are ready to be used
                    OnServicesInitialized?.Invoke(resolver);
                })

                // 3. FINALIZE
                .Build();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _container?.Dispose();
                _logger = null;
                _instance = null;
            }
        }
    }
}