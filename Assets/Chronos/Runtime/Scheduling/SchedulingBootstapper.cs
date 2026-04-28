using Kingkode.Chronos.Clock.Services;
using Kingkode.Chronos.Scheduling.Configurations;
using UnityEngine;

namespace Kingkode.Chronos.Scheduling
{
    public class SchedulingBootstapper : MonoBehaviour
    {
        [SerializeField] ActionSchedulerOptions _options;

        private ActionScheduler _scheduler;
        private IClock _clock;

        private void Awake()
        {
            ChronosBootstrapper.Instance.OnRegisterServices.AddListener(services =>
            {
                services.Register(_options);
                services.Register<ActionScheduler>();
                services.RegisterForward<IActionScheduler, ActionScheduler>();
            });

            ChronosBootstrapper.Instance.OnServicesInitialized.AddListener(services =>
            {
                _clock = services.Resolve<IClock>();
                _scheduler = services.Resolve<ActionScheduler>();

                // Setup default exception handler if enabled
                if (_options.UseDefaultExceptionHandler)
                {
                    var logger = services.Resolve<ILogger>();
                    var defaultHandler = new DefaultExceptionHandler(logger);

                    _options.ExceptionHandler.AddListener(defaultHandler.Handle);
                }
            });
        }

        private void OnEnable()
        {
            
        }

        private void OnDestroy()
        {
            _scheduler?.Dispose();
            _scheduler = null;
        }

        private void Update()
        {
            if (_scheduler == null || _clock == null) 
                return;

            _scheduler.Tick(_clock.UtcNow.ToUnixTimeMilliseconds());
        }
    }
}
