using UnityEditor;
using UnityEngine;

namespace Kingkode.Chronos.Editor
{
    public static class ChronosModuleMenu
    {
        private const string MenuPath = "Window/Chrono/Setup Chronos";
        private const string SettingsMenuPath = "Window/Chrono/Create Settings Asset";

        [MenuItem(MenuPath)]
        private static void CreateChronosBootstrapperHierarchy()
        {
            var bootstrapper = ChronosModuleFactory.GetOrCreateChronosBootstrapper();

            Selection.activeGameObject = bootstrapper.gameObject;
            EditorGUIUtility.PingObject(bootstrapper.gameObject);
        }

        /// <summary>
        /// Creates the project-side ChronosSettings asset in Assets/Resources (or pings the
        /// existing one), so consumers configure Chronos without touching the package.
        /// </summary>
        [MenuItem(SettingsMenuPath)]
        private static void CreateOrSelectSettingsAsset()
        {
            var existing = FindSettingsAsset();
            if (existing != null)
            {
                Selection.activeObject = existing;
                EditorGUIUtility.PingObject(existing);
                return;
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            var settings = ScriptableObject.CreateInstance<ChronosSettings>();
            var path = "Assets/Resources/" + ChronosSettings.ResourcesFileName + ".asset";
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();

            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
        }

        private static ChronosSettings FindSettingsAsset()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:" + nameof(ChronosSettings)))
            {
                var asset = AssetDatabase.LoadAssetAtPath<ChronosSettings>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset != null)
                    return asset;
            }

            return null;
        }
    }
}
