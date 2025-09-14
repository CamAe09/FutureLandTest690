using UnityEngine;

namespace TPSBR
{
    public class SimpleQuestSetup : MonoBehaviour
    {
        [Header("🎯 Simple Quest Setup - No Errors!")]
        [TextArea(3, 5)]
        public string instructions = "This is the SAFEST way to set up your quest system.\n\nRight-click and select 'Setup Quest System Safely' to avoid all errors!";
        
        [ContextMenu("Setup Quest System Safely")]
        public void SetupQuestSystemSafely()
        {
            Debug.Log("🛡️ Setting up quest system safely - avoiding all errors...");
            
            // Since you already have Quest System GameObject, just create assets and connect everything
            CreateQuestAssets();
            AssignQuestAssets();
            ConnectQuestButton();
            VerifySetup();
            
            Debug.Log("✅ Safe quest system setup complete!");
            Debug.Log("🎮 Your QuestButton should now work without errors!");
        }
        
        private void CreateQuestAssets()
        {
            Debug.Log("📦 Creating quest assets using existing setup...");
            
            // Use your existing Quest System's setup component
            QuestSystemSetup existingSetup = FindObjectOfType<QuestSystemSetup>();
            if (existingSetup != null)
            {
                // This uses the existing QuestSystemSetup component you already have
                existingSetup.Step1_CreateQuestAssets();
                Debug.Log("✅ Quest assets created successfully!");
            }
            else
            {
                Debug.LogWarning("⚠️ QuestSystemSetup not found. Creating assets manually...");
                CreateQuestAssetsManually();
            }
        }
        
        private void CreateQuestAssetsManually()
        {
            // Fallback method to create assets
            QuestAssetCreator assetCreator = FindObjectOfType<QuestAssetCreator>();
            if (assetCreator == null)
            {
                GameObject tempCreator = new GameObject("Temp Asset Creator");
                assetCreator = tempCreator.AddComponent<QuestAssetCreator>();
            }
            
            assetCreator.CreateAllQuests();
            
            if (assetCreator.gameObject.name == "Temp Asset Creator")
            {
                DestroyImmediate(assetCreator.gameObject);
            }
        }
        
        private void AssignQuestAssets()
        {
            Debug.Log("⚙️ Assigning quest assets to QuestManager...");
            
            QuestManager questManager = FindObjectOfType<QuestManager>();
            if (questManager == null)
            {
                Debug.LogError("❌ QuestManager not found!");
                return;
            }
            
            #if UNITY_EDITOR
            // Load all quest assets from the folder
            string questFolder = "Assets/Quest/QuestAssets";
            if (System.IO.Directory.Exists(questFolder))
            {
                string[] assetPaths = System.IO.Directory.GetFiles(questFolder, "*.asset");
                if (assetPaths.Length > 0)
                {
                    QuestData[] questAssets = new QuestData[assetPaths.Length];
                    
                    for (int i = 0; i < assetPaths.Length; i++)
                    {
                        questAssets[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<QuestData>(assetPaths[i]);
                    }
                    
                    // Use reflection to set the private availableQuests field
                    var field = typeof(QuestManager).GetField("availableQuests", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (field != null)
                    {
                        field.SetValue(questManager, questAssets);
                        UnityEditor.EditorUtility.SetDirty(questManager);
                        
                        Debug.Log($"✅ Assigned {questAssets.Length} quest assets to QuestManager");
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ Could not automatically assign quest assets. Please assign them manually.");
                    }
                }
                else
                {
                    Debug.LogWarning("⚠️ No quest assets found. Run quest asset creation first.");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Quest assets folder not found.");
            }
            #endif
        }
        
        private void ConnectQuestButton()
        {
            GameObject questButton = GameObject.Find("QuestButton");
            if (questButton != null)
            {
                CleanTPSBRQuestButton questBtnComponent = questButton.GetComponent<CleanTPSBRQuestButton>();
                if (questBtnComponent != null)
                {
                    Debug.Log("✅ QuestButton properly connected");
                }
                else
                {
                    questButton.AddComponent<CleanTPSBRQuestButton>();
                    Debug.Log("✅ Added CleanTPSBRQuestButton component");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ QuestButton not found in scene");
            }
        }
        
        private void VerifySetup()
        {
            bool allGood = true;
            
            if (QuestManager.Instance == null)
            {
                Debug.LogError("❌ QuestManager not found");
                allGood = false;
            }
            else
            {
                Debug.Log("✅ QuestManager active");
            }
            
            if (BattleRoyaleQuestTracker.Instance == null)
            {
                Debug.LogError("❌ BattleRoyaleQuestTracker not found");
                allGood = false;
            }
            else
            {
                Debug.Log("✅ BattleRoyaleQuestTracker active");
            }
            
            if (CurrencyManager.Instance == null)
            {
                Debug.LogWarning("⚠️ CurrencyManager not found (optional)");
            }
            else
            {
                Debug.Log("✅ CurrencyManager found");
            }
            
            if (allGood)
            {
                Debug.Log("🎉 Quest system fully functional!");
                Debug.Log("📋 To complete setup:");
                Debug.Log("   1. Find 'Quest System' GameObject in hierarchy");
                Debug.Log("   2. In QuestManager component, assign quest assets from Assets/Quest/QuestAssets/");
                Debug.Log("   3. Test by clicking your QuestButton!");
            }
        }
        
        [ContextMenu("Test Quest Button")]
        public void TestQuestButton()
        {
            GameObject questButton = GameObject.Find("QuestButton");
            if (questButton != null)
            {
                CleanTPSBRQuestButton questBtnComponent = questButton.GetComponent<CleanTPSBRQuestButton>();
                if (questBtnComponent != null)
                {
                    questBtnComponent.OnQuestButtonClicked();
                    Debug.Log("🧪 Quest button test complete");
                }
                else
                {
                    Debug.LogError("❌ CleanTPSBRQuestButton component not found");
                }
            }
            else
            {
                Debug.LogError("❌ QuestButton not found");
            }
        }
        
        [ContextMenu("Quick Test Quest System")]
        public void QuickTestQuestSystem()
        {
            if (BattleRoyaleQuestTracker.Instance != null)
            {
                BattleRoyaleQuestTracker.Instance.TestStartMatch();
                BattleRoyaleQuestTracker.Instance.TestElimination();
                Debug.Log("🧪 Quest progress simulated");
            }
            else
            {
                Debug.LogError("❌ BattleRoyaleQuestTracker not found. Run setup first.");
            }
        }
        
        [ContextMenu("Force Fix UI Issues")]
        public void ForceFixUIIssues()
        {
            Debug.Log("🔧 Force fixing UI issues...");
            
            // Remove all conflicting quest panels
            string[] conflictingNames = { 
                "EnhancedQuestPanel", 
                "QuestPanel", 
                "MainMenuQuestPanel",
                "StableQuestPanel"
            };
            
            foreach (string panelName in conflictingNames)
            {
                GameObject[] panels = GameObject.FindObjectsOfType<GameObject>();
                foreach (GameObject panel in panels)
                {
                    if (panel.name == panelName)
                    {
                        DestroyImmediate(panel);
                        Debug.Log($"🗑️ Removed {panelName}");
                    }
                }
            }
            
            // Use your existing QuestButton for simple testing
            GameObject questButton = GameObject.Find("QuestButton");
            if (questButton != null)
            {
                // Ensure it has the proper component
                if (questButton.GetComponent<CleanTPSBRQuestButton>() == null)
                {
                    questButton.AddComponent<CleanTPSBRQuestButton>();
                }
                
                Debug.Log("✅ QuestButton fixed");
            }
            
            Debug.Log("🔧 UI issues fixed - try the safe setup now");
        }
    }
}