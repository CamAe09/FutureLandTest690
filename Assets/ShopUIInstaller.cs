using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace TPSBR
{
    [System.Serializable]
    public class ShopUIInstaller
    {
        public static void InstallShopUI()
        {
            Debug.Log("🏪 Installing Complete Shop UI System...");
            
            // Create Canvas if needed
            Canvas shopCanvas = CreateOrFindCanvas();
            
            // Create Shop Panel
            GameObject shopPanel = CreateShopPanel(shopCanvas.transform);
            
            // Create all UI elements
            CreateShopHeader(shopPanel.transform);
            CreateCurrencyDisplay(shopPanel.transform);
            Button closeButton = CreateCloseButton(shopPanel.transform);
            Transform itemContainer = CreateItemContainer(shopPanel.transform);
            
            // Create item prefab in project
            GameObject itemPrefab = CreateShopItemPrefab();
            
            // Setup Shop System
            SetupShopSystem(shopPanel, itemContainer, itemPrefab, closeButton);
            
            Debug.Log("✅ Shop UI Installation Complete!");
            Debug.Log("📝 Next Steps:");
            Debug.Log("   1. Create ShopItem assets: Right-click in Project → Create → TPSBR → Shop Item");
            Debug.Log("   2. Assign shop items to ShopManager component");
            Debug.Log("   3. Press B in Play mode to test the shop");
            Debug.Log("   4. Use ShopManager context menu to add test coins");
        }
        
        private static Canvas CreateOrFindCanvas()
        {
            Canvas existingCanvas = GameObject.FindObjectOfType<Canvas>();
            if (existingCanvas != null && existingCanvas.name == "MenuUI")
            {
                return existingCanvas;
            }
            
            GameObject canvasGO = new GameObject("Shop Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Ensure EventSystem exists
            if (GameObject.FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
            
            return canvas;
        }
        
        private static GameObject CreateShopPanel(Transform parent)
        {
            GameObject panel = new GameObject("Shop Panel");
            panel.transform.SetParent(parent, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            panel.SetActive(false); // Start hidden
            
            return panel;
        }
        
        private static void CreateShopHeader(Transform parent)
        {
            GameObject header = new GameObject("Shop Header");
            header.transform.SetParent(parent, false);
            
            RectTransform rect = header.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(0, -40);
            rect.sizeDelta = new Vector2(0, 60);
            
            TextMeshProUGUI text = header.AddComponent<TextMeshProUGUI>();
            text.text = "ITEM SHOP";
            text.fontSize = 42;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
        }
        
        private static void CreateCurrencyDisplay(Transform parent)
        {
            GameObject currencyObj = new GameObject("Currency Display");
            currencyObj.transform.SetParent(parent, false);
            
            RectTransform rect = currencyObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-120, -20);
            rect.sizeDelta = new Vector2(220, 50);
            
            Image bg = currencyObj.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.5f);
            
            TextMeshProUGUI text = currencyObj.AddComponent<TextMeshProUGUI>();
            text.text = "$1,000";
            text.fontSize = 28;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.yellow;
            text.alignment = TextAlignmentOptions.Center;
            
            currencyObj.AddComponent<CurrencyDisplay>();
        }
        
        private static Button CreateCloseButton(Transform parent)
        {
            GameObject closeBtn = new GameObject("Close Button");
            closeBtn.transform.SetParent(parent, false);
            
            RectTransform rect = closeBtn.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-25, -25);
            rect.sizeDelta = new Vector2(40, 40);
            
            Button button = closeBtn.AddComponent<Button>();
            Image buttonImg = closeBtn.AddComponent<Image>();
            buttonImg.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);
            
            // Close button text
            GameObject textObj = new GameObject("X");
            textObj.transform.SetParent(closeBtn.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "×";
            text.fontSize = 28;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            
            return button;
        }
        
        private static Transform CreateItemContainer(Transform parent)
        {
            GameObject container = new GameObject("Items Container");
            container.transform.SetParent(parent, false);
            
            RectTransform rect = container.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.05f, 0.1f);
            rect.anchorMax = new Vector2(0.95f, 0.85f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // Add scroll view
            GameObject scrollView = new GameObject("Scroll View");
            scrollView.transform.SetParent(container.transform, false);
            
            RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
            scrollRect.anchorMin = Vector2.zero;
            scrollRect.anchorMax = Vector2.one;
            scrollRect.offsetMin = Vector2.zero;
            scrollRect.offsetMax = Vector2.zero;
            
            ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
            Image scrollImg = scrollView.AddComponent<Image>();
            scrollImg.color = new Color(0, 0, 0, 0.3f);
            
            // Content area
            GameObject content = new GameObject("Content");
            content.transform.SetParent(scrollView.transform, false);
            
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 600);
            
            // Grid layout for items
            GridLayoutGroup grid = content.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(220, 300);
            grid.spacing = new Vector2(15, 15);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;
            grid.childAlignment = TextAnchor.UpperCenter;
            
            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            scroll.content = contentRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            
            return content.transform;
        }
        
        private static GameObject CreateShopItemPrefab()
        {
            GameObject prefab = new GameObject("Shop Item");
            
            RectTransform rect = prefab.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(220, 300);
            
            // Background
            Image bg = prefab.AddComponent<Image>();
            bg.color = new Color(0.25f, 0.25f, 0.25f, 0.95f);
            
            // Item Icon (top section)
            GameObject icon = CreateChild(prefab, "Item Icon", new Vector2(0.1f, 0.6f), new Vector2(0.9f, 0.95f));
            Image iconImg = icon.AddComponent<Image>();
            iconImg.color = Color.white;
            
            // Item Name
            GameObject nameObj = CreateChild(prefab, "Item Name", new Vector2(0.05f, 0.45f), new Vector2(0.95f, 0.6f));
            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "Item Name";
            nameText.fontSize = 16;
            nameText.fontStyle = FontStyles.Bold;
            nameText.color = Color.white;
            nameText.alignment = TextAlignmentOptions.Center;
            
            // Item Description
            GameObject descObj = CreateChild(prefab, "Item Description", new Vector2(0.05f, 0.25f), new Vector2(0.95f, 0.45f));
            TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
            descText.text = "Item description here";
            descText.fontSize = 11;
            descText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            descText.alignment = TextAlignmentOptions.Center;
            descText.enableWordWrapping = true;
            
            // Cost Display
            GameObject costObj = CreateChild(prefab, "Cost Text", new Vector2(0.1f, 0.15f), new Vector2(0.9f, 0.25f));
            TextMeshProUGUI costText = costObj.AddComponent<TextMeshProUGUI>();
            costText.text = "$100";
            costText.fontSize = 14;
            costText.fontStyle = FontStyles.Bold;
            costText.color = Color.yellow;
            costText.alignment = TextAlignmentOptions.Center;
            
            // Purchase Button
            GameObject btnObj = CreateChild(prefab, "Purchase Button", new Vector2(0.1f, 0.02f), new Vector2(0.9f, 0.13f));
            Button purchaseBtn = btnObj.AddComponent<Button>();
            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.7f, 0.2f, 1f);
            
            GameObject btnTextObj = CreateChild(btnObj, "Button Text", Vector2.zero, Vector2.one);
            TextMeshProUGUI btnText = btnTextObj.AddComponent<TextMeshProUGUI>();
            btnText.text = "BUY";
            btnText.fontSize = 14;
            btnText.fontStyle = FontStyles.Bold;
            btnText.color = Color.white;
            btnText.alignment = TextAlignmentOptions.Center;
            
            // Owned Indicator
            GameObject ownedObj = CreateChild(prefab, "Owned Indicator", new Vector2(0.75f, 0.8f), new Vector2(0.95f, 0.95f));
            Image ownedImg = ownedObj.AddComponent<Image>();
            ownedImg.color = Color.green;
            ownedObj.SetActive(false);
            
            GameObject checkObj = CreateChild(ownedObj, "Check", Vector2.zero, Vector2.one);
            TextMeshProUGUI checkText = checkObj.AddComponent<TextMeshProUGUI>();
            checkText.text = "✓";
            checkText.fontSize = 14;
            checkText.fontStyle = FontStyles.Bold;
            checkText.color = Color.white;
            checkText.alignment = TextAlignmentOptions.Center;
            
            // Rarity Border (invisible placeholder)
            GameObject rarityObj = CreateChild(prefab, "Rarity Border", Vector2.zero, Vector2.one);
            Image rarityImg = rarityObj.AddComponent<Image>();
            rarityImg.color = Color.clear;
            
            // Hidden coin icon placeholder
            GameObject coinObj = CreateChild(prefab, "Coin Icon", Vector2.zero, Vector2.zero);
            coinObj.AddComponent<Image>();
            coinObj.SetActive(false);
            
            // Add ShopItemUI script
            prefab.AddComponent<ShopItemUI>();
            
            return prefab;
        }
        
        private static GameObject CreateChild(GameObject parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(parent.transform, false);
            
            RectTransform rect = child.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            return child;
        }
        
        private static void SetupShopSystem(GameObject shopPanel, Transform itemContainer, GameObject itemPrefab, Button closeButton)
        {
            GameObject shopSystem = GameObject.Find("Shop System");
            if (shopSystem == null)
            {
                shopSystem = new GameObject("Shop System");
            }
            
            // Add components if they don't exist
            if (!shopSystem.GetComponent<CurrencyManager>())
                shopSystem.AddComponent<CurrencyManager>();
                
            if (!shopSystem.GetComponent<PlayerInventory>())
                shopSystem.AddComponent<PlayerInventory>();
                
            ShopManager shopManager = shopSystem.GetComponent<ShopManager>();
            if (!shopManager)
                shopManager = shopSystem.AddComponent<ShopManager>();
            
            Debug.Log($"Shop System setup complete. You may need to manually assign references:");
            Debug.Log($"   - Shop Panel: {shopPanel.name}");
            Debug.Log($"   - Item Container: {itemContainer.name}"); 
            Debug.Log($"   - Close Button: {closeButton.name}");
            Debug.Log($"   - Item Prefab: Create it as prefab in Project folder");
        }
    }
}