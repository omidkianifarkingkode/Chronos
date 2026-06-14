using Kingkode.Chronos.Clock.Configurations;
using Kingkode.Chronos.Scheduling.Configurations;
using Kingkode.Chronos.Ticking.Configurations;
using UnityEngine;

namespace Kingkode.Chronos
{
    /// <summary>
    /// Single data asset holding every Chronos configuration, so a consumer project can
    /// change the values without touching the package source, scenes, or prefabs.
    ///
    /// Resolution order (see <see cref="Resolve"/>):
    /// 1. The asset explicitly assigned to <see cref="ChronosBootstrapper"/> in the scene.
    /// 2. A "ChronosSettings" asset at the root of any Resources folder in the project.
    /// 3. Built-in package defaults (a transient instance), so Chronos works out of the box.
    ///
    /// Create the asset via "Assets > Create > Chronos > Settings" or
    /// "Window > Chrono > Create Settings Asset".
    /// </summary>
    [CreateAssetMenu(fileName = ResourcesFileName, menuName = "Chronos/Settings")]
    public sealed class ChronosSettings : ScriptableObject
    {
        /// <summary>File name the asset must keep inside a Resources folder to be auto-loaded.</summary>
        public const string ResourcesFileName = "ChronosSettings";

        [field: Header("Logging")]
        [field: SerializeField] public bool LogEnabled { get; set; } = true;
        [field: SerializeField] public LogType LogLevel { get; set; } = LogType.Log;

        [field: Header("Clock")]
        [field: SerializeField] public GameClockOptions Clock { get; set; } = new GameClockOptions();
        [field: SerializeField] public ServerTimeSyncOptions ServerTimeSync { get; set; } = new ServerTimeSyncOptions();
        [field: SerializeField] public ClockOverlayOptions ClockOverlay { get; set; } = new ClockOverlayOptions();
        [field: SerializeField] public ClockCheatOverlayOptions ClockCheatOverlay { get; set; } = new ClockCheatOverlayOptions();

        [field: Header("Scheduling")]
        [field: SerializeField] public ActionSchedulerOptions Scheduler { get; set; } = new ActionSchedulerOptions();

        [field: Header("Ticking")]
        [field: SerializeField] public TickingOptions Ticking { get; set; } = new TickingOptions();

        /// <summary>
        /// Returns the settings to use. Never returns null:
        /// explicit asset > "ChronosSettings" in Resources > package defaults.
        /// </summary>
        internal static ChronosSettings Resolve(ChronosSettings explicitSettings = null)
        {
            if (explicitSettings != null)
                return explicitSettings;

            var fromResources = Resources.Load<ChronosSettings>(ResourcesFileName);
            if (fromResources != null)
                return fromResources;

            var defaults = CreateInstance<ChronosSettings>();
            defaults.name = ResourcesFileName + " (package defaults)";
            return defaults;
        }
    }
}
