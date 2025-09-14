using UnityEngine;

namespace TPSBR
{
    public class QuestSystemTest : MonoBehaviour
    {
        [ContextMenu("Test Quest System Setup")]
        public void TestQuestSystemSetup()
        {
            Debug.Log("🧪 Testing Quest System Setup...");
            
            // Test 1: Check if quest system components exist
            if (QuestManager.Instance != null)
            {
                Debug.Log("✅ QuestManager found");
            }
            else
            {
                Debug.LogWarning("⚠️ QuestManager not found");
            }
            
            if (BattleRoyaleQuestTracker.Instance != null)
            {
                Debug.Log("✅ BattleRoyaleQuestTracker found");
            }
            else
            {
                Debug.LogWarning("⚠️ BattleRoyaleQuestTracker not found");
            }
            
            if (CurrencyManager.Instance != null)
            {
                Debug.Log("✅ CurrencyManager found");
            }
            else
            {
                Debug.LogWarning("⚠️ CurrencyManager not found");
            }
            
            // Test 2: Check quest button
            GameObject questButton = GameObject.Find("QuestButton");
            if (questButton != null)
            {
                CleanTPSBRQuestButton questBtnComponent = questButton.GetComponent<CleanTPSBRQuestButton>();
                if (questBtnComponent != null)
                {
                    Debug.Log("✅ Quest button properly set up");
                }
                else
                {
                    Debug.LogWarning("⚠️ Quest button missing CleanTPSBRQuestButton component");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ QuestButton GameObject not found");
            }
            
            Debug.Log("🧪 Quest System test complete!");
        }
        
        [ContextMenu("Force Create Quest System")]
        public void ForceCreateQuestSystem()
        {
            Debug.Log("🔧 Force creating quest system components...");
            
            // Create Quest System GameObject if it doesn't exist
            GameObject questSystemObj = GameObject.Find("Quest System");
            if (questSystemObj == null)
            {
                questSystemObj = new GameObject("Quest System");
                questSystemObj.AddComponent<QuestManager>();
                questSystemObj.AddComponent<BattleRoyaleQuestTracker>();
                DontDestroyOnLoad(questSystemObj);
                Debug.Log("✅ Created Quest System GameObject");
            }
            
            // Add quest UI components
            Transform menuUI = GameObject.Find("MenuUI")?.transform;
            if (menuUI != null)
            {
                QuestUISetup uiSetup = menuUI.GetComponent<QuestUISetup>();
                if (uiSetup == null)
                {
                    uiSetup = menuUI.gameObject.AddComponent<QuestUISetup>();
                    Debug.Log("✅ Added QuestUISetup to MenuUI");
                }
                
                // Create quest UI if panel doesn't exist
                if (GameObject.Find("EnhancedQuestPanel") == null)
                {
                    uiSetup.CreateEnhancedQuestSystem();
                    Debug.Log("✅ Created quest UI system");
                }
            }
            
            Debug.Log("🔧 Force creation complete!");
        }
    }
}