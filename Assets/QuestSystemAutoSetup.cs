using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TPSBR
{
    public class QuestSystemAutoSetup : MonoBehaviour
    {
        [Header("🎯 Automatic Quest System Setup")]
        [TextArea(3, 5)]
        public string instructions = "Right-click and select 'Setup Complete Quest System with Assets' to automatically create everything!";
        
        [Header("Setup Status")]
        [SerializeField] private bool questAssetsCreated = false;
        [SerializeField] private bool questSystemSetup = false;
        [SerializeField] private bool questManagerConfigured = false;
        
        [ContextMenu("Setup Complete Quest System with Assets")]
        public void SetupCompleteQuestSystemWithAssets()
        {
            Debug.Log("🚀 Starting COMPLETE automated quest system setup...");
            
            // Step 1: Create all quest assets
            CreateAllQuestAssets();
            
            // Step 2: Create quest system GameObject
            CreateQuestSystemGameObject();
            
            // Step 3: Setup quest UI
            SetupQuestUI();
            
            // Step 4: Configure QuestManager with all assets
            ConfigureQuestManagerWithAssets();
            
            // Step 5: Verify everything is working
            VerifySetup();
            
            Debug.Log("✅ COMPLETE quest system setup finished!");
            Debug.Log("🎮 Click your QuestButton to test the system!");
        }
        
        private void CreateAllQuestAssets()
        {
            Debug.Log("📦 Creating all quest assets...");
            
            #if UNITY_EDITOR
            string questFolder = "Assets/Quest/QuestAssets";
            
            if (!Directory.Exists(questFolder))
            {
                Directory.CreateDirectory(questFolder);
                Debug.Log($"📁 Created directory: {questFolder}");
            }
            
            // Create all quest categories
            CreateDailyQuests(questFolder);
            CreateCombatQuests(questFolder);
            CreateWeeklyQuests(questFolder);
            CreateProgressionQuests(questFolder);
            CreateSpecialQuests(questFolder);
            
            AssetDatabase.Refresh();
            questAssetsCreated = true;
            
            Debug.Log("✅ All 24 quest assets created successfully!");
            #else
            Debug.LogWarning("⚠️ Quest asset creation only available in editor");
            #endif
        }
        
        private void CreateQuestSystemGameObject()
        {
            Debug.Log("🔧 Creating Quest System GameObject...");
            
            // Remove any existing quest system objects
            GameObject[] existingQuestSystems = GameObject.FindGameObjectsWithTag("Untagged");
            foreach (GameObject obj in existingQuestSystems)
            {
                if (obj.name.Contains("Quest System") || obj.name.Contains("QuestManager"))
                {
                    DestroyImmediate(obj);
                }
            }
            
            // Create new quest system
            GameObject questSystem = new GameObject("Quest System");
            questSystem.AddComponent<QuestManager>();
            questSystem.AddComponent<BattleRoyaleQuestTracker>();
            
            // Only call DontDestroyOnLoad during play mode
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(questSystem);
            }
            
            questSystemSetup = true;
            Debug.Log("✅ Quest System GameObject created with components");
        }
        
        private void SetupQuestUI()
        {
            Debug.Log("🎨 Setting up Quest UI...");
            
            // Find MenuUI
            Transform menuUI = GameObject.Find("MenuUI")?.transform;
            if (menuUI == null)
            {
                Debug.LogError("❌ MenuUI not found!");
                return;
            }
            
            // Add QuestUISetup if not exists
            QuestUISetup uiSetup = menuUI.GetComponent<QuestUISetup>();
            if (uiSetup == null)
            {
                uiSetup = menuUI.gameObject.AddComponent<QuestUISetup>();
            }
            
            // Create quest UI system
            uiSetup.CreateEnhancedQuestSystem();
            
            Debug.Log("✅ Quest UI system created");
        }
        
        private void ConfigureQuestManagerWithAssets()
        {
            Debug.Log("⚙️ Configuring QuestManager with all quest assets...");
            
            #if UNITY_EDITOR
            // Find QuestManager
            QuestManager questManager = FindObjectOfType<QuestManager>();
            if (questManager == null)
            {
                Debug.LogError("❌ QuestManager not found!");
                return;
            }
            
            // Load all quest assets
            string questFolder = "Assets/Quest/QuestAssets";
            string[] assetPaths = Directory.GetFiles(questFolder, "*.asset");
            QuestData[] questAssets = new QuestData[assetPaths.Length];
            
            for (int i = 0; i < assetPaths.Length; i++)
            {
                questAssets[i] = AssetDatabase.LoadAssetAtPath<QuestData>(assetPaths[i]);
            }
            
            // Use reflection to set the private availableQuests field
            var field = typeof(QuestManager).GetField("availableQuests", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(questManager, questAssets);
                questManagerConfigured = true;
                
                Debug.Log($"✅ QuestManager configured with {questAssets.Length} quest assets");
                
                // Mark the object as dirty so it saves
                EditorUtility.SetDirty(questManager);
            }
            else
            {
                Debug.LogWarning("⚠️ Could not automatically assign quest assets. Please assign them manually in the inspector.");
            }
            #endif
        }
        
        private void VerifySetup()
        {
            Debug.Log("🔍 Verifying quest system setup...");
            
            // Check QuestManager
            if (QuestManager.Instance != null)
            {
                Debug.Log("✅ QuestManager is active");
            }
            else
            {
                Debug.LogWarning("⚠️ QuestManager not found");
            }
            
            // Check BattleRoyaleQuestTracker
            if (BattleRoyaleQuestTracker.Instance != null)
            {
                Debug.Log("✅ BattleRoyaleQuestTracker is active");
            }
            else
            {
                Debug.LogWarning("⚠️ BattleRoyaleQuestTracker not found");
            }
            
            // Check CurrencyManager
            if (CurrencyManager.Instance != null)
            {
                Debug.Log("✅ CurrencyManager found (for coin rewards)");
            }
            else
            {
                Debug.LogWarning("⚠️ CurrencyManager not found");
            }
            
            // Check QuestButton
            GameObject questButton = GameObject.Find("QuestButton");
            if (questButton != null && questButton.GetComponent<CleanTPSBRQuestButton>() != null)
            {
                Debug.Log("✅ QuestButton is properly configured");
            }
            else
            {
                Debug.LogWarning("⚠️ QuestButton not properly configured");
            }
            
            Debug.Log("🎯 Setup verification complete!");
        }
        
        #if UNITY_EDITOR
        private void CreateDailyQuests(string folder)
        {
            CreateQuest(folder, "Daily_FirstDrop", "First Drop", "Play 1 match", 
                      QuestType.Daily, QuestDifficulty.Easy, QuestObjectiveType.PlayMatches, 1, 25, true, 24f);
                      
            CreateQuest(folder, "Daily_Survivor", "Survivor", "Survive for 5 minutes in a match", 
                      QuestType.Daily, QuestDifficulty.Easy, QuestObjectiveType.SurviveTime, 1, 50, true, 24f);
                      
            CreateQuest(folder, "Daily_Scavenger", "Scavenger", "Land and loot 3 different buildings", 
                      QuestType.Daily, QuestDifficulty.Easy, QuestObjectiveType.LootBuildings, 3, 40, true, 24f);
                      
            CreateQuest(folder, "Daily_TopHalf", "Top Half", "Finish in top 50% of players", 
                      QuestType.Daily, QuestDifficulty.Easy, QuestObjectiveType.FinishTopPercent, 1, 60, true, 24f);
                      
            CreateQuest(folder, "Daily_DistanceWalker", "Distance Walker", "Travel 1000 meters on foot", 
                      QuestType.Daily, QuestDifficulty.Easy, QuestObjectiveType.TravelDistance, 1, 35, true, 24f);
                      
            CreateQuest(folder, "Daily_ZoneRunner", "Zone Runner", "Survive 3 storm circles", 
                      QuestType.Daily, QuestDifficulty.Easy, QuestObjectiveType.SurviveStormCircles, 1, 75, true, 24f);
        }
        
        private void CreateCombatQuests(string folder)
        {
            CreateQuest(folder, "Combat_FirstBlood", "First Blood", "Get your first elimination", 
                      QuestType.Combat, QuestDifficulty.Medium, QuestObjectiveType.GetEliminations, 1, 100);
                      
            CreateQuest(folder, "Combat_Marksman", "Marksman", "Deal 200 damage to enemies", 
                      QuestType.Combat, QuestDifficulty.Medium, QuestObjectiveType.DealDamage, 1, 125);
                      
            CreateQuest(folder, "Combat_CloseQuarters", "Close Combat", "Get 1 elimination within 10 meters", 
                      QuestType.Combat, QuestDifficulty.Medium, QuestObjectiveType.CloseRangeEliminations, 1, 150);
                      
            CreateQuest(folder, "Combat_Headhunter", "Headhunter", "Get 2 headshot eliminations", 
                      QuestType.Combat, QuestDifficulty.Medium, QuestObjectiveType.HeadshotEliminations, 2, 175);
        }
        
        private void CreateWeeklyQuests(string folder)
        {
            CreateQuest(folder, "Weekly_VictoryRoyale", "Victory Royale", "Win 1 match", 
                      QuestType.Weekly, QuestDifficulty.Hard, QuestObjectiveType.WinMatches, 1, 500, true, 168f);
                      
            CreateQuest(folder, "Weekly_TopTenStreak", "Top 10 Streak", "Finish top 10 in 3 matches", 
                      QuestType.Weekly, QuestDifficulty.Hard, QuestObjectiveType.FinishTopTen, 3, 250, true, 168f);
                      
            CreateQuest(folder, "Weekly_EliminationSpree", "Elimination Spree", "Get 5 total eliminations across all matches", 
                      QuestType.Weekly, QuestDifficulty.Hard, QuestObjectiveType.TotalEliminations, 5, 200, true, 168f);
                      
            CreateQuest(folder, "Weekly_ZoneMaster", "Zone Master", "Survive to final circle 2 times", 
                      QuestType.Weekly, QuestDifficulty.Hard, QuestObjectiveType.SurviveFinalCircle, 2, 300, true, 168f);
        }
        
        private void CreateProgressionQuests(string folder)
        {
            CreateQuest(folder, "Progression_BRVeteran", "Battle Royale Veteran", "Play 10 total matches", 
                      QuestType.Progression, QuestDifficulty.Expert, QuestObjectiveType.PlayMatches, 10, 300);
                      
            CreateQuest(folder, "Progression_StormSurvivor", "Storm Survivor", "Take storm damage and survive 5 times", 
                      QuestType.Progression, QuestDifficulty.Expert, QuestObjectiveType.TakeStormDamage, 5, 400);
                      
            CreateQuest(folder, "Progression_WeaponMaster", "Weapon Master", "Get eliminations with 3 different weapon types", 
                      QuestType.Progression, QuestDifficulty.Expert, QuestObjectiveType.UseWeaponTypes, 3, 350);
                      
            CreateQuest(folder, "Progression_Explorer", "Explorer", "Land in 5 different named locations", 
                      QuestType.Progression, QuestDifficulty.Expert, QuestObjectiveType.LandInLocations, 5, 250);
        }
        
        private void CreateSpecialQuests(string folder)
        {
            CreateQuest(folder, "Special_WeeklyChampion", "Weekly Champion", "Win 3 matches this week", 
                      QuestType.Special, QuestDifficulty.Expert, QuestObjectiveType.WinMatches, 3, 1000, true, 168f);
                      
            CreateQuest(folder, "Special_PerfectGame", "Perfect Game", "Win without taking storm damage", 
                      QuestType.Special, QuestDifficulty.Expert, QuestObjectiveType.WinWithoutStormDamage, 1, 750);
                      
            CreateQuest(folder, "Special_HotDropMaster", "Hot Drop Master", "Land in high-risk areas 5 times", 
                      QuestType.Special, QuestDifficulty.Expert, QuestObjectiveType.LandInHighRiskAreas, 5, 200);
                      
            CreateQuest(folder, "Special_Medic", "Medic", "Use healing items 10 times", 
                      QuestType.Special, QuestDifficulty.Expert, QuestObjectiveType.UseHealingItems, 10, 100);
        }
        
        private void CreateQuest(string folder, string fileName, string questName, string description,
                               QuestType questType, QuestDifficulty difficulty, QuestObjectiveType objectiveType,
                               int targetAmount, int coinReward, bool hasTimeLimit = false, float timeLimitHours = 0f)
        {
            QuestData quest = ScriptableObject.CreateInstance<QuestData>();
            quest.questName = questName;
            quest.description = description;
            quest.questType = questType;
            quest.difficulty = difficulty;
            quest.objectiveType = objectiveType;
            quest.targetAmount = targetAmount;
            quest.coinReward = coinReward;
            quest.hasTimeLimit = hasTimeLimit;
            quest.timeLimitHours = timeLimitHours;
            quest.objectiveDescription = GetObjectiveDescription(objectiveType, targetAmount);
            
            string assetPath = $"{folder}/{fileName}.asset";
            AssetDatabase.CreateAsset(quest, assetPath);
            
            Debug.Log($"📦 Created quest: {questName} ({coinReward} coins)");
        }
        
        private string GetObjectiveDescription(QuestObjectiveType objectiveType, int targetAmount)
        {
            return objectiveType switch
            {
                QuestObjectiveType.PlayMatches => $"Play {targetAmount} match(es)",
                QuestObjectiveType.SurviveTime => "Survive for 5+ minutes",
                QuestObjectiveType.LootBuildings => $"Loot {targetAmount} buildings",
                QuestObjectiveType.FinishTopPercent => "Finish in top 50%",
                QuestObjectiveType.TravelDistance => "Travel 1000+ meters",
                QuestObjectiveType.SurviveStormCircles => "Survive 3+ storm circles",
                QuestObjectiveType.GetEliminations => $"Get {targetAmount} elimination(s)",
                QuestObjectiveType.DealDamage => "Deal 200+ damage",
                QuestObjectiveType.CloseRangeEliminations => $"Get {targetAmount} close-range elimination(s)",
                QuestObjectiveType.HeadshotEliminations => $"Get {targetAmount} headshot elimination(s)",
                QuestObjectiveType.WinMatches => $"Win {targetAmount} match(es)",
                QuestObjectiveType.FinishTopTen => $"Finish top 10 {targetAmount} times",
                QuestObjectiveType.TotalEliminations => $"Get {targetAmount} total eliminations",
                QuestObjectiveType.SurviveFinalCircle => $"Survive to final circle {targetAmount} times",
                QuestObjectiveType.TakeStormDamage => $"Take storm damage {targetAmount} times",
                QuestObjectiveType.UseWeaponTypes => $"Use {targetAmount} different weapon types",
                QuestObjectiveType.LandInLocations => $"Land in {targetAmount} different locations",
                QuestObjectiveType.WinWithoutStormDamage => "Win without storm damage",
                QuestObjectiveType.LandInHighRiskAreas => $"Land in high-risk areas {targetAmount} times",
                QuestObjectiveType.UseHealingItems => $"Use healing items {targetAmount} times",
                _ => $"Complete {targetAmount} objectives"
            };
        }
        #endif
        
        [ContextMenu("Test Quest System")]
        public void TestQuestSystem()
        {
            Debug.Log("🧪 Testing complete quest system...");
            
            if (BattleRoyaleQuestTracker.Instance != null)
            {
                BattleRoyaleQuestTracker.Instance.TestStartMatch();
                BattleRoyaleQuestTracker.Instance.TestElimination();
                BattleRoyaleQuestTracker.Instance.TestQuestProgress();
                
                Debug.Log("✅ Quest system test complete! Check your quest panel.");
            }
            else
            {
                Debug.LogError("❌ BattleRoyaleQuestTracker not found. Run setup first.");
            }
        }
        
        [ContextMenu("Manual Quest Asset Assignment")]
        public void ManualQuestAssetAssignment()
        {
            Debug.Log("📋 Instructions for manual quest asset assignment:");
            Debug.Log("1. Find the 'Quest System' GameObject in your hierarchy");
            Debug.Log("2. Select it and look for the 'QuestManager' component");
            Debug.Log("3. Expand the 'Available Quests' array");
            Debug.Log("4. Set the size to 24");
            Debug.Log("5. Drag all quest assets from Assets/Quest/QuestAssets/ into the slots");
            Debug.Log("6. Your quest assets should be automatically created in Assets/Quest/QuestAssets/");
        }
    }
}