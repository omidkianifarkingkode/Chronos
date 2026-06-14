using Kingkode.Chronos.Ticking.Configurations;
using Kingkode.Chronos.Ticking.Infrasturctures;
using Kingkode.Chronos.Ticking.Services;
using UnityEngine;

namespace Kingkode.Chronos.Ticking
{
    public class TickingBootstrapper : MonoBehaviour 
    {
        private TickingOptions _options;
        private TickProvider _tickSystem;

        private void Awake()
        {
            var chronos = FindAnyObjectByType<ChronosBootstrapper>();
            // Configuration comes from the ChronosSettings asset; nothing is serialized
            // on this component, so consumers configure the module without editing the package.
            _options = chronos.settings.Ticking;

            chronos.OnRegisterServices.AddListener((services) =>
            {
                services.Register(_options);
                services.Register<TickProvider>();
                services.RegisterForward<ITickProvider, TickProvider>();
            });

            chronos.OnServicesInitialized.AddListener((services) =>
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
