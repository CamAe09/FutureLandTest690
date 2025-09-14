using UnityEngine;
using UnityEngine.UI;

namespace TPSBR
{
    /// <summary>
    /// Simple fix for your existing quest system - no new QuestManager needed
    /// </summary>
    public class SimpleQuestFix : MonoBehaviour
    {
        [Header("🎯 Simple Quest Fix")]
        [TextArea(3, 5)]
        public string instructions = "RIGHT-CLICK → 'Connect Quest System'\n\nUses your existing QuestManager and QuestUISetup!";
        
        [ContextMenu("Connect Quest System")]
        public void ConnectQuestSystem()
        {
            Debug.Log("🔧 Connecting quest system using your existing components...");
            
            // Step 1: Your QuestManager already exists - just verify
            if (QuestManager.Instance == null)
            {
                Debug.LogWarning("⚠️ QuestManager not found - it should be in the scene");
            }
            else
            {
                Debug.Log("✅ Your existing QuestManager found");
            }
            
            // Step 2: Use your existing QuestUISetup to create the panel
            QuestUISetup questSetup = FindObjectOfType<QuestUISetup>();
            if (questSetup == null)
            {
                // Add to MenuUI if needed
                GameObject menuUI = GameObject.Find("MenuUI");
                if (menuUI != null)
                {
                    questSetup = menuUI.AddComponent<QuestUISetup>();
                    Debug.Log("✅ Added QuestUISetup to MenuUI");
                }
            }
            
            if (questSetup != null)
            {
                questSetup.CreateEnhancedQuestSystem();
                Debug.Log("✅ Created quest panel using QuestUISetup");
            }
            
            // Step 3: Connect your QuestButton properly
            ConnectQuestButton();
            
            Debug.Log("🎉 Quest system connected!");
            Debug.Log("💡 Click your QUEST button to test!");
        }
        
        private void ConnectQuestButton()
        {
            GameObject questButton = GameObject.Find("QuestButton");
            if (questButton == null)
            {
                Debug.LogWarning("⚠️ QuestButton not found");
                return;
            }
            
            // Clean up any existing handlers
            SimpleQuestButtonHandler oldHandler = questButton.GetComponent<SimpleQuestButtonHandler>();
            if (oldHandler != null)
            {
                DestroyImmediate(oldHandler);
                Debug.Log("🗑️ Removed old SimpleQuestButtonHandler");
            }
            
            // Add Unity Button if missing
            Button button = questButton.GetComponent<Button>();
            if (button == null)
            {
                button = questButton.AddComponent<Button>();
                Debug.Log("✅ Added Unity Button to QuestButton");
            }
            
            // Connect to toggle method
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ToggleQuestPanel);
            
            Debug.Log("✅ Connected QuestButton");
        }
        
        private void ToggleQuestPanel()
        {
            // Look for any quest panels
            GameObject panel = null;
            
            string[] panelNames = { "EnhancedQuestPanel", "QuestPanel", "WorkingQuestPanel" };
            foreach (string name in panelNames)
            {
                panel = GameObject.Find(name);
                if (panel != null) break;
            }
            
            if (panel != null)
            {
                bool isVisible = panel.activeSelf;
                panel.SetActive(!isVisible);
                Debug.Log($"🎯 Quest panel {(panel.activeSelf ? "opened" : "closed")}!");
            }
            else
            {
                Debug.LogWarning("⚠️ No quest panel found - run 'Connect Quest System' first");
            }
        }
        
        [ContextMenu("Test Quest Button")]
        public void TestQuestButton()
        {
            ToggleQuestPanel();
        }
    }
}