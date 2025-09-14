using UnityEngine;

namespace TPSBR
{
    /// <summary>
    /// Connects your existing QuestButton to your existing QuestManager system
    /// </summary>
    public class ConnectExistingQuests : MonoBehaviour
    {
        [Header("🔧 Connect Existing Quest System")]
        [TextArea(3, 5)]
        public string instructions = "RIGHT-CLICK → 'Connect Quest System'\n\nThis connects your existing QuestButton to your existing QuestManager using QuestUISetup.";
        
        [ContextMenu("Connect Quest System")]
        public void ConnectQuestSystem()
        {
            Debug.Log("🔧 Connecting existing quest system...");
            
            // Step 1: Verify your QuestManager exists
            if (QuestManager.Instance == null)
            {
                Debug.LogError("❌ QuestManager not found! Make sure QuestManager GameObject is active.");
                return;
            }
            
            Debug.Log("✅ Found existing QuestManager");
            
            // Step 2: Check if we have QuestUISetup
            QuestUISetup questSetup = FindObjectOfType<QuestUISetup>();
            if (questSetup == null)
            {
                // Add QuestUISetup to MenuUI 
                GameObject menuUI = GameObject.Find("MenuUI");
                if (menuUI != null)
                {
                    questSetup = menuUI.AddComponent<QuestUISetup>();
                    Debug.Log("✅ Added QuestUISetup to MenuUI");
                }
                else
                {
                    Debug.LogError("❌ MenuUI not found!");
                    return;
                }
            }
            
            // Step 3: Create the quest panel using your existing setup
            questSetup.CreateEnhancedQuestSystem();
            
            // Step 4: Verify the button connection
            GameObject questButton = GameObject.Find("QuestButton");
            if (questButton != null)
            {
                SimpleQuestButtonHandler handler = questButton.GetComponent<SimpleQuestButtonHandler>();
                if (handler != null)
                {
                    handler.debugMode = true;
                    Debug.Log("✅ QuestButton handler is ready");
                }
                else
                {
                    Debug.LogWarning("⚠️ SimpleQuestButtonHandler not found on QuestButton");
                }
            }
            
            Debug.Log("🎉 Quest system connected!");
            Debug.Log("💡 Click your QUEST button to test it!");
            Debug.Log("🎯 Your existing QuestManager will handle all the quest logic!");
        }
        
        [ContextMenu("Test Quest Button")]
        public void TestQuestButton()
        {
            GameObject questButton = GameObject.Find("QuestButton");
            if (questButton != null)
            {
                SimpleQuestButtonHandler handler = questButton.GetComponent<SimpleQuestButtonHandler>();
                if (handler != null)
                {
                    handler.TestQuestToggle();
                }
                else
                {
                    Debug.LogWarning("⚠️ SimpleQuestButtonHandler not found - run 'Connect Quest System' first");
                }
            }
        }
    }
}