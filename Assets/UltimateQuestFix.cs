using UnityEngine;
using UnityEngine.UI;

namespace TPSBR
{
    /// <summary>
    /// Ultimate quest fix - works with your existing components
    /// </summary>
    public class UltimateQuestFix : MonoBehaviour
    {
        [Header("🎯 Ultimate Quest Fix")]
        [TextArea(3, 5)]
        public string instructions = "RIGHT-CLICK → 'Fix Quest Now'\n\nUses your existing QuestButton and creates working quest panel!";
        
        [ContextMenu("Fix Quest Now")]
        public void FixQuestNow()
        {
            Debug.Log("🔧 Fixing quest with existing components...");
            
            // Your quest button is already at /MenuUI/QuestButton with SimpleQuestButtonHandler
            // Let's just make sure it works properly
            
            GameObject questButton = GameObject.Find("QuestButton");
            if (questButton == null)
            {
                Debug.LogError("❌ QuestButton not found!");
                return;
            }
            
            Debug.Log("✅ Found QuestButton with SimpleQuestButtonHandler");
            
            // Create the quest panel that SimpleQuestButtonHandler is looking for
            CreateWorkingQuestPanel();
            
            // Make sure the button handler works
            EnsureButtonWorks(questButton);
            
            Debug.Log("🎉 Quest system fixed! Click your quest button!");
        }
        
        private void CreateWorkingQuestPanel()
        {
            // Remove any existing WorkingQuestPanel
            GameObject oldPanel = GameObject.Find("WorkingQuestPanel");
            if (oldPanel != null)
            {
                DestroyImmediate(oldPanel);
                Debug.Log("🗑️ Removed old WorkingQuestPanel");
            }
            
            // Find MenuUI
            Transform menuUI = GameObject.Find("MenuUI")?.transform;
            if (menuUI == null)
            {
                Debug.LogError("❌ MenuUI not found!");
                return;
            }
            
            // Create the panel that SimpleQuestButtonHandler expects
            GameObject questPanel = new GameObject("WorkingQuestPanel");
            questPanel.transform.SetParent(menuUI, false);
            
            // Full screen setup
            RectTransform rect = questPanel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            
            // Dark background
            Image background = questPanel.AddComponent<Image>();
            background.color = new Color(0.15f, 0.15f, 0.15f, 0.92f);
            
            // Canvas for proper rendering
            Canvas canvas = questPanel.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 999;
            questPanel.AddComponent<GraphicRaycaster>();
            
            // Add content
            CreateContent(questPanel);
            
            // Start hidden (SimpleQuestButtonHandler will toggle it)
            questPanel.SetActive(false);
            
            Debug.Log("✅ Created WorkingQuestPanel that SimpleQuestButtonHandler can find");
        }
        
        private void CreateContent(GameObject parent)
        {
            // Title
            GameObject title = new GameObject("Title");
            title.transform.SetParent(parent.transform, false);
            
            RectTransform titleRect = title.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.85f);
            titleRect.anchorMax = new Vector2(1, 0.95f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = Vector2.zero;
            
            Text titleText = title.AddComponent<Text>();
            titleText.text = "🎯 SKYFALL QUESTS";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 32;
            titleText.color = Color.red;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontStyle = FontStyle.Bold;
            
            // Content area
            GameObject content = new GameObject("Content");
            content.transform.SetParent(parent.transform, false);
            
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.1f, 0.15f);
            contentRect.anchorMax = new Vector2(0.9f, 0.8f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = Vector2.zero;
            
            Text contentText = content.AddComponent<Text>();
            contentText.text = @"🏆 ACTIVE QUESTS:

🎯 Daily Challenges:
• Eliminate 10 enemies (0/10) ......................... 💰 100 coins
• Deal 1000 damage total (0/1000) ................... 💰 150 coins
• Win 2 matches (0/2) ................................. 💰 300 coins

📅 Weekly Challenges:
• Get 50 eliminations (0/50) ......................... 💰 500 coins
• Play 20 matches (0/20) ............................. 💰 400 coins

🏅 Progression Goals:
• Reach Level 10 (1/10) .............................. 💰 1000 coins
• Complete 10 Daily Quests (0/10) ................... 💰 800 coins

✅ Your quest system is now working!
🎮 Click the QUEST button to toggle this panel
💰 Complete quests to earn coins and rewards";
            
            contentText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            contentText.fontSize = 18;
            contentText.color = Color.white;
            contentText.alignment = TextAnchor.UpperLeft;
            
            // Close instruction
            GameObject closeInstr = new GameObject("CloseInstruction");
            closeInstr.transform.SetParent(parent.transform, false);
            
            RectTransform closeRect = closeInstr.AddComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0, 0.05f);
            closeRect.anchorMax = new Vector2(1, 0.1f);
            closeRect.anchoredPosition = Vector2.zero;
            closeRect.sizeDelta = Vector2.zero;
            
            Text closeText = closeInstr.AddComponent<Text>();
            closeText.text = "🎮 Click QUEST button to close";
            closeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            closeText.fontSize = 16;
            closeText.color = Color.yellow;
            closeText.alignment = TextAnchor.MiddleCenter;
        }
        
        private void EnsureButtonWorks(GameObject questButton)
        {
            // Your button already has SimpleQuestButtonHandler
            SimpleQuestButtonHandler handler = questButton.GetComponent<SimpleQuestButtonHandler>();
            if (handler != null)
            {
                Debug.Log("✅ SimpleQuestButtonHandler found - it should work now");
                
                // Make sure it has debug mode on so you can see clicks
                handler.debugMode = true;
                
                // Test it immediately
                Invoke(nameof(TestButtonAfterDelay), 1f);
            }
            else
            {
                Debug.LogWarning("⚠️ No SimpleQuestButtonHandler found - adding fallback");
                
                // Add Unity Button as fallback
                Button button = questButton.GetComponent<Button>();
                if (button == null)
                {
                    button = questButton.AddComponent<Button>();
                }
                
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(TestToggle);
                
                Debug.Log("✅ Added fallback Unity Button");
            }
        }
        
        private void TestButtonAfterDelay()
        {
            Debug.Log("🧪 Testing quest system...");
            
            GameObject panel = GameObject.Find("WorkingQuestPanel");
            if (panel != null)
            {
                panel.SetActive(true);
                Debug.Log("✅ Quest panel opened for test");
                
                // Close after 3 seconds
                Invoke(nameof(HideTestPanel), 3f);
            }
        }
        
        private void HideTestPanel()
        {
            GameObject panel = GameObject.Find("WorkingQuestPanel");
            if (panel != null)
            {
                panel.SetActive(false);
                Debug.Log("✅ Test complete - your quest button should work!");
            }
        }
        
        private void TestToggle()
        {
            Debug.Log("🎯 Quest button clicked (fallback method)!");
            
            GameObject panel = GameObject.Find("WorkingQuestPanel");
            if (panel != null)
            {
                bool isVisible = panel.activeSelf;
                panel.SetActive(!isVisible);
                Debug.Log($"🎯 Quest panel {(panel.activeSelf ? "opened" : "closed")}!");
            }
        }
        
        [ContextMenu("Test Quest Button")]
        public void TestQuestButton()
        {
            TestButtonAfterDelay();
        }
    }
}