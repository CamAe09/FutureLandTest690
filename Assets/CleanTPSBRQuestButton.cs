using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace TPSBR
{
    /// <summary>
    /// Clean TPSBR Quest Button - with proper Unity Input System escape key handling
    /// </summary>
    public class CleanTPSBRQuestButton : MonoBehaviour
    {
        [Header("🎯 TPSBR Quest Button")]
        [SerializeField] private bool debugMode = true;

        private Button button;
        private GameObject questPanel;
        private bool isQuestPanelOpen = false;

        private void Start()
        {
            // Get the Unity Button component
            button = GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("❌ No Button component found! Make sure this is on your QuestButton GameObject.");
                return;
            }

            // Connect the button click to our quest method
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnQuestButtonClicked);

            // Clean up any problematic panels first
            CleanupProblematicPanels();

            // Find the existing quest panel in the scene
            FindExistingQuestPanel();

            if (debugMode)
            {
                Debug.Log("✅ Clean TPSBR Quest Button connected and ready!");
            }
        }

        private void CleanupProblematicPanels()
        {
            // Remove any EnhancedQuestPanel objects that might cause conflicts
            GameObject[] problematicPanels = GameObject.FindGameObjectsWithTag("Untagged");
            int cleanedCount = 0;

            foreach (GameObject obj in problematicPanels)
            {
                if (obj.name == "EnhancedQuestPanel" || obj.name == "QuestPanel" || obj.name == "StableQuestPanel")
                {
                    DestroyImmediate(obj);
                    cleanedCount++;
                }
            }

            if (cleanedCount > 0 && debugMode)
            {
                Debug.Log($"🗑️ Cleaned up {cleanedCount} problematic quest panels");
            }
        }

        private void FindExistingQuestPanel()
        {
            // Look for the MainMenuQuestPanel that already exists in the scene
            Transform menuUI = GameObject.Find("MenuUI")?.transform;
            if (menuUI != null)
            {
                Transform mainMenuView = menuUI.Find("UIMainMenuView");
                if (mainMenuView != null)
                {
                    Transform existingPanel = mainMenuView.Find("MainMenuQuestPanel");
                    if (existingPanel != null)
                    {
                        questPanel = existingPanel.gameObject;

                        // Make sure it's properly configured
                        SetupExistingQuestPanel();

                        if (debugMode)
                        {
                            Debug.Log("✅ Found and configured MainMenuQuestPanel!");
                        }
                        return;
                    }
                }
            }

            // If no existing panel found, create a simple working panel
            CreateSimpleQuestPanel();
        }

        private void SetupExistingQuestPanel()
        {
            if (questPanel == null) return;

            // Make sure it has the basic components it needs
            Canvas canvas = questPanel.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = questPanel.AddComponent<Canvas>();
            }

            // Configure canvas for proper display
            canvas.overrideSorting = true;
            canvas.sortingOrder = 100; // Higher than menu UI

            // Make sure it starts hidden
            questPanel.SetActive(false);
            isQuestPanelOpen = false;

            if (debugMode)
            {
                Debug.Log("🎯 Existing quest panel configured and ready!");
            }
        }

        private void CreateSimpleQuestPanel()
        {
            // Create a simple working quest panel without using QuestUISetup
            Transform menuUI = GameObject.Find("MenuUI")?.transform;
            if (menuUI == null)
            {
                Debug.LogError("❌ MenuUI not found! Cannot create quest panel.");
                return;
            }

            try
            {
                // Create main panel
                questPanel = new GameObject("WorkingQuestPanel");
                questPanel.transform.SetParent(menuUI, false);

                // Add Canvas component
                Canvas canvas = questPanel.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = 100;

                // Add CanvasGroup for easy show/hide
                CanvasGroup canvasGroup = questPanel.AddComponent<CanvasGroup>();

                // Setup RectTransform for fullscreen
                RectTransform rect = questPanel.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;

                // Add background
                Image bg = questPanel.AddComponent<Image>();
                bg.color = new Color(0, 0, 0, 0.8f);

                // Create simple content
                CreateSimpleQuestContent();

                // Start hidden
                questPanel.SetActive(false);
                isQuestPanelOpen = false;

                if (debugMode)
                {
                    Debug.Log("✅ Created simple working quest panel!");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating simple quest panel: {e.Message}");
            }
        }

        private void CreateSimpleQuestContent()
        {
            // Create simple title
            GameObject title = new GameObject("QuestTitle");
            title.transform.SetParent(questPanel.transform, false);

            RectTransform titleRect = title.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.8f);
            titleRect.anchorMax = new Vector2(0.5f, 0.8f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(400, 60);

            UnityEngine.UI.Text titleText = title.AddComponent<UnityEngine.UI.Text>();
            titleText.text = "🎯 QUESTS";
            titleText.fontSize = 36;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Create simple message
            GameObject message = new GameObject("QuestMessage");
            message.transform.SetParent(questPanel.transform, false);

            RectTransform msgRect = message.AddComponent<RectTransform>();
            msgRect.anchorMin = new Vector2(0.5f, 0.5f);
            msgRect.anchorMax = new Vector2(0.5f, 0.5f);
            msgRect.anchoredPosition = Vector2.zero;
            msgRect.sizeDelta = new Vector2(800, 300);  // Wider and taller


            UnityEngine.UI.Text msgText = message.AddComponent<UnityEngine.UI.Text>();
            msgText.text = "Quest system is ready!\n\nTo set up quests:...";
            msgText.fontSize = 18;
            msgText.color = Color.white;
            msgText.alignment = TextAnchor.MiddleLeft;  // Changed from MiddleCenter
            msgText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            msgText.horizontalOverflow = HorizontalWrapMode.Wrap;  // Add this line
            msgText.verticalOverflow = VerticalWrapMode.Overflow;    // Add this line


            // Create close button
            CreateSimpleCloseButton();
        }

        private void CreateSimpleCloseButton()
        {
            GameObject closeBtn = new GameObject("CloseButton");
            closeBtn.transform.SetParent(questPanel.transform, false);

            RectTransform btnRect = closeBtn.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(1, 1);
            btnRect.anchorMax = new Vector2(1, 1);
            btnRect.anchoredPosition = new Vector2(-50, -50);
            btnRect.sizeDelta = new Vector2(80, 80);

            Image btnImage = closeBtn.AddComponent<Image>();
            btnImage.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);

            Button button = closeBtn.AddComponent<Button>();
            button.targetGraphic = btnImage;
            button.onClick.AddListener(() => {
                CloseQuestPanel();
            });

            // Add X text
            GameObject btnText = new GameObject("ButtonText");
            btnText.transform.SetParent(closeBtn.transform, false);

            RectTransform textRect = btnText.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = Vector2.zero;

            UnityEngine.UI.Text text = btnText.AddComponent<UnityEngine.UI.Text>();
            text.text = "✕";
            text.fontSize = 36;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        public void OnQuestButtonClicked()
        {
            if (debugMode)
            {
                Debug.Log("🎯 TPSBR Quest Button clicked!");
            }

            // Find the quest panel if we don't have it
            if (questPanel == null)
            {
                FindExistingQuestPanel();
            }

            // Toggle the quest panel
            if (questPanel != null)
            {
                ToggleQuestPanel();
            }
            else
            {
                Debug.LogWarning("⚠️ No quest panel found to display!");
            }
        }

        private void OpenQuestPanel()
        {
            if (questPanel == null) return;

            isQuestPanelOpen = true;
            questPanel.SetActive(true);

            if (debugMode)
            {
                Debug.Log("🎯 Quest panel OPENED - should be VISIBLE now!");
            }
        }

        private void CloseQuestPanel()
        {
            if (questPanel == null) return;

            isQuestPanelOpen = false;
            questPanel.SetActive(false);

            if (debugMode)
            {
                Debug.Log("🎯 Quest panel CLOSED - should be HIDDEN now!");
            }
        }

        private void ToggleQuestPanel()
        {
            if (isQuestPanelOpen)
            {
                CloseQuestPanel();
            }
            else
            {
                OpenQuestPanel();
            }
        }

        // Handle escape key input using proper Unity Input System
        private void Update()
        {
            // Check if quest panel is open and escape key was pressed
            if (isQuestPanelOpen && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseQuestPanel();

                if (debugMode)
                {
                    Debug.Log("🎯 Quest panel closed with Escape key using Unity Input System");
                }
            }
        }

        // Public method to check if quest UI is blocking other inputs
        public bool IsQuestUIOpen()
        {
            return isQuestPanelOpen;
        }

        // Public method to close quest panel from external systems
        public void CloseQuestPanelExternal()
        {
            if (isQuestPanelOpen)
            {
                CloseQuestPanel();
            }
        }

        // For testing in inspector
        [ContextMenu("Test Quest Button")]
        public void TestQuestButton()
        {
            OnQuestButtonClicked();
        }

        [ContextMenu("Force Close Quest Panel")]
        public void ForceCloseQuestPanel()
        {
            CloseQuestPanel();
            Debug.Log("🎯 Quest panel forcefully closed!");
        }

        [ContextMenu("Find Quest Panel Now")]
        public void FindQuestPanelNow()
        {
            FindExistingQuestPanel();
            Debug.Log("🔍 Quest panel search completed!");
        }

        [ContextMenu("Clean Up All Quest Panels")]
        public void CleanUpAllQuestPanels()
        {
            CleanupProblematicPanels();
            Debug.Log("🗑️ All problematic quest panels cleaned up!");
        }
    }
}
