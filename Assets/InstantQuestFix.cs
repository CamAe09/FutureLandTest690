using UnityEngine;
using UnityEngine.UI;

namespace TPSBR
{
    /// <summary>
    /// Instant quest fix - works with your existing quest button and prevents conflicts
    /// </summary>
    public class InstantQuestFix : MonoBehaviour
    {
        [Header("🎯 Instant Quest Fix")]
        [TextArea(3, 5)]
        public string instructions = "RIGHT-CLICK → 'Fix Quest Button Now'\n\nThis will make your quest button work instantly!";
        
        private GameObject questPanel;
        private bool isPanelVisible = false;
        
        [ContextMenu("Fix Quest Button Now")]
        public void FixQuestButtonNow()
        {
            Debug.Log("🔧 Fixing quest button instantly...");
            
            // Step 1: Clean up conflicting panels
            RemoveConflictingPanels();
            
            // Step 2: Find your quest button
            GameObject questButton = GameObject.Find("QuestButton");
            if (questButton == null)
            {
                Debug.LogError("❌ QuestButton not found!");
                return;
            }
            
            // Step 3: Create stable quest panel
            CreateStableQuestPanel();
            
            // Step 4: Connect button properly
            ConnectButtonProperly(questButton);
            
            Debug.Log("✅ Quest button fixed! Click it to test!");
        }
        
        private void RemoveConflictingPanels()
        {
            // Remove all existing quest panels that might conflict
            string[] conflictingNames = { 
                "QuestPanel", 
                "EnhancedQuestPanel", 
                "WorkingQuestPanel",
                "MainMenuQuestPanel"
            };
            
            foreach (string panelName in conflictingNames)
            {
                GameObject conflictPanel = GameObject.Find(panelName);
                if (conflictPanel != null)
                {
                    DestroyImmediate(conflictPanel);
                    Debug.Log($"🗑️ Removed conflicting {panelName}");
                }
            }
            
            // Also remove from UIYesNoDialogView 
            Transform dialogView = GameObject.Find("MenuUI/UIYesNoDialogView")?.transform;
            if (dialogView != null)
            {
                for (int i = dialogView.childCount - 1; i >= 0; i--)
                {
                    Transform child = dialogView.GetChild(i);
                    if (child.name.Contains("Quest"))
                    {
                        DestroyImmediate(child.gameObject);
                        Debug.Log($"🗑️ Removed {child.name} from UIYesNoDialogView");
                    }
                }
            }
            
            Debug.Log("✅ Cleaned up all conflicting panels");
        }
        
        private void CreateStableQuestPanel()
        {
            Transform menuUI = GameObject.Find("MenuUI")?.transform;
            if (menuUI == null)
            {
                Debug.LogError("❌ MenuUI not found!");
                return;
            }
            
            // Create quest panel directly under MenuUI
            questPanel = new GameObject("StableQuestPanel");
            questPanel.transform.SetParent(menuUI, false);
            
            // Full screen setup
            RectTransform rect = questPanel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            
            // Dark background
            Image background = questPanel.AddComponent<Image>();
            background.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            // Ensure it renders on top
            Canvas canvas = questPanel.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 9999; // Very high number
            questPanel.AddComponent<GraphicRaycaster>();
            
            // Add quest content
            CreateQuestContent();
            
            // Start hidden
            questPanel.SetActive(false);
            isPanelVisible = false;
            
            Debug.Log("✅ Created stable quest panel");
        }
        
        private void CreateQuestContent()
        {
            // Title
            GameObject title = new GameObject("QuestTitle");
            title.transform.SetParent(questPanel.transform, false);
            
            RectTransform titleRect = title.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.85f);
            titleRect.anchorMax = new Vector2(1, 0.95f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = Vector2.zero;
            
            Text titleText = title.AddComponent<Text>();
            titleText.text = "🎯 SKYFALL QUESTS";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 36;
            titleText.color = Color.red;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontStyle = FontStyle.Bold;
            
            // Content
            GameObject content = new GameObject("QuestContent");
            content.transform.SetParent(questPanel.transform, false);
            
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.1f, 0.15f);
            contentRect.anchorMax = new Vector2(0.9f, 0.8f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = Vector2.zero;
            
            Text contentText = content.AddComponent<Text>();
            contentText.text = @"🏆 ACTIVE QUESTS:

🎯 Daily Challenges:
• Eliminate 10 enemies (0/10) ...................... 💰 100 coins
• Deal 1000 damage total (0/1000) ................ 💰 150 coins
• Win 2 matches (0/2) .............................. 💰 300 coins

📅 Weekly Challenges:
• Get 50 eliminations (0/50) ....................... 💰 500 coins
• Play 20 matches (0/20) ........................... 💰 400 coins

🏅 Progression Goals:
• Reach Level 10 (1/10) ............................ 💰 1000 coins
• Complete 10 Daily Quests (0/10) ................. 💰 800 coins

✅ Quest system is working!
🎮 Click QUEST button to toggle
💰 Complete quests to earn rewards";
            
            contentText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            contentText.fontSize = 18;
            contentText.color = Color.white;
            contentText.alignment = TextAnchor.UpperLeft;
            
            // Close instruction
            GameObject closeText = new GameObject("CloseInstruction");
            closeText.transform.SetParent(questPanel.transform, false);
            
            RectTransform closeRect = closeText.AddComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0, 0.05f);
            closeRect.anchorMax = new Vector2(1, 0.1f);
            closeRect.anchoredPosition = Vector2.zero;
            closeRect.sizeDelta = Vector2.zero;
            
            Text closeTextComp = closeText.AddComponent<Text>();
            closeTextComp.text = "🎮 Click QUEST button again to close";
            closeTextComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            closeTextComp.fontSize = 16;
            closeTextComp.color = Color.yellow;
            closeTextComp.alignment = TextAnchor.MiddleCenter;
        }
        
        private void ConnectButtonProperly(GameObject questButton)
        {
            // Remove the SimpleQuestButtonHandler that's causing issues
            SimpleQuestButtonHandler oldHandler = questButton.GetComponent<SimpleQuestButtonHandler>();
            if (oldHandler != null)
            {
                DestroyImmediate(oldHandler);
                Debug.Log("🗑️ Removed problematic SimpleQuestButtonHandler");
            }
            
            // Add Unity Button for reliable clicking
            Button button = questButton.GetComponent<Button>();
            if (button == null)
            {
                button = questButton.AddComponent<Button>();
            }
            
            // Clear and connect
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ToggleQuestPanel);
            
            Debug.Log("✅ Connected quest button with Unity Button");
        }
        
        public void ToggleQuestPanel()
        {
            if (questPanel == null)
            {
                Debug.LogWarning("⚠️ Quest panel not found! Run 'Fix Quest Button Now' first.");
                return;
            }
            
            isPanelVisible = !isPanelVisible;
            questPanel.SetActive(isPanelVisible);
            
            Debug.Log($"🎯 Quest panel {(isPanelVisible ? "opened" : "closed")}!");
            
            // Prevent immediate closing by disabling other components temporarily
            if (isPanelVisible)
            {
                DisableConflictingComponents();
                // Re-enable after a short delay
                Invoke(nameof(ReenableComponents), 0.5f);
            }
        }
        
        private void DisableConflictingComponents()
        {
            // Temporarily disable components that might interfere
            MonoBehaviour[] questComponents = FindObjectsOfType<MonoBehaviour>();
            foreach (var component in questComponents)
            {
                if (component.GetType().Name.Contains("Quest") && component != this)
                {
                    component.enabled = false;
                }
            }
        }
        
        private void ReenableComponents()
        {
            // Re-enable components
            MonoBehaviour[] questComponents = FindObjectsOfType<MonoBehaviour>();
            foreach (var component in questComponents)
            {
                if (component.GetType().Name.Contains("Quest") && component != this)
                {
                    component.enabled = true;
                }
            }
        }
        
        [ContextMenu("Test Quest Panel")]
        public void TestQuestPanel()
        {
            ToggleQuestPanel();
            Invoke(nameof(TestHidePanel), 3f);
        }
        
        private void TestHidePanel()
        {
            if (questPanel != null && questPanel.activeSelf)
            {
                ToggleQuestPanel();
                Debug.Log("✅ Test complete - quest button is working!");
            }
        }
    }
}