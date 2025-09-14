using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TPSBR
{
    /// <summary>
    /// Simple handler for quest button - adds Unity Button if needed and handles clicks
    /// </summary>
    public class SimpleQuestButtonHandler : MonoBehaviour, IPointerClickHandler
    {
        [Header("🎯 Simple Quest Button Handler")]
        public bool debugMode = true;
        
        private void Start()
        {
            // Ensure this button is actually clickable
            EnsureButtonIsClickable();
        }
        
        private void EnsureButtonIsClickable()
        {
            // Check if we have any button component
            Button unityButton = GetComponent<Button>();
            TPSBR.UI.UIButton tpsbrButton = GetComponent<TPSBR.UI.UIButton>();
            
            if (unityButton == null && tpsbrButton == null)
            {
                // Add Unity Button to make it clickable
                Button newButton = gameObject.AddComponent<Button>();
                newButton.onClick.AddListener(ToggleQuests);
                
                if (debugMode)
                {
                    Debug.Log("✅ Added Unity Button to make QuestButton clickable");
                }
            }
            else if (unityButton != null)
            {
                // Connect existing Unity Button
                unityButton.onClick.RemoveAllListeners();
                unityButton.onClick.AddListener(ToggleQuests);
                
                if (debugMode)
                {
                    Debug.Log("✅ Connected existing Unity Button");
                }
            }
            
            // Ensure we can receive pointer events
            if (GetComponent<GraphicRaycaster>() == null)
            {
                Canvas parentCanvas = GetComponentInParent<Canvas>();
                if (parentCanvas != null && parentCanvas.GetComponent<GraphicRaycaster>() == null)
                {
                    parentCanvas.gameObject.AddComponent<GraphicRaycaster>();
                    
                    if (debugMode)
                    {
                        Debug.Log("✅ Added GraphicRaycaster to parent canvas for click detection");
                    }
                }
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (debugMode)
            {
                Debug.Log("🎯 Quest button clicked via IPointerClickHandler!");
            }
            
            ToggleQuestPanel();
        }
        
        public void ToggleQuests()
        {
            if (debugMode)
            {
                Debug.Log("🎯 Quest button clicked via Unity Button!");
            }
            
            ToggleQuestPanel();
        }
        
        private void ToggleQuestPanel()
        {
            // Try to find existing quest panels first
            GameObject panel = GameObject.Find("QuestPanel");
            if (panel == null)
                panel = GameObject.Find("EnhancedQuestPanel");
            if (panel == null)
                panel = GameObject.Find("WorkingQuestPanel");
            
            if (panel != null)
            {
                bool isVisible = panel.activeSelf;
                panel.SetActive(!isVisible);
                
                if (debugMode)
                {
                    Debug.Log($"🎯 Quest panel {(panel.activeSelf ? "opened" : "closed")}!");
                }
            }
            else
            {
                // Use the existing QuestUISetup to create enhanced quest system
                QuestUISetup questSetup = FindObjectOfType<QuestUISetup>();
                if (questSetup != null)
                {
                    if (debugMode)
                    {
                        Debug.Log("🔧 Creating quest panel using existing QuestUISetup...");
                    }
                    questSetup.CreateEnhancedQuestSystem();
                }
                else
                {
                    Debug.LogWarning("⚠️ No quest panel found and no QuestUISetup available!");
                }
            }
        }
        
        private void CreateQuickQuestPanel()
        {
            Transform menuUI = GameObject.Find("MenuUI")?.transform;
            if (menuUI == null) return;
            
            GameObject panel = new GameObject("WorkingQuestPanel");
            panel.transform.SetParent(menuUI, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            Canvas canvas = panel.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1000;
            panel.AddComponent<GraphicRaycaster>();
            
            // Add simple text
            GameObject text = new GameObject("QuestText");
            text.transform.SetParent(panel.transform, false);
            
            RectTransform textRect = text.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            Text questText = text.AddComponent<Text>();
            questText.text = "🎯 QUEST PANEL WORKING!\n\nClick quest button again to close.\n\nThis panel was created automatically.";
            questText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            questText.fontSize = 24;
            questText.color = Color.white;
            questText.alignment = TextAnchor.MiddleCenter;
            
            if (debugMode)
            {
                Debug.Log("✅ Created quick quest panel - your button should now work!");
            }
        }
        
        [ContextMenu("Test Quest Toggle")]
        public void TestQuestToggle()
        {
            ToggleQuestPanel();
        }
        
        [ContextMenu("Make Button Clickable")]
        public void MakeButtonClickable()
        {
            EnsureButtonIsClickable();
        }
    }
}