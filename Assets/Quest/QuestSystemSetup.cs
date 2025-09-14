using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace TPSBR
{
    public class QuestSystemSetup : MonoBehaviour
    {
        [Header("Setup Progress")]
        [SerializeField] private bool questAssetsCreated = false;
        [SerializeField] private bool questSystemCreated = false;
        [SerializeField] private bool questUICreated = false;
        [SerializeField] private bool buttonConnected = false;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        [ContextMenu("Setup Complete Quest System")]
        public void SetupCompleteQuestSystem()
        {
            if (debugMode)
            {
                Debug.Log("🎯 Starting complete quest system setup...");
            }

            Step1_CreateQuestAssets();
            Step2_CreateQuestSystemGameObject();
            Step3_SetupQuestUI();
            Step4_ConnectQuestButton();
            Step5_FinalInstructions();

            if (debugMode)
            {
                Debug.Log("✅ Complete quest system setup finished!");
            }
        }

        [ContextMenu("Step 1: Create Quest Assets")]
        public void Step1_CreateQuestAssets()
        {
            if (questAssetsCreated)
            {
                if (debugMode) Debug.Log("⏭️ Quest assets already created, skipping...");
                return;
            }

            QuestAssetCreator creator = FindObjectOfType<QuestAssetCreator>();
            if (creator == null)
            {
                GameObject creatorObj = new GameObject("Quest Asset Creator");
                creator = creatorObj.AddComponent<QuestAssetCreator>();
            }

            creator.CreateAllQuests();
            questAssetsCreated = true;

            if (debugMode)
            {
                Debug.Log("✅ Step 1 complete: Quest assets created");
            }
        }

        [ContextMenu("Step 2: Create Quest System")]
        public void Step2_CreateQuestSystemGameObject()
        {
            if (questSystemCreated)
            {
                if (debugMode) Debug.Log("⏭️ Quest system already created, skipping...");
                return;
            }

            GameObject questSystem = GameObject.Find("Quest System");
            if (questSystem == null)
            {
                questSystem = new GameObject("Quest System");
                questSystem.AddComponent<QuestManager>();
                questSystem.AddComponent<BattleRoyaleQuestTracker>();

                DontDestroyOnLoad(questSystem);
            }

            questSystemCreated = true;

            if (debugMode)
            {
                Debug.Log("✅ Step 2 complete: Quest system GameObject created with QuestManager and BattleRoyaleQuestTracker");
            }
        }

        [ContextMenu("Step 3: Setup Quest UI")]
        public void Step3_SetupQuestUI()
        {
            if (questUICreated)
            {
                if (debugMode) Debug.Log("⏭️ Quest UI already prepared, skipping...");
                return;
            }

            // COMPLETELY SKIP UI CREATION - Let CleanTPSBRQuestButton handle it dynamically
            // This avoids all Transform destroyed errors and conflicts with existing UI

            Transform menuUI = GameObject.Find("MenuUI")?.transform;
            if (menuUI == null)
            {
                if (debugMode) Debug.LogWarning("⚠️ MenuUI not found. Quest UI will be created dynamically when quest button is pressed.");
                questUICreated = true;
                return;
            }

            // Just ensure the QuestUISetup component exists, but don't create UI
            QuestUISetup uiSetup = menuUI.GetComponent<QuestUISetup>();
            if (uiSetup == null)
            {
                uiSetup = menuUI.gameObject.AddComponent<QuestUISetup>();
            }

            // Clean up any existing conflicting panels safely
            CleanupOldQuestPanels();

            questUICreated = true;

            if (debugMode)
            {
                Debug.Log("✅ Step 3 complete: Quest UI component ready (UI will be created dynamically by quest button)");
            }
        }

        private void CleanupOldQuestPanels()
        {
            try
            {
                // Find and remove old quest panels that might conflict
                string[] panelsToRemove = { "EnhancedQuestPanel", "QuestPanel", "StableQuestPanel" };

                foreach (string panelName in panelsToRemove)
                {
                    GameObject oldPanel = GameObject.Find(panelName);
                    if (oldPanel != null)
                    {
                        DestroyImmediate(oldPanel);
                        if (debugMode)
                        {
                            Debug.Log($"🗑️ Cleaned up old {panelName}");
                        }
                    }
                }

                // Check for panels in MenuUI too
                Transform menuUI = GameObject.Find("MenuUI")?.transform;
                if (menuUI != null)
                {
                    foreach (string panelName in panelsToRemove)
                    {
                        Transform oldPanel = menuUI.Find(panelName);
                        if (oldPanel != null)
                        {
                            DestroyImmediate(oldPanel.gameObject);
                            if (debugMode)
                            {
                                Debug.Log($"🗑️ Cleaned up old {panelName} from MenuUI");
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                if (debugMode)
                {
                    Debug.LogWarning($"⚠️ Minor cleanup issue (safe to ignore): {e.Message}");
                }
            }
        }

        [ContextMenu("Step 4: Connect Quest Button")]
        public void Step4_ConnectQuestButton()
        {
            if (buttonConnected)
            {
                if (debugMode) Debug.Log("⏭️ Quest button already connected, skipping...");
                return;
            }

            GameObject questButton = GameObject.Find("QuestButton");
            if (questButton == null)
            {
                Transform menuUI = GameObject.Find("MenuUI")?.transform;
                if (menuUI != null)
                {
                    questButton = menuUI.Find("QuestButton")?.gameObject;
                }
            }

            if (questButton != null)
            {
                CleanTPSBRQuestButton questBtnComponent = questButton.GetComponent<CleanTPSBRQuestButton>();
                if (questBtnComponent == null)
                {
                    questBtnComponent = questButton.AddComponent<CleanTPSBRQuestButton>();
                }

                buttonConnected = true;

                if (debugMode)
                {
                    Debug.Log("✅ Step 4 complete: Quest button connected with CleanTPSBRQuestButton");
                }
            }
            else
            {
                if (debugMode)
                {
                    Debug.LogWarning("⚠️ QuestButton not found. Please make sure you have a QuestButton GameObject in your MenuUI");
                }
            }
        }

        [ContextMenu("Step 5: Show Final Instructions")]
        public void Step5_FinalInstructions()
        {
            if (debugMode)
            {
                Debug.Log("🎯 QUEST SYSTEM SETUP COMPLETE!");
                Debug.Log("📝 FINAL SETUP INSTRUCTIONS:");
                Debug.Log("   1. Find the QuestManager component in the Quest System GameObject");
                Debug.Log("   2. Assign all quest assets from Assets/Quest/QuestAssets/ to the Available Quests array");
                Debug.Log("   3. Test the quest button in your menu to open the quest panel");
                Debug.Log("   4. The quest UI will be created automatically when you click the quest button");
                Debug.Log("   5. Integrate BattleRoyaleQuestTracker.Instance calls in your game scripts:");
                Debug.Log("      - Call StartMatch() when a match begins");
                Debug.Log("      - Call EndMatch() when a match ends");
                Debug.Log("      - Call OnEliminationScored() when player gets kills");
                Debug.Log("      - Call OnDamageDealt() when player deals damage");
                Debug.Log("      - Call OnBuildingLooted() when player loots buildings");
                Debug.Log("      - And other tracking methods as needed");
                Debug.Log("   6. The quest system will automatically save progress and give coins!");
            }
        }

        [ContextMenu("Reset Setup Flags")]
        public void ResetSetupFlags()
        {
            questAssetsCreated = false;
            questSystemCreated = false;
            questUICreated = false;
            buttonConnected = false;

            if (debugMode)
            {
                Debug.Log("🔄 Setup flags reset. You can run the setup again.");
            }
        }

        [ContextMenu("Test Quest System")]
        public void TestQuestSystem()
        {
            if (QuestManager.Instance == null)
            {
                Debug.LogError("❌ QuestManager not found! Run the setup first.");
                return;
            }

            if (BattleRoyaleQuestTracker.Instance == null)
            {
                Debug.LogError("❌ BattleRoyaleQuestTracker not found! Run the setup first.");
                return;
            }

            Debug.Log("🧪 Testing quest system...");

            BattleRoyaleQuestTracker.Instance.TestStartMatch();
            BattleRoyaleQuestTracker.Instance.TestElimination();
            BattleRoyaleQuestTracker.Instance.TestQuestProgress();
            BattleRoyaleQuestTracker.Instance.TestEndMatchVictory();

            Debug.Log("✅ Quest system test complete! Check the quest panel to see progress.");
        }

        private void Start()
        {
            if (debugMode)
            {
                Debug.Log("🎯 Quest System Setup ready. Use the context menu to set up the complete quest system.");
            }
        }
    }
}
