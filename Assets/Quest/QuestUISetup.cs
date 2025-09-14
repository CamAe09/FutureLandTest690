using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace TPSBR
{
    public class QuestUISetup : MonoBehaviour
    {
        [Header("Quest UI Settings")]
        [SerializeField] private bool debugMode = true;

        private GameObject questPanel;
        private Transform questContainer;
        private GameObject questItemPrefab;

        // Much safer null checking for Unity objects
        private bool IsValidTransform(Transform t)
        {
            try
            {
                return t != null && t.gameObject != null && t.gameObject.activeInHierarchy != null;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidGameObject(GameObject go)
        {
            try
            {
                return go != null && go.activeInHierarchy != null;
            }
            catch
            {
                return false;
            }
        }

        public void CreateEnhancedQuestSystem()
        {
            if (debugMode)
            {
                Debug.Log("🎯 Creating Enhanced Quest System...");
            }

            try
            {
                SafeCleanupExistingPanels();
                CreateQuestPanel();

                if (IsValidGameObject(questPanel))
                {
                    CreateQuestHeader();
                    CreateQuestContainer();
                    CreateQuestItemPrefab();
                    CreateCloseButton();
                    AddQuestUIManager();
                }

                if (debugMode)
                {
                    Debug.Log("✅ Enhanced Quest System Created Successfully!");
                    Debug.Log("📝 Instructions:");
                    Debug.Log("   1. Create quest assets with Right-click → Create → TPSBR → Quest");
                    Debug.Log("   2. Add quests to QuestManager's Available Quests list");
                    Debug.Log("   3. Click the Quest button to open the quest panel");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating quest system: {e.Message}");
                Debug.LogError($"Stack trace: {e.StackTrace}");
            }
        }

        private void SafeCleanupExistingPanels()
        {
            string[] conflictingNames = {
                "EnhancedQuestPanel",
                "QuestPanel",
                "MainMenuQuestPanel",
                "StableQuestPanel"
            };

            // Safer cleanup - collect all objects first, then destroy
            System.Collections.Generic.List<GameObject> objectsToDestroy = new System.Collections.Generic.List<GameObject>();

            // Find all conflicting panels in the scene
            foreach (string panelName in conflictingNames)
            {
                try
                {
                    GameObject existingPanel = GameObject.Find(panelName);
                    if (existingPanel != null)
                    {
                        objectsToDestroy.Add(existingPanel);
                    }
                }
                catch (System.Exception e)
                {
                    if (debugMode)
                    {
                        Debug.LogWarning($"⚠️ Error finding panel {panelName}: {e.Message}");
                    }
                }
            }

            // Also check in MenuUI
            try
            {
                GameObject menuUIObj = GameObject.Find("MenuUI");
                if (menuUIObj != null && menuUIObj.transform != null)
                {
                    foreach (string panelName in conflictingNames)
                    {
                        try
                        {
                            Transform existingPanel = menuUIObj.transform.Find(panelName);
                            if (existingPanel != null && existingPanel.gameObject != null)
                            {
                                objectsToDestroy.Add(existingPanel.gameObject);
                            }
                        }
                        catch (System.Exception e)
                        {
                            if (debugMode)
                            {
                                Debug.LogWarning($"⚠️ Error finding panel {panelName} in MenuUI: {e.Message}");
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                if (debugMode)
                {
                    Debug.LogWarning($"⚠️ Error accessing MenuUI: {e.Message}");
                }
            }

            // Now safely destroy all collected objects
            foreach (GameObject obj in objectsToDestroy)
            {
                try
                {
                    if (obj != null)
                    {
                        string objName = obj.name; // Get name before destruction
                        DestroyImmediate(obj);
                        if (debugMode)
                        {
                            Debug.Log($"🗑️ Removed existing {objName}");
                        }
                    }
                }
                catch (System.Exception e)
                {
                    if (debugMode)
                    {
                        Debug.LogWarning($"⚠️ Error destroying object: {e.Message}");
                    }
                }
            }

            // Clear references to avoid stale references
            questPanel = null;
            questContainer = null;
            questItemPrefab = null;
        }

        private void CreateQuestPanel()
        {
            Transform menuUI = FindUIRoot();
            if (menuUI == null)
            {
                Debug.LogError("❌ Cannot create quest panel - no valid UI root found!");
                return;
            }

            try
            {
                questPanel = new GameObject("EnhancedQuestPanel");
                questPanel.transform.SetParent(menuUI, false);

                Canvas canvas = questPanel.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.overrideSorting = true;
                canvas.sortingOrder = 1000;

                CanvasScaler scaler = questPanel.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);

                questPanel.AddComponent<GraphicRaycaster>();

                RectTransform rect = questPanel.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;

                Image bg = questPanel.AddComponent<Image>();
                bg.color = new Color(0, 0, 0, 0.85f);

                questPanel.SetActive(false);

                if (debugMode)
                {
                    Debug.Log("✅ Quest panel created with proper Canvas settings");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating quest panel: {e.Message}");
                questPanel = null;
            }
        }

        private Transform FindUIRoot()
        {
            try
            {
                GameObject menuUIObj = GameObject.Find("MenuUI");
                if (menuUIObj != null && menuUIObj.transform != null)
                {
                    return menuUIObj.transform;
                }
            }
            catch (System.Exception e)
            {
                if (debugMode)
                {
                    Debug.LogWarning($"⚠️ Error finding MenuUI: {e.Message}");
                }
            }

            // Create fallback canvas
            try
            {
                GameObject canvas = new GameObject("Quest Canvas");
                Canvas canvasComp = canvas.AddComponent<Canvas>();
                canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasComp.sortingOrder = 1000;

                CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);

                canvas.AddComponent<GraphicRaycaster>();

                return canvas.transform;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating fallback canvas: {e.Message}");
                return null;
            }
        }

        private void CreateQuestHeader()
        {
            if (!IsValidGameObject(questPanel))
            {
                Debug.LogError("❌ Quest panel is null when trying to create header!");
                return;
            }

            try
            {
                GameObject header = new GameObject("Quest Header");
                header.transform.SetParent(questPanel.transform, false);

                RectTransform rect = header.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0.9f);
                rect.anchorMax = new Vector2(1, 1);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                Image headerBg = header.AddComponent<Image>();
                headerBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

                GameObject headerText = new GameObject("Header Text");
                headerText.transform.SetParent(header.transform, false);

                RectTransform textRect = headerText.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;

                TextMeshProUGUI text = headerText.AddComponent<TextMeshProUGUI>();
                text.text = "🎯 BATTLE ROYALE QUESTS";
                text.fontSize = 36;
                text.fontStyle = FontStyles.Bold;
                text.color = Color.white;
                text.alignment = TextAlignmentOptions.Center;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating quest header: {e.Message}");
            }
        }

        private void CreateQuestContainer()
        {
            if (!IsValidGameObject(questPanel))
            {
                Debug.LogError("❌ Quest panel is null when trying to create container!");
                return;
            }

            try
            {
                GameObject scrollArea = new GameObject("Quest Scroll Area");
                scrollArea.transform.SetParent(questPanel.transform, false);

                RectTransform scrollRect = scrollArea.AddComponent<RectTransform>();
                scrollRect.anchorMin = new Vector2(0.1f, 0.1f);
                scrollRect.anchorMax = new Vector2(0.9f, 0.85f);
                scrollRect.offsetMin = Vector2.zero;
                scrollRect.offsetMax = Vector2.zero;

                ScrollRect scroll = scrollArea.AddComponent<ScrollRect>();
                Image scrollBg = scrollArea.AddComponent<Image>();
                scrollBg.color = new Color(0.05f, 0.05f, 0.05f, 0.8f);

                GameObject viewport = new GameObject("Viewport");
                viewport.transform.SetParent(scrollArea.transform, false);

                RectTransform viewportRect = viewport.AddComponent<RectTransform>();
                viewportRect.anchorMin = Vector2.zero;
                viewportRect.anchorMax = Vector2.one;
                viewportRect.offsetMin = Vector2.zero;
                viewportRect.offsetMax = Vector2.zero;

                Image viewportImage = viewport.AddComponent<Image>();
                viewportImage.color = Color.clear;
                Mask viewportMask = viewport.AddComponent<Mask>();
                viewportMask.showMaskGraphic = false;

                questContainer = new GameObject("Quest Container").transform;
                questContainer.SetParent(viewport.transform, false);

                RectTransform containerRect = questContainer.GetComponent<RectTransform>();
                if (containerRect == null)
                    containerRect = questContainer.gameObject.AddComponent<RectTransform>();

                containerRect.anchorMin = new Vector2(0, 1);
                containerRect.anchorMax = new Vector2(1, 1);
                containerRect.anchoredPosition = Vector2.zero;
                containerRect.sizeDelta = new Vector2(0, 0);

                VerticalLayoutGroup layout = questContainer.gameObject.AddComponent<VerticalLayoutGroup>();
                layout.spacing = 15;
                layout.padding = new RectOffset(20, 20, 20, 20);
                layout.childAlignment = TextAnchor.UpperCenter;
                layout.childControlHeight = false;
                layout.childControlWidth = true;
                layout.childForceExpandHeight = false;
                layout.childForceExpandWidth = true;

                ContentSizeFitter fitter = questContainer.gameObject.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                scroll.content = containerRect;
                scroll.viewport = viewportRect;
                scroll.horizontal = false;
                scroll.vertical = true;
                scroll.movementType = ScrollRect.MovementType.Clamped;
                scroll.scrollSensitivity = 40f;

                if (debugMode)
                {
                    Debug.Log("✅ Quest container with scroll view created");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating quest container: {e.Message}");
            }
        }

        private void CreateQuestItemPrefab()
        {
            try
            {
                questItemPrefab = new GameObject("Quest Item Prefab");

                RectTransform rect = questItemPrefab.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(0, 120);

                Image bg = questItemPrefab.AddComponent<Image>();
                bg.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

                HorizontalLayoutGroup layout = questItemPrefab.AddComponent<HorizontalLayoutGroup>();
                layout.spacing = 15;
                layout.padding = new RectOffset(15, 15, 10, 10);
                layout.childAlignment = TextAnchor.MiddleLeft;
                layout.childControlHeight = false;
                layout.childControlWidth = false;
                layout.childForceExpandHeight = true;
                layout.childForceExpandWidth = false;

                questItemPrefab.AddComponent<QuestItemUI>();

                if (debugMode)
                {
                    Debug.Log("✅ Quest item prefab created");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating quest item prefab: {e.Message}");
            }
        }

        private void CreateCloseButton()
        {
            if (!IsValidGameObject(questPanel))
            {
                Debug.LogError("❌ Quest panel is null when trying to create close button!");
                return;
            }

            try
            {
                GameObject closeBtn = new GameObject("Close Button");
                closeBtn.transform.SetParent(questPanel.transform, false);

                RectTransform rect = closeBtn.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.anchoredPosition = new Vector2(-50, -50);
                rect.sizeDelta = new Vector2(80, 80);

                Button button = closeBtn.AddComponent<Button>();
                Image buttonImage = closeBtn.AddComponent<Image>();
                buttonImage.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);

                GameObject buttonText = new GameObject("Close Text");
                buttonText.transform.SetParent(closeBtn.transform, false);

                RectTransform textRect = buttonText.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;

                TextMeshProUGUI text = buttonText.AddComponent<TextMeshProUGUI>();
                text.text = "✕";
                text.fontSize = 36;
                text.fontStyle = FontStyles.Bold;
                text.color = Color.white;
                text.alignment = TextAlignmentOptions.Center;

                button.onClick.AddListener(() => {
                    if (IsValidGameObject(questPanel))
                    {
                        questPanel.SetActive(false);
                        if (debugMode)
                        {
                            Debug.Log("🎯 Quest panel closed");
                        }
                    }
                });
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating close button: {e.Message}");
            }
        }

        private void AddQuestUIManager()
        {
            if (!IsValidGameObject(questPanel))
            {
                Debug.LogError("❌ Quest panel is null when trying to add UI manager!");
                return;
            }

            try
            {
                if (questPanel.GetComponent<QuestUIManager>() == null)
                {
                    QuestUIManager uiManager = questPanel.AddComponent<QuestUIManager>();

                    if (debugMode)
                    {
                        Debug.Log("✅ Quest UI Manager added to quest panel");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error adding quest UI manager: {e.Message}");
            }
        }

        [ContextMenu("Create Quest System Now")]
        public void CreateQuestSystemNow()
        {
            CreateEnhancedQuestSystem();
        }
    }
}
