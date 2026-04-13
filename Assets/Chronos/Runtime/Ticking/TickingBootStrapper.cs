using Kingkode.Chronos.Ticking.Configurations;
using Kingkode.Chronos.Ticking.Infrasturctures;
using Kingkode.Chronos.Ticking.Services;
using UnityEngine;

namespace Kingkode.Chronos.Ticking
{
    public class TickingBootStrapper : MonoBehaviour 
    {
        [SerializeField] TickingOptions _options;
        private TickProvider _tickSystem;

        private void Awake()
        {
            ChronosBootstrapper.Instance.OnRegisterServices.AddListener((services) =>
            {
                services.Register(_options);
                services.Register<TickProvider>();
                services.RegisterForward<ITickProvider, TickProvider>();
            });

            ChronosBootstrapper.Instance.OnServicesInitialized.AddListener((services) =>
            {
                _tickSystem = services.Resolve<TickProvider>();
            });
        }

        private void Update()
        {
            _tickSystem.Tick();
        }
    }
}
