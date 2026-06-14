using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kingkode.Chronos.Clock.Overlay
{
    /// <summary>
    /// Keeps an invisible screen-space canvas with one raycast-target image per overlay
    /// region, so clicks on the IMGUI overlays count as "pointer over UI" for the host
    /// project (EventSystem.IsPointerOverGameObject and uGUI raycasts) instead of
    /// falling through to gameplay input. IMGUI itself is invisible to the EventSystem,
    /// which is why the overlays must mirror their rects here.
    /// </summary>
    internal static class ChronosOverlayRaycastBlocker
    {
        private static Canvas _canvas;
        private static readonly Dictionary<string, RectTransform> _regions = new Dictionary<string, RectTransform>();

        /// <summary>
        /// Creates or moves a blocking region. The rect is in OnGUI screen coordinates
        /// (top-left origin, pixels), which map 1:1 onto an unscaled overlay canvas.
        /// </summary>
        public static void SetRegion(string id, Rect guiRect)
        {
            EnsureCanvas();

            if (!_regions.TryGetValue(id, out var region) || region == null)
            {
                var go = new GameObject("Region_" + id, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                go.transform.SetParent(_canvas.transform, false);

                var image = go.GetComponent<Image>();
                image.color = Color.clear; // invisible, but still a raycast target

                region = (RectTransform)go.transform;
                region.anchorMin = region.anchorMax = new Vector2(0f, 1f);
                region.pivot = new Vector2(0f, 1f);

                _regions[id] = region;
            }

            if (!region.gameObject.activeSelf)
                region.gameObject.SetActive(true);

            region.anchoredPosition = new Vector2(guiRect.x, -guiRect.y);
            region.sizeDelta = new Vector2(guiRect.width, guiRect.height);
        }

        public static void RemoveRegion(string id)
        {
            if (_regions.TryGetValue(id, out var region) && region != null)
                region.gameObject.SetActive(false);
        }

        private static void EnsureCanvas()
        {
            if (_canvas != null)
                return;

            var go = new GameObject("[Chronos] Overlay Raycast Blocker", typeof(Canvas), typeof(GraphicRaycaster));
            Object.DontDestroyOnLoad(go);

            _canvas = go.GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            // Sit above the project's UI so the blocker wins the raycast where the
            // overlays draw (OnGUI itself always renders on top of uGUI anyway).
            _canvas.sortingOrder = 32000;
        }
    }
}
