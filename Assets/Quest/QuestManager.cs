using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TPSBR
{
    public class QuestManager : MonoBehaviour
    {
        [Header("Quest Settings")]
        [SerializeField] private QuestData[] availableQuests;
        [SerializeField] private int maxActiveQuests = 10;
        [SerializeField] private bool debugMode = true;
        
        [Header("Quest Refresh Settings")]
        [SerializeField] private bool enableDailyRefresh = true;
        [SerializeField] private bool enableWeeklyRefresh = true;
        
        private Dictionary<string, QuestProgress> activeQuests = new Dictionary<string, QuestProgress>();
        private DateTime lastDailyRefresh;
        private DateTime lastWeeklyRefresh;
        
        public static QuestManager Instance { get; private set; }
        
        public static event Action<QuestData, QuestProgress> OnQuestCompleted;
        public static event Action<QuestData, QuestProgress> OnQuestProgressUpdated;
        public static event Action OnQuestsRefreshed;
        
        public IReadOnlyDictionary<string, QuestProgress> ActiveQuests => activeQuests;
        public QuestData[] AvailableQuests => availableQuests;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeQuestSystem();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            LoadQuestData();
            CheckForQuestRefresh();
            
            if (debugMode)
            {
                Debug.Log($"🎯 Quest Manager initialized with {activeQuests.Count} active quests");
            }
        }
        
        private void InitializeQuestSystem()
        {
            lastDailyRefresh = DateTime.Parse(PlayerPrefs.GetString("LastDailyRefresh", DateTime.Now.AddDays(-1).ToString()));
            lastWeeklyRefresh = DateTime.Parse(PlayerPrefs.GetString("LastWeeklyRefresh", DateTime.Now.AddDays(-7).ToString()));
        }
        
        public void UpdateQuestProgress(QuestObjectiveType objectiveType, int amount = 1, object additionalData = null)
        {
            var questsToUpdate = activeQuests.Values
                .Where(progress => !progress.isCompleted)
                .Select(progress => new { Progress = progress, Quest = GetQuestData(progress.questId) })
                .Where(item => item.Quest != null && item.Quest.objectiveType == objectiveType)
                .ToList();
            
            foreach (var item in questsToUpdate)
            {
                var questProgress = item.Progress;
                var questData = item.Quest;
                
                if (ShouldUpdateQuest(questData, additionalData))
                {
                    questProgress.AddProgress(amount);
                    
                    if (questProgress.currentProgress >= questData.targetAmount)
                    {
                        CompleteQuest(questData, questProgress);
                    }
                    else
                    {
                        OnQuestProgressUpdated?.Invoke(questData, questProgress);
                        
                        if (debugMode)
                        {
                            Debug.Log($"🎯 Quest progress: {questData.questName} ({questProgress.currentProgress}/{questData.targetAmount})");
                        }
                    }
                }
            }
            
            SaveQuestData();
        }
        
        private bool ShouldUpdateQuest(QuestData questData, object additionalData)
        {
            return questData.objectiveType switch
            {
                QuestObjectiveType.FinishTopPercent => additionalData is int placement && IsTopPercentage(placement, (int)additionalData),
                QuestObjectiveType.CloseRangeEliminations => additionalData is float distance && distance <= 10f,
                QuestObjectiveType.HeadshotEliminations => additionalData is bool isHeadshot && isHeadshot,
                QuestObjectiveType.LandInHighRiskAreas => additionalData is string location && IsHighRiskArea(location),
                QuestObjectiveType.UseWeaponTypes => additionalData is string weaponType && !HasUsedWeaponType(questData.questName, weaponType),
                QuestObjectiveType.LandInLocations => additionalData is string landLocation && !HasVisitedLocation(questData.questName, landLocation),
                _ => true
            };
        }
        
        private bool IsTopPercentage(int placement, int totalPlayers)
        {
            float percentage = (float)placement / totalPlayers;
            return percentage <= 0.5f;
        }
        
        private bool IsHighRiskArea(string location)
        {
            string[] highRiskAreas = { "Hot Zone", "Military Base", "Supply Drop", "High Loot Area" };
            return highRiskAreas.Any(area => location.Contains(area));
        }
        
        private bool HasUsedWeaponType(string questId, string weaponType)
        {
            string key = $"{questId}_WeaponTypes";
            string usedTypes = PlayerPrefs.GetString(key, "");
            return usedTypes.Contains(weaponType);
        }
        
        private bool HasVisitedLocation(string questId, string location)
        {
            string key = $"{questId}_Locations";
            string visitedLocations = PlayerPrefs.GetString(key, "");
            return visitedLocations.Contains(location);
        }
        
        private void CompleteQuest(QuestData questData, QuestProgress questProgress)
        {
            questProgress.CompleteQuest();
            OnQuestCompleted?.Invoke(questData, questProgress);
            
            if (debugMode)
            {
                Debug.Log($"🎉 Quest completed: {questData.questName} - Reward: {questData.coinReward} coins");
            }
            
            SaveQuestData();
        }
        
        public bool ClaimQuestReward(string questId)
        {
            if (activeQuests.TryGetValue(questId, out QuestProgress progress) && 
                progress.isCompleted && !progress.isRewardClaimed)
            {
                QuestData questData = GetQuestData(questId);
                if (questData != null)
                {
                    progress.ClaimReward();
                    
                    if (CurrencyManager.Instance != null)
                    {
                        CurrencyManager.Instance.AddCoins(questData.coinReward);
                    }
                    
                    if (debugMode)
                    {
                        Debug.Log($"💰 Claimed reward: {questData.coinReward} coins for quest: {questData.questName}");
                    }
                    
                    SaveQuestData();
                    return true;
                }
            }
            
            return false;
        }
        
        public void AddQuest(QuestData questData)
        {
            if (questData == null || activeQuests.ContainsKey(questData.name))
                return;
            
            if (activeQuests.Count >= maxActiveQuests)
            {
                if (debugMode)
                {
                    Debug.LogWarning($"⚠️ Cannot add quest {questData.questName}: Maximum active quests reached");
                }
                return;
            }
            
            activeQuests[questData.name] = new QuestProgress(questData.name);
            
            if (debugMode)
            {
                Debug.Log($"✅ Added quest: {questData.questName}");
            }
            
            SaveQuestData();
        }
        
        public void RefreshQuests()
        {
            RemoveExpiredQuests();
            AddNewQuests();
            OnQuestsRefreshed?.Invoke();
            
            if (debugMode)
            {
                Debug.Log($"🔄 Quests refreshed. Active quests: {activeQuests.Count}");
            }
        }
        
        private void CheckForQuestRefresh()
        {
            bool needsRefresh = false;
            
            if (enableDailyRefresh && DateTime.Now.Date > lastDailyRefresh.Date)
            {
                RefreshQuestsByType(QuestType.Daily);
                lastDailyRefresh = DateTime.Now;
                needsRefresh = true;
            }
            
            if (enableWeeklyRefresh && (DateTime.Now - lastWeeklyRefresh).TotalDays >= 7)
            {
                RefreshQuestsByType(QuestType.Weekly);
                lastWeeklyRefresh = DateTime.Now;
                needsRefresh = true;
            }
            
            if (needsRefresh)
            {
                SaveRefreshTimes();
                OnQuestsRefreshed?.Invoke();
            }
        }
        
        private void RefreshQuestsByType(QuestType questType)
        {
            var questsToRemove = activeQuests.Values
                .Where(progress => GetQuestData(progress.questId)?.questType == questType)
                .ToList();
            
            foreach (var progress in questsToRemove)
            {
                activeQuests.Remove(progress.questId);
            }
            
            var questsToAdd = availableQuests
                .Where(quest => quest.questType == questType)
                .OrderBy(x => UnityEngine.Random.Range(0f, 1f))
                .Take(GetMaxQuestsForType(questType));
            
            foreach (var quest in questsToAdd)
            {
                if (!activeQuests.ContainsKey(quest.name))
                {
                    activeQuests[quest.name] = new QuestProgress(quest.name);
                }
            }
            
            if (debugMode)
            {
                Debug.Log($"🔄 Refreshed {questType} quests");
            }
        }
        
        private int GetMaxQuestsForType(QuestType questType)
        {
            return questType switch
            {
                QuestType.Daily => 3,
                QuestType.Combat => 2,
                QuestType.Weekly => 2,
                QuestType.Progression => 2,
                QuestType.Special => 1,
                _ => 1
            };
        }
        
        private void RemoveExpiredQuests()
        {
            var expiredQuests = activeQuests.Values
                .Where(progress => {
                    var questData = GetQuestData(progress.questId);
                    return questData != null && questData.hasTimeLimit && progress.IsExpired(questData.timeLimitHours);
                })
                .ToList();
            
            foreach (var progress in expiredQuests)
            {
                activeQuests.Remove(progress.questId);
                
                if (debugMode)
                {
                    Debug.Log($"⏰ Removed expired quest: {progress.questId}");
                }
            }
        }
        
        private void AddNewQuests()
        {
            if (activeQuests.Count >= maxActiveQuests) return;
            
            var availableNewQuests = availableQuests
                .Where(quest => !activeQuests.ContainsKey(quest.name))
                .OrderBy(x => UnityEngine.Random.Range(0f, 1f))
                .Take(maxActiveQuests - activeQuests.Count);
            
            foreach (var quest in availableNewQuests)
            {
                AddQuest(quest);
            }
        }
        
        public QuestData GetQuestData(string questId)
        {
            return availableQuests.FirstOrDefault(quest => quest.name == questId);
        }
        
        private void SaveQuestData()
        {
            foreach (var kvp in activeQuests)
            {
                string key = $"Quest_{kvp.Key}";
                string progressJson = JsonUtility.ToJson(kvp.Value);
                PlayerPrefs.SetString(key, progressJson);
            }
            
            string activeQuestKeys = string.Join(",", activeQuests.Keys);
            PlayerPrefs.SetString("ActiveQuestKeys", activeQuestKeys);
            PlayerPrefs.Save();
        }
        
        private void LoadQuestData()
        {
            string activeQuestKeys = PlayerPrefs.GetString("ActiveQuestKeys", "");
            if (string.IsNullOrEmpty(activeQuestKeys)) return;
            
            string[] keys = activeQuestKeys.Split(',');
            foreach (string questId in keys)
            {
                if (string.IsNullOrEmpty(questId)) continue;
                
                string key = $"Quest_{questId}";
                string progressJson = PlayerPrefs.GetString(key, "");
                
                if (!string.IsNullOrEmpty(progressJson))
                {
                    try
                    {
                        QuestProgress progress = JsonUtility.FromJson<QuestProgress>(progressJson);
                        activeQuests[questId] = progress;
                    }
                    catch (System.Exception e)
                    {
                        if (debugMode)
                        {
                            Debug.LogWarning($"⚠️ Failed to load quest progress for {questId}: {e.Message}");
                        }
                    }
                }
            }
        }
        
        private void SaveRefreshTimes()
        {
            PlayerPrefs.SetString("LastDailyRefresh", lastDailyRefresh.ToString());
            PlayerPrefs.SetString("LastWeeklyRefresh", lastWeeklyRefresh.ToString());
            PlayerPrefs.Save();
        }
        
        [ContextMenu("Refresh All Quests")]
        public void ForceRefreshAllQuests()
        {
            activeQuests.Clear();
            AddNewQuests();
            OnQuestsRefreshed?.Invoke();
            SaveQuestData();
            
            Debug.Log("🔄 Force refreshed all quests");
        }
        
        [ContextMenu("Complete All Quests")]
        public void DebugCompleteAllQuests()
        {
            foreach (var kvp in activeQuests.ToList())
            {
                var questData = GetQuestData(kvp.Key);
                if (questData != null)
                {
                    kvp.Value.currentProgress = questData.targetAmount;
                    CompleteQuest(questData, kvp.Value);
                }
            }
        }
        
        [ContextMenu("Add Test Coins for Completed Quests")]
        public void DebugClaimAllRewards()
        {
            foreach (var kvp in activeQuests.ToList())
            {
                if (kvp.Value.isCompleted && !kvp.Value.isRewardClaimed)
                {
                    ClaimQuestReward(kvp.Key);
                }
            }
        }
    }
}