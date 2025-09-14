using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace TPSBR
{
    public class QuestUIManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform questContainer;
        [SerializeField] private GameObject questItemPrefab;
        [SerializeField] private bool debugMode = true;
        
        private Dictionary<string, GameObject> questUIItems = new Dictionary<string, GameObject>();
        
        private void Start()
        {
            FindUIReferences();
            SubscribeToEvents();
            RefreshQuestUI();
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        
        private void FindUIReferences()
        {
            if (questContainer == null)
            {
                questContainer = transform.Find("Quest Scroll Area/Viewport/Quest Container");
                if (questContainer == null)
                {
                    Debug.LogError("❌ Quest Container not found! Make sure the UI structure is correct.");
                    return;
                }
            }
            
            if (questItemPrefab == null)
            {
                questItemPrefab = Resources.Load<GameObject>("Quest Item Prefab");
                if (questItemPrefab == null)
                {
                    CreateQuestItemPrefab();
                }
            }
            
            if (debugMode)
            {
                Debug.Log("✅ Quest UI references found and set up");
            }
        }
        
        private void CreateQuestItemPrefab()
        {
            QuestUISetup questSetup = FindObjectOfType<QuestUISetup>();
            if (questSetup == null)
            {
                questSetup = gameObject.AddComponent<QuestUISetup>();
            }
            
            questSetup.CreateEnhancedQuestSystem();
            questItemPrefab = GameObject.Find("Quest Item Prefab");
        }
        
        private void SubscribeToEvents()
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.OnQuestCompleted += OnQuestCompleted;
                QuestManager.OnQuestProgressUpdated += OnQuestProgressUpdated;
                QuestManager.OnQuestsRefreshed += RefreshQuestUI;
            }
        }
        
        private void UnsubscribeFromEvents()
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.OnQuestCompleted -= OnQuestCompleted;
                QuestManager.OnQuestProgressUpdated -= OnQuestProgressUpdated;
                QuestManager.OnQuestsRefreshed -= RefreshQuestUI;
            }
        }
        
        private void OnQuestCompleted(QuestData questData, QuestProgress questProgress)
        {
            UpdateQuestUI(questData, questProgress);
            
            if (debugMode)
            {
                Debug.Log($"🎉 Quest UI updated for completed quest: {questData.questName}");
            }
        }
        
        private void OnQuestProgressUpdated(QuestData questData, QuestProgress questProgress)
        {
            UpdateQuestUI(questData, questProgress);
        }
        
        public void RefreshQuestUI()
        {
            ClearQuestUI();
            
            if (QuestManager.Instance == null)
            {
                if (debugMode)
                {
                    Debug.LogWarning("⚠️ QuestManager not found. Creating quest items disabled.");
                }
                return;
            }
            
            var activeQuests = QuestManager.Instance.ActiveQuests;
            var availableQuests = QuestManager.Instance.AvailableQuests;
            
            foreach (var kvp in activeQuests)
            {
                var questData = availableQuests.FirstOrDefault(q => q.name == kvp.Key);
                if (questData != null)
                {
                    CreateQuestUI(questData, kvp.Value);
                }
            }
            
            if (debugMode)
            {
                Debug.Log($"🔄 Quest UI refreshed with {activeQuests.Count} active quests");
            }
        }
        
        private void CreateQuestUI(QuestData questData, QuestProgress questProgress)
        {
            if (questContainer == null || questItemPrefab == null)
            {
                if (debugMode)
                {
                    Debug.LogError("❌ Missing UI references for creating quest UI");
                }
                return;
            }
            
            GameObject questItem = Instantiate(questItemPrefab, questContainer);
            questItem.name = $"Quest_{questData.name}";
            questItem.SetActive(true);
            
            questUIItems[questData.name] = questItem;
            
            QuestItemUI questItemUI = questItem.GetComponent<QuestItemUI>();
            if (questItemUI == null)
            {
                questItemUI = questItem.AddComponent<QuestItemUI>();
            }
            
            questItemUI.Initialize(questData, questProgress);
            
            if (debugMode)
            {
                Debug.Log($"✅ Created quest UI for: {questData.questName}");
            }
        }
        
        private void UpdateQuestUI(QuestData questData, QuestProgress questProgress)
        {
            if (questUIItems.TryGetValue(questData.name, out GameObject questItem))
            {
                QuestItemUI questItemUI = questItem.GetComponent<QuestItemUI>();
                if (questItemUI != null)
                {
                    questItemUI.UpdateProgress(questProgress);
                }
            }
        }
        
        private void ClearQuestUI()
        {
            foreach (var questItem in questUIItems.Values)
            {
                if (questItem != null)
                {
                    DestroyImmediate(questItem);
                }
            }
            
            questUIItems.Clear();
            
            if (debugMode)
            {
                Debug.Log("🗑️ Cleared existing quest UI items");
            }
        }
        
        public void OpenQuestPanel()
        {
            gameObject.SetActive(true);
            RefreshQuestUI();
            
            if (debugMode)
            {
                Debug.Log("🎯 Quest panel opened");
            }
        }
        
        public void CloseQuestPanel()
        {
            gameObject.SetActive(false);
            
            if (debugMode)
            {
                Debug.Log("🎯 Quest panel closed");
            }
        }
        
        [ContextMenu("Refresh Quest UI Now")]
        public void RefreshQuestUINow()
        {
            RefreshQuestUI();
        }
        
        [ContextMenu("Test Open Panel")]
        public void TestOpenPanel()
        {
            OpenQuestPanel();
        }
    }
}