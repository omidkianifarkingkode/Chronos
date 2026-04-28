using Kingkode.Chronos.Clock;
using Kingkode.Chronos.Scheduling;
using Kingkode.Chronos.Ticking;
using UnityEngine;

namespace Kingkode.Chronos
{
    public static class ChronosModuleFactory
    {
        public static ChronosBootstrapper GetOrCreateChronosBootstrapper()
        {
            var bootstrapper = Object.FindFirstObjectByType<ChronosBootstrapper>();
            if (bootstrapper == null)
            {
                var root = new GameObject("Chronos");
                bootstrapper = root.AddComponent<ChronosBootstrapper>();
            }

            EnsureHierarchy(bootstrapper);
            return bootstrapper;
        }

        public static void EnsureHierarchy(ChronosBootstrapper bootstrapper)
        {
            if (bootstrapper == null)
                return;

            EnsureChild<ClockBootstrapper>(bootstrapper.transform, "Clock", true);
            EnsureChild<SchedulingBootstapper>(bootstrapper.transform, "Scheduler", true);
            EnsureChild<TickingBootstrapper>(bootstrapper.transform, "Ticking", false);
        }

        private static T EnsureChild<T>(Transform parent, string name, bool isEnabled) where T : Component
        {
            var child = parent.Find(name);
            GameObject childObject;

            if (child == null)
            {
                childObject = new GameObject(name);
                childObject.transform.SetParent(parent, false);
            }
            else
            {
                childObject = child.gameObject;
            }

            var component = childObject.GetComponent<T>();
            if (component == null)
                component = childObject.AddComponent<T>();

            childObject.SetActive(isEnabled);

            return component;
        }
    }
}
