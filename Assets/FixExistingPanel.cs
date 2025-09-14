using UnityEngine;
using UnityEngine.UI;

namespace TPSBR
{
    /// <summary>
    /// Fixes the existing WorkingQuestPanel that already exists but isn't visible
    /// </summary>
    public class FixExistingPanel : MonoBehaviour
    {
        [ContextMenu("Fix Existing WorkingQuestPanel")]
        public void FixExistingWorkingQuestPanel()
        {
            GameObject panel = GameObject.Find("WorkingQuestPanel");
            if (panel == null)
            {
                Debug.LogError("❌ WorkingQuestPanel not found!");
                return;
            }
            
            Debug.Log("🔧 Fixing existing WorkingQuestPanel...");
            
            // The problem: Canvas is set to WorldSpace - needs to be part of UI
            Canvas canvas = panel.GetComponent<Canvas>();
            if (canvas != null)
            {
                // Remove the Canvas component that's causing issues
                DestroyImmediate(canvas);
                Debug.Log("🗑️ Removed problematic WorldSpace Canvas");
            }
            
            // Remove GraphicRaycaster too
            GraphicRaycaster raycaster = panel.GetComponent<GraphicRaycaster>();
            if (raycaster != null)
            {
                DestroyImmediate(raycaster);
                Debug.Log("🗑️ Removed GraphicRaycaster");
            }
            
            // Now it will use the parent MenuUI Canvas properly
            panel.SetActive(true);
            
            Debug.Log("✅ Fixed WorkingQuestPanel - should now be visible!");
            Debug.Log("🎯 Click your Quest button to test it!");
        }
    }
}