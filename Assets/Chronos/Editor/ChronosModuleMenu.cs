using UnityEditor;
using UnityEngine;

namespace Kingkode.Chronos.Editor
{
    public static class ChronosModuleMenu
    {
        private const string MenuPath = "Window/Chrono/Setup Chronos";

        [MenuItem(MenuPath)]
        private static void CreateChronosBootstrapperHierarchy()
        {
            var bootstrapper = ChronosModuleFactory.GetOrCreateChronosBootstrapper();

            Selection.activeGameObject = bootstrapper.gameObject;
            EditorGUIUtility.PingObject(bootstrapper.gameObject);
        }
    }
}
