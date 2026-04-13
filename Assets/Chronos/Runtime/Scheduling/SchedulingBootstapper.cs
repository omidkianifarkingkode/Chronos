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
            ChronosBootstrapper.Instance.OnRegisterServices.AddListener(serivces =>
            {
                serivces.Register(_options);
                serivces.Register<ActionScheduler>();
                serivces.RegisterForward<IActionScheduler, ActionScheduler>();
            });

            ChronosBootstrapper.Instance.OnServicesInitialized.AddListener(services =>
            {
                _clock = services.Resolve<IClock>();
                _scheduler = services.Resolve<ActionScheduler>();
            });
        }

        private void Update()
        {
            _scheduler.Tick(_clock.UtcNow.ToUnixTimeMilliseconds());
        }
    }
}
