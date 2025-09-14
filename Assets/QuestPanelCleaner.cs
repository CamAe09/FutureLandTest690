using UnityEngine;

namespace TPSBR
{
    /// <summary>
    /// One-time use script to clean up duplicate EnhancedQuestPanel objects
    /// </summary>
    public class QuestPanelCleaner : MonoBehaviour
    {
        [Header("🗑️ Quest Panel Cleaner")]
        [TextArea(3, 5)]
        public string instructions = "RIGHT-CLICK → 'Clean Up All Duplicates'\n\nThis will remove all duplicate EnhancedQuestPanel objects and keep only one.";
        
        [ContextMenu("Clean Up All Duplicates")]
        public void CleanUpAllDuplicates()
        {
            Debug.Log("🗑️ Cleaning up all duplicate quest panels...");
            
            // Find all objects with EnhancedQuestPanel name
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int duplicateCount = 0;
            GameObject keepPanel = null;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "EnhancedQuestPanel")
                {
                    if (keepPanel == null)
                    {
                        // Keep the first one we find
                        keepPanel = obj;
                        Debug.Log($"✅ Keeping quest panel: {obj.name} at {GetHierarchyPath(obj)}");
                    }
                    else
                    {
                        // Destroy duplicates
                        Debug.Log($"🗑️ Destroying duplicate: {obj.name} at {GetHierarchyPath(obj)}");
                        DestroyImmediate(obj);
                        duplicateCount++;
                    }
                }
            }
            
            Debug.Log($"🎉 Cleanup complete! Removed {duplicateCount} duplicate panels.");
            
            if (keepPanel != null)
            {
                // Make sure the remaining panel is properly configured
                Canvas canvas = keepPanel.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = 1000;
                    Debug.Log("✅ Fixed Canvas render mode on remaining panel");
                }
                
                keepPanel.SetActive(false); // Start hidden
                Debug.Log("✅ Panel set to start hidden");
            }
            
            Debug.Log("💡 Your quest button should now work without creating duplicates!");
        }
        
        private string GetHierarchyPath(GameObject obj)
        {
            string path = obj.name;
            Transform parent = obj.transform.parent;
            
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            
            return "/" + path;
        }
        
        [ContextMenu("Count Quest Panels")]
        public void CountQuestPanels()
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int count = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "EnhancedQuestPanel")
                {
                    count++;
                    Debug.Log($"Found quest panel #{count}: {GetHierarchyPath(obj)}");
                }
            }
            
            Debug.Log($"📊 Total EnhancedQuestPanel objects found: {count}");
        }
    }
}