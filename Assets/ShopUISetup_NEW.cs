using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TPSBR
{
    /// <summary>
    /// Shop UI Setup Tool - Build-compatible version
    /// </summary>
    [System.Serializable]
    public class ShopUISetup_NEW : MonoBehaviour
    {
        [ContextMenu("Create Complete Shop UI")]
        public void CreateCompleteShopUI()
        {
            Debug.Log("✅ Shop UI setup started (build-compatible version)");
            
            // Find existing MenuUI or create new Canvas
            Transform menuUIRoot = FindUIRoot();
            
            // Create Shop Panel
            GameObject shopPanel = CreateShopPanel(menuUIRoot);
            
            // Create Currency Display 
            GameObject currencyDisplay = CreateCurrencyDisplay(shopPanel.transform);
            
            // Create Header
            GameObject header = CreateHeader(shopPanel.transform);
            
            // Create Close Button
            GameObject closeButton = CreateCloseButton(shopPanel.transform);
            
            // Create Item Container with Grid Layout
            GameObject itemContainer = CreateItemContainer(shopPanel.transform);
            
            // Create Shop Item Prefab (build-safe version)
            GameObject itemPrefab = CreateShopItemPrefab_BuildSafe();
            
            // Create Shop System GameObject with all managers
            GameObject shopSystem = CreateShopSystem(shopPanel, itemContainer.transform, itemPrefab, closeButton.GetComponent<Button>());
            
            Debug.Log("✅ Complete Shop UI System Created (build-compatible)!");
            Debug.Log("📝 Instructions:");
            Debug.Log("   1. Create shop items with Right-click → Create → TPSBR → Shop Item");
            Debug.Log("   2. Add shop items to ShopManager's Available Items list");
            Debug.Log("   3. Press B in play mode to open shop");
            Debug.Log("   4. Use context menu on CurrencyManager to add test coins");
        }
        
        private Transform FindUIRoot()
        {
            // Try to find existing MenuUI
            Transform menuUI = GameObject.Find("MenuUI")?.transform;
            if (menuUI != null)
            {
                Debug.Log("Found existing MenuUI");
                return menuUI;
            }
            
            // Try to find any Canvas
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            if (canvases.Length > 0)
            {
                Debug.Log($"Found Canvas: {canvases[0].name}");
                return canvases[0].transform;
            }
            
            // Create new Canvas
            Debug.Log("Creating new Canvas");
            GameObject canvasGO = new GameObject("UI Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            
            return canvasGO.transform;
        }
        
        private GameObject CreateShopPanel(Transform parent)
        {
            GameObject panel = new GameObject("Shop Panel");
            panel.transform.SetParent(parent, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.8f);
            
            // Start with panel closed
            panel.SetActive(false);
            
            return panel;
        }
        
        private GameObject CreateCurrencyDisplay(Transform parent)
        {
            GameObject currencyDisplay = new GameObject("Currency Display");
            currencyDisplay.transform.SetParent(parent, false);
            
            RectTransform rect = currencyDisplay.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.02f, 0.9f);
            rect.anchorMax = new Vector2(0.3f, 0.98f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI text = currencyDisplay.AddComponent<TextMeshProUGUI>();
            text.text = "💰 1000 Coins";
            text.fontSize = 24;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.yellow;
            text.alignment = TextAlignmentOptions.Left; // Fixed: was MiddleLeft (not available in Unity 2022.2)
            
            return currencyDisplay;
        }
        
        private GameObject CreateHeader(Transform parent)
        {
            GameObject header = new GameObject("Shop Header");
            header.transform.SetParent(parent, false);
            
            RectTransform rect = header.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.3f, 0.9f);
            rect.anchorMax = new Vector2(0.7f, 0.98f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI text = header.AddComponent<TextMeshProUGUI>();
            text.text = "🏪 ITEM SHOP 🏪";
            text.fontSize = 32;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            
            return header;
        }
        
        private GameObject CreateCloseButton(Transform parent)
        {
            GameObject closeButton = new GameObject("Close Button");
            closeButton.transform.SetParent(parent, false);
            
            RectTransform rect = closeButton.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.92f, 0.92f);
            rect.anchorMax = new Vector2(0.98f, 0.98f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            Image bg = closeButton.AddComponent<Image>();
            bg.color = new Color(0.8f, 0.2f, 0.2f, 1f);
            
            Button button = closeButton.AddComponent<Button>();
            
            GameObject buttonText = new GameObject("Button Text");
            buttonText.transform.SetParent(closeButton.transform, false);
            
            RectTransform textRect = buttonText.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI text = buttonText.AddComponent<TextMeshProUGUI>();
            text.text = "✖";
            text.fontSize = 18;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            
            return closeButton;
        }
        
        private GameObject CreateItemContainer(Transform parent)
        {
            GameObject container = new GameObject("Item Container");
            container.transform.SetParent(parent, false);
            
            RectTransform rect = container.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.1f, 0.1f);
            rect.anchorMax = new Vector2(0.9f, 0.8f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // Add Grid Layout Group
            GridLayoutGroup grid = container.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(200, 280);
            grid.spacing = new Vector2(20, 20);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;
            grid.childAlignment = TextAnchor.UpperCenter;
            
            return container;
        }
        
        private GameObject CreateShopItemPrefab_BuildSafe()
        {
            Debug.Log("🔧 Creating build-safe shop item prefab...");
            
            GameObject itemPrefab = new GameObject("Shop Item Prefab (Build Safe)");
            
            // Main button
            RectTransform rect = itemPrefab.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 280);
            
            // Background
            Image bg = itemPrefab.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.3f, 1f);
            
            Button button = itemPrefab.AddComponent<Button>();
            
            // Add ShopItemUI component
            itemPrefab.AddComponent<ShopItemUI>();
            
            // Create UI elements
            CreateItemUIElements(itemPrefab);
            
            // In build mode, just return the runtime object
            // (Can't save prefabs during builds)
            Debug.Log("✅ Build-safe shop item prefab created");
            return itemPrefab;
        }
        
        private void CreateItemUIElements(GameObject itemPrefab)
        {
            // Item Name
            GameObject itemName = new GameObject("Item Name");
            itemName.transform.SetParent(itemPrefab.transform, false);
            RectTransform nameRect = itemName.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.05f, 0.7f);
            nameRect.anchorMax = new Vector2(0.95f, 0.85f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;
            TextMeshProUGUI nameText = itemName.AddComponent<TextMeshProUGUI>();
            nameText.text = "Item Name";
            nameText.fontSize = 16;
            nameText.fontStyle = FontStyles.Bold;
            nameText.color = Color.white;
            nameText.alignment = TextAlignmentOptions.Center;
            
            // Item Description
            GameObject itemDesc = new GameObject("Item Description");
            itemDesc.transform.SetParent(itemPrefab.transform, false);
            RectTransform descRect = itemDesc.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0.05f, 0.5f);
            descRect.anchorMax = new Vector2(0.95f, 0.7f);
            descRect.offsetMin = Vector2.zero;
            descRect.offsetMax = Vector2.zero;
            TextMeshProUGUI descText = itemDesc.AddComponent<TextMeshProUGUI>();
            descText.text = "Item description";
            descText.fontSize = 12;
            descText.color = Color.gray;
            descText.alignment = TextAlignmentOptions.Center;
            
            // Cost Display
            GameObject costDisplay = new GameObject("Cost Display");
            costDisplay.transform.SetParent(itemPrefab.transform, false);
            RectTransform costRect = costDisplay.AddComponent<RectTransform>();
            costRect.anchorMin = new Vector2(0.05f, 0.35f);
            costRect.anchorMax = new Vector2(0.95f, 0.5f);
            costRect.offsetMin = Vector2.zero;
            costRect.offsetMax = Vector2.zero;
            TextMeshProUGUI costText = costDisplay.AddComponent<TextMeshProUGUI>();
            costText.text = "💰 100 Coins";
            costText.fontSize = 14;
            costText.fontStyle = FontStyles.Bold;
            costText.color = Color.yellow;
            costText.alignment = TextAlignmentOptions.Center;
            
            // Purchase Button
            GameObject purchaseButton = new GameObject("Purchase Button");
            purchaseButton.transform.SetParent(itemPrefab.transform, false);
            RectTransform buttonRect = purchaseButton.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.1f, 0.05f);
            buttonRect.anchorMax = new Vector2(0.9f, 0.3f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            Image buttonBg = purchaseButton.AddComponent<Image>();
            buttonBg.color = new Color(0.2f, 0.8f, 0.2f, 1f);
            
            Button purchaseBtn = purchaseButton.AddComponent<Button>();
            
            GameObject buttonText = new GameObject("Button Text");
            buttonText.transform.SetParent(purchaseButton.transform, false);
            RectTransform textRect = buttonText.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI text = buttonText.AddComponent<TextMeshProUGUI>();
            text.text = "BUY NOW";
            text.fontSize = 14;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
        }
        
        private GameObject CreateShopSystem(GameObject shopPanel, Transform itemContainer, GameObject itemPrefab, Button closeButton)
        {
            // Find or create Shop System
            GameObject shopSystem = GameObject.Find("Shop System");
            if (shopSystem == null)
            {
                shopSystem = new GameObject("Shop System");
            }
            
            // Add Shop Manager
            ShopManager shopManager = shopSystem.GetComponent<ShopManager>();
            if (shopManager == null)
            {
                shopManager = shopSystem.AddComponent<ShopManager>();
            }
            
            Debug.Log("✅ Shop System created successfully!");
            return shopSystem;
        }
    }
}