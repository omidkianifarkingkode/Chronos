using Kingkode.Chronos.Clock.Services;
using Kingkode.Chronos.Scheduling;
using Kingkode.Chronos.Ticking;
using Kingkode.Chronos.Ticking.Services;
using MiniDI;
using UnityEngine;
using UnityEngine.Events;

namespace Kingkode.Chronos
{
    public sealed class ChronosBootstrapper : MonoBehaviour
    {
        internal static ChronosBootstrapper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<ChronosBootstrapper>();

                if (_instance == null)
                    _instance = new GameObject(nameof(ChronosBootstrapper)).AddComponent<ChronosBootstrapper>();

                return _instance;
            }
        }
        private static ChronosBootstrapper _instance;

        [Header("Logging")]
        [SerializeField] bool _logEnabled = true;
        [SerializeField] private LogType _logLevel = LogType.Log;

        [Header("Modules")]
        [SerializeField] bool _schedulerEnable;
        [SerializeField] bool _tickingEnable;

        [field: SerializeField] public UnityEvent<IServiceRegister> OnRegisterServices { get; private set; }
        [field: SerializeField] public UnityEvent<IServiceResolver> OnServicesInitialized { get; private set; }

        private ServiceContainer _container;
        private ILogger _chronosLogger;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeModules();
            BuildContainer();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                if (_chronosLogger != null)
                {
                    _chronosLogger.logEnabled = _logEnabled;
                    _chronosLogger.filterLogType = _logLevel;
                }

                return;
            }

            InitializeModules();
        }
#endif

        private void InitializeModules()
        {
            GetComponentInChildren<SchedulingBootstapper>(true).gameObject.SetActive(_schedulerEnable);
            GetComponentInChildren<TickingBootStrapper>(true).gameObject.SetActive(_tickingEnable);
        }

        private void BuildContainer()
        {
            _chronosLogger = new Logger(Debug.unityLogger.logHandler)
            {
                logEnabled = _logEnabled,
                filterLogType = _logLevel
            };

            // The new Microsoft-style Builder pattern
            _container = ServiceContainer.Create("Chronos DI", _chronosLogger)

                // 1. REGISTRATION PHASE
                .ConfigureServices(services =>
                {
                    services.Register<ILogger>(_chronosLogger);

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
                _chronosLogger = null;
                _container?.Dispose();
                _instance = null;
            }
        }
    }
}