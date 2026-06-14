using Kingkode.Chronos.Clock;
using Kingkode.Chronos.Scheduling;
using Kingkode.Chronos.Ticking;
using UnityEngine;

namespace Kingkode.Chronos
{
    public static class ChronosModuleFactory
    {
        public static ILogger Logger { get; internal set; }

        public static ChronosBootstrapper GetOrCreateChronosBootstrapper()
        {
            var bootstrapper = Object.FindFirstObjectByType<ChronosBootstrapper>();
            if (bootstrapper == null)
            {
                var root = new GameObject("Chronos");
                bootstrapper = root.AddComponent<ChronosBootstrapper>();
            }

            return bootstrapper;
        }

        public static void EnsureHierarchy(ChronosBootstrapper bootstrapper, ChronosSettings settings)
        {
            if (bootstrapper == null)
                return;

            EnsureChild<ClockBootstrapper>(bootstrapper.transform, "Clock");

            EnsureOptionalChild<SchedulingBootstrapper>(bootstrapper.transform, "Scheduler", settings.Scheduler.Enabled);

            EnsureOptionalChild<TickingBootstrapper>(bootstrapper.transform, "Ticking", settings.Ticking.Enabled);
        }


        private static T EnsureChild<T>(Transform parent, string name) where T : Component
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

            return component;
        }

        private static void EnsureOptionalChild<T>(Transform parent, string name, bool enabled) where T : Component
        {
            var child = parent.Find(name);

            if (enabled)
            {
                if (child == null)
                {
                    var go = new GameObject(name);
                    go.transform.SetParent(parent, false);
                    go.AddComponent<T>();
                }
                else if (child.GetComponent<T>() == null)
                {
                    child.gameObject.AddComponent<T>();
                }
            }
            else
            {
                if (child == null)
                    return;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    Object.DestroyImmediate(child.gameObject);
                else
#endif
                    Object.Destroy(child.gameObject);
            }
        }
    }
}