using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TPSBR
{
    public class CreateShopUI : MonoBehaviour
    {
        [ContextMenu("Create Shop UI Now")]
        public void CreateShopUINow()
        {
            CreateCompleteShopUI();
        }
        
        public void CreateCompleteShopUI()
        {
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
            
            // Create Shop Item Prefab
            GameObject itemPrefab = CreateShopItemPrefab();
            
            // Create Shop System GameObject with all managers
            GameObject shopSystem = CreateShopSystem(shopPanel, itemContainer.transform, itemPrefab, closeButton.GetComponent<Button>());
            
            Debug.Log("✅ Complete Shop UI System Created!");
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
                return menuUI;
            }
            
            // Create new Canvas for shop
            GameObject canvas = new GameObject("Shop Canvas");
            Canvas canvasComp = canvas.AddComponent<Canvas>();
            canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasComp.sortingOrder = 200; // Above other UI
            
            CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvas.AddComponent<GraphicRaycaster>();
            
            return canvas.transform;
        }
        
        private GameObject CreateShopPanel(Transform parent)
        {
            GameObject shopPanel = new GameObject("Shop Panel");
            shopPanel.transform.SetParent(parent, false);
            
            // Add RectTransform
            RectTransform rect = shopPanel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // Add Image background
            Image bg = shopPanel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black
            
            // Start inactive
            shopPanel.SetActive(false);
            
            return shopPanel;
        }
        
        private GameObject CreateCurrencyDisplay(Transform parent)
        {
            GameObject currencyObj = new GameObject("Currency Display");
            currencyObj.transform.SetParent(parent, false);
            
            RectTransform rect = currencyObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-20, -20);
            rect.sizeDelta = new Vector2(200, 40);
            
            TextMeshProUGUI text = currencyObj.AddComponent<TextMeshProUGUI>();
            text.text = "$1000";
            text.fontSize = 24;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.yellow;
            text.alignment = TextAlignmentOptions.Right;
            
            currencyObj.AddComponent<CurrencyDisplay>();
            
            return currencyObj;
        }
        
        private GameObject CreateHeader(Transform parent)
        {
            GameObject header = new GameObject("Shop Header");
            header.transform.SetParent(parent, false);
            
            RectTransform rect = header.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(0, -50);
            rect.sizeDelta = new Vector2(0, 60);
            
            TextMeshProUGUI text = header.AddComponent<TextMeshProUGUI>();
            text.text = "ITEM SHOP";
            text.fontSize = 36;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            
            return header;
        }
        
        private GameObject CreateCloseButton(Transform parent)
        {
            GameObject closeBtn = new GameObject("Close Button");
            closeBtn.transform.SetParent(parent, false);
            
            RectTransform rect = closeBtn.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-30, -30);
            rect.sizeDelta = new Vector2(40, 40);
            
            Button button = closeBtn.AddComponent<Button>();
            Image buttonImage = closeBtn.AddComponent<Image>();
            buttonImage.color = Color.red;
            
            // Add X text
            GameObject buttonText = new GameObject("Text");
            buttonText.transform.SetParent(closeBtn.transform, false);
            
            RectTransform textRect = buttonText.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI text = buttonText.AddComponent<TextMeshProUGUI>();
            text.text = "X";
            text.fontSize = 24;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            
            return closeBtn;
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
        
        private GameObject CreateShopItemPrefab()
        {
            GameObject itemPrefab = new GameObject("Shop Item Prefab");
            
            // Main button
            RectTransform rect = itemPrefab.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 280);
            
            Button mainButton = itemPrefab.AddComponent<Button>();
            Image mainImage = itemPrefab.AddComponent<Image>();
            mainImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            
            // Rarity border
            GameObject rarityBorder = new GameObject("Rarity Border");
            rarityBorder.transform.SetParent(itemPrefab.transform, false);
            RectTransform rarityRect = rarityBorder.AddComponent<RectTransform>();
            rarityRect.anchorMin = Vector2.zero;
            rarityRect.anchorMax = Vector2.one;
            rarityRect.offsetMin = Vector2.zero;
            rarityRect.offsetMax = Vector2.zero;
            Image rarityImage = rarityBorder.AddComponent<Image>();
            rarityImage.color = Color.clear;
            
            // Item Icon
            GameObject icon = new GameObject("Item Icon");
            icon.transform.SetParent(itemPrefab.transform, false);
            RectTransform iconRect = icon.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.6f);
            iconRect.anchorMax = new Vector2(0.9f, 0.9f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            Image iconImage = icon.AddComponent<Image>();
            iconImage.color = Color.white;
            
            // Item Name
            GameObject itemName = new GameObject("Item Name");
            itemName.transform.SetParent(itemPrefab.transform, false);
            RectTransform nameRect = itemName.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.05f, 0.45f);
            nameRect.anchorMax = new Vector2(0.95f, 0.6f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;
            TextMeshProUGUI nameText = itemName.AddComponent<TextMeshProUGUI>();
            nameText.text = "Item Name";
            nameText.fontSize = 14;
            nameText.fontStyle = FontStyles.Bold;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.color = Color.white;
            
            // Item Description
            GameObject itemDesc = new GameObject("Item Description");
            itemDesc.transform.SetParent(itemPrefab.transform, false);
            RectTransform descRect = itemDesc.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0.05f, 0.25f);
            descRect.anchorMax = new Vector2(0.95f, 0.45f);
            descRect.offsetMin = Vector2.zero;
            descRect.offsetMax = Vector2.zero;
            TextMeshProUGUI descText = itemDesc.AddComponent<TextMeshProUGUI>();
            descText.text = "Item description";
            descText.fontSize = 10;
            descText.alignment = TextAlignmentOptions.Center;
            descText.color = Color.gray;
            descText.enableWordWrapping = true;
            
            // Cost Text
            GameObject costText = new GameObject("Cost Text");
            costText.transform.SetParent(itemPrefab.transform, false);
            RectTransform costTextRect = costText.AddComponent<RectTransform>();
            costTextRect.anchorMin = new Vector2(0.1f, 0.15f);
            costTextRect.anchorMax = new Vector2(0.9f, 0.25f);
            costTextRect.offsetMin = Vector2.zero;
            costTextRect.offsetMax = Vector2.zero;
            TextMeshProUGUI costTextComp = costText.AddComponent<TextMeshProUGUI>();
            costTextComp.text = "$100";
            costTextComp.fontSize = 12;
            costTextComp.fontStyle = FontStyles.Bold;
            costTextComp.alignment = TextAlignmentOptions.Center;
            costTextComp.color = Color.yellow;
            
            // Purchase Button
            GameObject purchaseBtn = new GameObject("Purchase Button");
            purchaseBtn.transform.SetParent(itemPrefab.transform, false);
            RectTransform purchaseRect = purchaseBtn.AddComponent<RectTransform>();
            purchaseRect.anchorMin = new Vector2(0.1f, 0.02f);
            purchaseRect.anchorMax = new Vector2(0.9f, 0.13f);
            purchaseRect.offsetMin = Vector2.zero;
            purchaseRect.offsetMax = Vector2.zero;
            
            Button purchaseButton = purchaseBtn.AddComponent<Button>();
            Image purchaseImage = purchaseBtn.AddComponent<Image>();
            purchaseImage.color = Color.green;
            
            // Purchase Button Text
            GameObject purchaseText = new GameObject("Purchase Text");
            purchaseText.transform.SetParent(purchaseBtn.transform, false);
            RectTransform purchaseTextRect = purchaseText.AddComponent<RectTransform>();
            purchaseTextRect.anchorMin = Vector2.zero;
            purchaseTextRect.anchorMax = Vector2.one;
            purchaseTextRect.offsetMin = Vector2.zero;
            purchaseTextRect.offsetMax = Vector2.zero;
            TextMeshProUGUI purchaseTextComp = purchaseText.AddComponent<TextMeshProUGUI>();
            purchaseTextComp.text = "BUY";
            purchaseTextComp.fontSize = 12;
            purchaseTextComp.fontStyle = FontStyles.Bold;
            purchaseTextComp.alignment = TextAlignmentOptions.Center;
            purchaseTextComp.color = Color.white;
            
            // Owned Indicator
            GameObject ownedIndicator = new GameObject("Owned Indicator");
            ownedIndicator.transform.SetParent(itemPrefab.transform, false);
            RectTransform ownedRect = ownedIndicator.AddComponent<RectTransform>();
            ownedRect.anchorMin = new Vector2(0.7f, 0.8f);
            ownedRect.anchorMax = new Vector2(0.95f, 0.95f);
            ownedRect.offsetMin = Vector2.zero;
            ownedRect.offsetMax = Vector2.zero;
            Image ownedImage = ownedIndicator.AddComponent<Image>();
            ownedImage.color = Color.green;
            ownedIndicator.SetActive(false);
            
            // Add owned checkmark
            GameObject ownedText = new GameObject("Owned Text");
            ownedText.transform.SetParent(ownedIndicator.transform, false);
            RectTransform ownedTextRect = ownedText.AddComponent<RectTransform>();
            ownedTextRect.anchorMin = Vector2.zero;
            ownedTextRect.anchorMax = Vector2.one;
            ownedTextRect.offsetMin = Vector2.zero;
            ownedTextRect.offsetMax = Vector2.zero;
            TextMeshProUGUI ownedTextComp = ownedText.AddComponent<TextMeshProUGUI>();
            ownedTextComp.text = "✓";
            ownedTextComp.fontSize = 14;
            ownedTextComp.fontStyle = FontStyles.Bold;
            ownedTextComp.alignment = TextAlignmentOptions.Center;
            ownedTextComp.color = Color.white;
            
            // Add coin icon placeholder
            GameObject coinIcon = new GameObject("Coin Icon");
            coinIcon.transform.SetParent(itemPrefab.transform, false);
            RectTransform coinRect = coinIcon.AddComponent<RectTransform>();
            coinRect.anchorMin = new Vector2(0, 0);
            coinRect.anchorMax = new Vector2(0, 0);
            coinRect.sizeDelta = new Vector2(20, 20);
            Image coinImage = coinIcon.AddComponent<Image>();
            coinImage.color = Color.yellow;
            coinIcon.SetActive(false); // Hidden placeholder
            
            // Add ShopItemUI component
            ShopItemUI shopItemUI = itemPrefab.AddComponent<ShopItemUI>();
            
            return itemPrefab;
        }
        
        private GameObject CreateShopSystem(GameObject shopPanel, Transform itemContainer, GameObject itemPrefab, Button closeButton)
        {
            // Find or create Shop System
            GameObject shopSystem = GameObject.Find("Shop System");
            if (shopSystem == null)
            {
                shopSystem = new GameObject("Shop System");
            }
            
            // Add Currency Manager
            if (shopSystem.GetComponent<CurrencyManager>() == null)
            {
                shopSystem.AddComponent<CurrencyManager>();
            }
            
            // Add Player Inventory
            if (shopSystem.GetComponent<PlayerInventory>() == null)
            {
                shopSystem.AddComponent<PlayerInventory>();
            }
            
            // Add Shop Manager
            ShopManager shopManager = shopSystem.GetComponent<ShopManager>();
            if (shopManager == null)
            {
                shopManager = shopSystem.AddComponent<ShopManager>();
            }
            
            Debug.Log("Shop System created with components. You'll need to manually assign the UI references in the inspector.");
            
            return shopSystem;
        }
    }
}