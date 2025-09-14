using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

namespace TPSBR
{
    public class ShopManager : MonoBehaviour
    {
        [Header("Shop Settings")]
        [SerializeField] private List<ShopItem> _availableItems = new List<ShopItem>();
        [SerializeField] private bool _debugMode = true;
        
        [Header("UI References")]
        [SerializeField] private GameObject _shopPanel;
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private GameObject _shopItemPrefab;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _mainPurchaseButton; // Main visible purchase button
        
        [Header("Main Menu Blocking")]
        [SerializeField] private CanvasGroup _mainMenuCanvasGroup;
        
        public static ShopManager Instance { get; private set; }
        
        // Events
        public static event System.Action OnShopOpened;
        public static event System.Action OnShopClosed;
        
        private List<ShopItemUI> _itemUIElements = new List<ShopItemUI>();
        private bool _isShopOpen = false;
        
        public bool IsShopOpen => _isShopOpen;
        
        private void Awake()
        {
            Instance = this;
        }
        
        private void Start()
        {
            // Force clear any existing setup first
            Debug.Log("🔄 Starting fresh shop setup...");
            
            // Find main menu canvas group if not assigned
            if (_mainMenuCanvasGroup == null)
            {
                GameObject mainMenuView = GameObject.Find("UIMainMenuView");
                if (mainMenuView != null)
                {
                    _mainMenuCanvasGroup = mainMenuView.GetComponent<CanvasGroup>();
                    if (_mainMenuCanvasGroup != null)
                    {
                        Debug.Log("✅ Found main menu CanvasGroup for blocking");
                    }
                }
            }
            
            CreateShopUIIfNeeded();
            SetupShop();
            
            // Shop can be opened with 'B' key - no auto-opening
            Debug.Log("🔑 Press 'B' to open/close the shop!");
        }
        
        private Canvas FindOrCreateShopCanvas()
        {
            // PRIORITY 1: Look for MenuUI Canvas (the main UI Canvas)
            GameObject menuUI = GameObject.Find("MenuUI");
            if (menuUI != null)
            {
                Canvas menuCanvas = menuUI.GetComponent<Canvas>();
                if (menuCanvas != null)
                {
                    Debug.Log($"   Found MenuUI Canvas: {menuCanvas.name}");
                    return menuCanvas;
                }
            }
            
            // PRIORITY 2: Look for existing Canvas with our shop elements
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            
            foreach (Canvas canvas in canvases)
            {
                if (canvas.transform.Find("Shop Panel") != null)
                {
                    Debug.Log($"   Found existing Canvas with Shop Panel: {canvas.name}");
                    return canvas;
                }
            }
            
            // PRIORITY 3: Use any existing Canvas
            if (canvases.Length > 0)
            {
                Debug.Log($"   Using existing Canvas: {canvases[0].name}");
                return canvases[0];
            }
            
            // PRIORITY 4: Create new Canvas if none found
            Debug.Log("🖼️ Creating Shop Canvas...");
            
            try
            {
                GameObject canvasObj = new GameObject("Shop Canvas");
                Canvas canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100; // Make sure it appears on top
                
                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
                
                GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();
                
                Debug.Log($"✅ Created Shop Canvas: {canvasObj.name}");
                return canvas;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Failed to create Canvas: {e.Message}");
                return null;
            }
        }
        
        [ContextMenu("Complete Shop Reset")]
        public void CompleteShopReset()
        {
            Debug.Log("🔄 Performing complete shop reset...");
            
            try 
            {
                // Clear all references
                _shopPanel = null;
                _itemContainer = null;
                _closeButton = null;
                _shopItemPrefab = null;
                
                // Clear existing items
                foreach (ShopItemUI item in _itemUIElements)
                {
                    if (item != null && item.gameObject != null)
                    {
                        DestroyImmediate(item.gameObject);
                    }
                }
                _itemUIElements.Clear();
                
                // Find and cleanup any existing shop panels
                GameObject[] existingPanels = GameObject.FindGameObjectsWithTag("Untagged");
                foreach (GameObject obj in existingPanels)
                {
                    if (obj.name.Contains("Shop Panel") || obj.name.Contains("ShopPanel"))
                    {
                        Debug.Log($"   Cleaning up existing panel: {obj.name}");
                        DestroyImmediate(obj);
                    }
                }
                
                // Wait a frame for cleanup
                System.GC.Collect();
                
                // Recreate everything from scratch
                CreateShopUIIfNeeded();
                SetupShop();
                CloseShop();
                
                Debug.Log("✅ Complete shop reset finished!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error during shop reset: {e.Message}");
                Debug.LogError($"Stack trace: {e.StackTrace}");
            }
        }
        
        [ContextMenu("Force Refresh Shop")]
        public void ForceRefreshShop()
        {
            Debug.Log("🔄 Force refreshing shop setup...");
            
            // Clear existing items
            foreach (ShopItemUI item in _itemUIElements)
            {
                if (item != null)
                {
                    DestroyImmediate(item.gameObject);
                }
            }
            _itemUIElements.Clear();
            
            // Clear the container reference to force recreation
            _itemContainer = null;
            
            // Recreate everything
            CreateShopUIIfNeeded();
            SetupShop();
            
            Debug.Log("✅ Shop forcefully refreshed!");
        }
        
        private Transform FindItemContainerInChildren(Transform parent)
        {
            // Look for ItemContainer directly as child
            Transform direct = parent.Find("ItemContainer");
            if (direct != null) return direct;
            
            // Look for ItemContainer in all children recursively
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == "ItemContainer")
                    return child;
                    
                // Check nested children
                Transform nested = FindItemContainerInChildren(child);
                if (nested != null) return nested;
            }
            
            return null;
        }
        
        private void CreateShopUIIfNeeded()
        {
            Debug.Log("🔍 Validating shop UI references...");
            
            // CRITICAL: Ensure we have a Canvas for UI elements
            Canvas shopCanvas = FindOrCreateShopCanvas();
            if (shopCanvas == null)
            {
                Debug.LogError("❌ Failed to create Canvas! Shop will not work.");
                return;
            }
            
            // Always check and fix missing references
            if (_shopPanel == null)
            {
                Debug.Log("   ShopPanel is null, searching...");
                
                // First look for Shop Panel under any Canvas
                GameObject foundPanel = shopCanvas.transform.Find("Shop Panel")?.gameObject;
                if (foundPanel == null)
                {
                    // Look in global scene
                    GameObject globalPanel = GameObject.Find("Shop Panel");
                    if (globalPanel != null)
                    {
                        // Move it under Canvas
                        globalPanel.transform.SetParent(shopCanvas.transform, false);
                        foundPanel = globalPanel;
                        Debug.Log($"   Moved existing Shop Panel under Canvas");
                    }
                }
                
                if (foundPanel != null)
                {
                    _shopPanel = foundPanel;
                    Debug.Log($"   Found ShopPanel: {_shopPanel.name}");
                }
                else
                {
                    // Create new Shop Panel under Canvas
                    GameObject newPanel = new GameObject("Shop Panel");
                    newPanel.transform.SetParent(shopCanvas.transform, false);
                    newPanel.AddComponent<RectTransform>();
                    _shopPanel = newPanel;
                    Debug.Log($"   Created new ShopPanel under Canvas: {_shopPanel.name}");
                }
            }
            else
            {
                Debug.Log($"   ShopPanel already assigned: {_shopPanel.name}");
            }
            
            // Ensure Shop Panel is properly under Canvas and has RectTransform
            if (_shopPanel != null)
            {
                // Check if it's under Canvas
                if (_shopPanel.GetComponentInParent<Canvas>() == null)
                {
                    Debug.Log("🔧 Moving ShopPanel under Canvas...");
                    _shopPanel.transform.SetParent(shopCanvas.transform, false);
                }
                
                // Ensure it has RectTransform
                RectTransform shopRect = _shopPanel.GetComponent<RectTransform>();
                if (shopRect == null)
                {
                    Debug.Log("🔧 Adding RectTransform to ShopPanel...");
                    shopRect = _shopPanel.AddComponent<RectTransform>();
                }
                
                // Ensure it's on the UI layer
                if (_shopPanel.layer != LayerMask.NameToLayer("UI"))
                {
                    Debug.Log("🔧 Setting ShopPanel to UI layer...");
                    _shopPanel.layer = LayerMask.NameToLayer("UI");
                }
                
                // Set full screen
                shopRect.anchorMin = Vector2.zero;
                shopRect.anchorMax = Vector2.one;
                shopRect.anchoredPosition = Vector2.zero;
                shopRect.sizeDelta = Vector2.zero;
                
                // Add background image to shop panel for visibility
                Image shopBackground = _shopPanel.GetComponent<Image>();
                if (shopBackground == null)
                {
                    Debug.Log("🎨 Adding background to ShopPanel for visibility");
                    shopBackground = _shopPanel.AddComponent<Image>();
                }
                shopBackground.color = new Color(0f, 0f, 0f, 0.8f); // Semi-transparent black background
                
                // Fix scale if needed
                if (shopRect.localScale == Vector3.zero)
                {
                    Debug.Log("🔧 Fixing ShopPanel scale from (0,0,0) to (1,1,1)");
                    shopRect.localScale = Vector3.one;
                }
                
                // Start with shop closed
                if (_shopPanel.activeSelf)
                {
                    Debug.Log("🔧 Starting with Shop Panel closed");
                    _shopPanel.SetActive(false);
                }
            }
            
            // Always ensure we use a container inside ShopPanel, not the standalone one
            Debug.Log("   Looking for proper ItemContainer inside ShopPanel...");
            
            // First try to find ItemContainer inside ShopPanel
            Transform shopContainer = null;
            if (_shopPanel != null)
            {
                shopContainer = FindItemContainerInChildren(_shopPanel.transform);
                Debug.Log($"   Found container in ShopPanel: {(shopContainer != null ? shopContainer.name : "NONE")}");
            }
            
            if (shopContainer != null)
            {
                _itemContainer = shopContainer;
                Debug.Log($"✅ Using ShopPanel's ItemContainer: {_itemContainer.name}");
            }
            else
            {
                // Create a proper container inside ShopPanel for items
                Debug.Log("   Creating new ItemContainer inside ShopPanel...");
                GameObject container = new GameObject("ItemContainer");
                container.transform.SetParent(_shopPanel.transform, false);
                
                RectTransform containerRect = container.AddComponent<RectTransform>();
                containerRect.anchorMin = new Vector2(0.35f, 0.2f);  // Start after tabs area
                containerRect.anchorMax = new Vector2(0.95f, 0.85f);  // Take most of content area
                containerRect.anchoredPosition = Vector2.zero;
                containerRect.sizeDelta = Vector2.zero;
                
                // Add visible background for debugging
                Image containerBg = container.AddComponent<Image>();
                containerBg.color = new Color(0, 1, 0, 0.1f); // Semi-transparent green for debugging
                
                // Add grid layout for items
                GridLayoutGroup grid = container.AddComponent<GridLayoutGroup>();
                grid.cellSize = new Vector2(180, 220);
                grid.spacing = new Vector2(20, 20);
                grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
                grid.startAxis = GridLayoutGroup.Axis.Horizontal;
                grid.childAlignment = TextAnchor.UpperLeft;
                grid.constraint = GridLayoutGroup.Constraint.Flexible;
                grid.padding = new RectOffset(20, 20, 20, 20);
                
                _itemContainer = container.transform;
                Debug.Log($"✅ Created new ItemContainer inside ShopPanel: {_itemContainer.name}");
            }
            
            Debug.Log($"   ItemContainer result: {(_itemContainer != null ? _itemContainer.name : "NOT FOUND")}");
            Debug.Log($"   ItemContainer parent: {(_itemContainer != null ? _itemContainer.parent?.name : "NO PARENT")}");
            
            // Create Fortnite-style UI if we have the basic structure
            if (_shopPanel != null && _itemContainer != null)
            {
                Debug.Log("🎨 Creating Fortnite-style shop UI...");
                FortniteStyleShopUI.CreateFullScreenShop(_shopPanel.transform, _itemContainer);
                
                // Look for close button in the new UI
                _closeButton = _shopPanel.transform.Find("Header/Close Button")?.GetComponent<Button>();
                if (_closeButton != null)
                {
                    Debug.Log($"✅ Found close button: {_closeButton.name}");
                }
                
                // Create the purchase button right after the UI is created
                Debug.Log("🛒 Creating purchase button...");
                CreateMainPurchaseButton();
            }
            
            if (_shopItemPrefab == null)
            {
                Debug.Log("   ShopItemPrefab is null, creating Fortnite-style prefab...");
                
                if (_itemContainer != null)
                {
                    // Create a new Fortnite-style shop item prefab
                    _shopItemPrefab = FortniteStyleShopItem.CreateFortniteStyleItem(_itemContainer);
                    _shopItemPrefab.SetActive(false); // Hide the prefab
                    _shopItemPrefab.name = "Fortnite Shop Item Prefab";
                    Debug.Log($"✅ Created Fortnite-style prefab: {_shopItemPrefab.name}");
                }
            }
            else
            {
                Debug.Log($"   ShopItemPrefab already assigned: {_shopItemPrefab.name}");
            }
            
            // Final validation and logging
            Debug.Log($"🏪 Shop UI Validation Complete:");
            Debug.Log($"   Shop Panel: {(_shopPanel != null ? "✅ " + _shopPanel.name : "❌ NOT FOUND")}");
            Debug.Log($"   Item Container: {(_itemContainer != null ? "✅ " + _itemContainer.name : "❌ NOT FOUND")}");
            Debug.Log($"   Shop Item Prefab: {(_shopItemPrefab != null ? "✅ " + _shopItemPrefab.name : "❌ NOT FOUND")}");
            Debug.Log($"   Close Button: {(_closeButton != null ? "✅ " + _closeButton.name : "⚠️ NOT FOUND (will search later)")}");
            
            if (_shopPanel == null || _itemContainer == null)
            {
                Debug.LogError("❌ Critical shop UI elements missing!");
                Debug.LogError("   This will cause 'missing prefab or container' errors when trying to create shop items.");
            }
            
            if (_shopItemPrefab == null)
            {
                Debug.LogError("❌ Shop item prefab missing!");
                Debug.LogError("   This will cause 'missing prefab or container' errors when trying to create shop items.");
            }
        }
        
        private void Update()
        {
            // Debug key to check coin amounts (C key)
            if (UnityEngine.InputSystem.Keyboard.current.cKey.wasPressedThisFrame)
            {
                if (CurrencyManager.Instance != null)
                {
                    Debug.Log($"💰 DEBUG: CurrencyManager says you have {CurrencyManager.Instance.CurrentCoins} coins");
                    Debug.Log($"💰 DEBUG: Can afford 400 coins? {CurrencyManager.Instance.CanAfford(400)}");
                    
                    // Update the button text to reflect current state
                    if (_mainPurchaseButton != null)
                    {
                        UpdatePurchaseButtonText();
                    }
                }
                else
                {
                    Debug.Log("❌ DEBUG: CurrencyManager.Instance is null!");
                }
            }
            
            // Toggle shop with 'B' key
            if (UnityEngine.InputSystem.Keyboard.current.bKey.wasPressedThisFrame)
            {
                if (_isShopOpen)
                {
                    CloseShop();
                }
                else
                {
                    OpenShop();
                }
            }
            
            // Fallback: Close shop with Escape key (direct input handling)
            if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame && _isShopOpen)
            {
                CloseShop();
            }
        }
        
        private void SetupShop()
        {
            if (_itemContainer == null || _shopItemPrefab == null)
            {
                Debug.LogError("❌ Shop UI references not set!");
                return;
            }
            
            // Setup close button - find it if not assigned
            if (_closeButton == null && _shopPanel != null)
            {
                Debug.Log("🔍 Searching for close button in shop panel...");
                _closeButton = _shopPanel.GetComponentInChildren<Button>();
                if (_closeButton != null)
                {
                    Debug.Log($"✅ Found close button: {_closeButton.name}");
                }
            }
            
            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveAllListeners();
                _closeButton.onClick.AddListener(() => {
                    Debug.Log("🔘 Close button clicked");
                    CloseShop();
                });
                Debug.Log("✅ Close button listener added");
            }
            else
            {
                Debug.LogWarning("⚠️ No close button found or assigned!");
            }
            
            // If no items are configured, add some test items
            if (_availableItems.Count == 0)
            {
                Debug.Log("📦 No shop items configured, adding test items...");
                
                ShopItem testWeapon = ScriptableObject.CreateInstance<ShopItem>();
                testWeapon.itemName = "Test Weapon";
                testWeapon.description = "A basic test weapon";
                testWeapon.cost = 100;
                testWeapon.isAvailable = true;
                testWeapon.itemType = ShopItemType.Weapon;
                testWeapon.rarity = ShopItemRarity.Common;
                _availableItems.Add(testWeapon);
                
                ShopItem testArmor = ScriptableObject.CreateInstance<ShopItem>();
                testArmor.itemName = "Test Armor";
                testArmor.description = "Basic test armor";
                testArmor.cost = 200;
                testArmor.isAvailable = true;
                testArmor.itemType = ShopItemType.Cosmetic;
                testArmor.rarity = ShopItemRarity.Uncommon;
                _availableItems.Add(testArmor);
                
                Debug.Log($"✅ Added {_availableItems.Count} test items");
            }
            
            // Clear existing items (but preserve the prefab and its parent hierarchy)
            for (int i = _itemContainer.childCount - 1; i >= 0; i--)
            {
                Transform child = _itemContainer.GetChild(i);
                
                // Don't destroy anything that contains our prefab
                bool containsPrefab = false;
                if (_shopItemPrefab != null)
                {
                    // Check if this child is the prefab itself
                    if (child.gameObject == _shopItemPrefab)
                    {
                        containsPrefab = true;
                    }
                    // Check if this child contains the prefab
                    else if (_shopItemPrefab.transform.IsChildOf(child))
                    {
                        containsPrefab = true;
                    }
                }
                
                if (containsPrefab)
                {
                    Debug.Log($"🛡️ Preserving {child.name} (contains prefab: {_shopItemPrefab.name})");
                    continue;
                }
                
                DestroyImmediate(child.gameObject);
            }
            _itemUIElements.Clear();
            
            // Create UI elements for each shop item
            foreach (ShopItem item in _availableItems)
            {
                if (item != null && item.isAvailable)
                {
                    CreateShopItemUI(item);
                }
            }
            
            if (_debugMode)
            {
                Debug.Log($"🏪 Shop setup complete with {_itemUIElements.Count} items");
            }
            
            // Create a big, visible purchase button for the main character
            CreateMainPurchaseButton();
        }
        
        private void CreateShopItemUI(ShopItem item)
        {
            // Debug what we have right before creating the item
            Debug.Log($"🛠️ CreateShopItemUI called for: {item.itemName}");
            Debug.Log($"   _shopItemPrefab: {(_shopItemPrefab != null ? _shopItemPrefab.name : "NULL")}");
            Debug.Log($"   _itemContainer: {(_itemContainer != null ? _itemContainer.name : "NULL")}");
            
            if (_shopItemPrefab != null && _itemContainer != null)
            {
                // Check if the prefab has ShopItemUI component
                ShopItemUI prefabShopItemUI = _shopItemPrefab.GetComponent<ShopItemUI>();
                Debug.Log($"   ShopItemUI on prefab: {(prefabShopItemUI != null ? "YES" : "NO")}");
                
                if (prefabShopItemUI == null)
                {
                    Debug.LogWarning($"⚠️ Adding missing ShopItemUI component to prefab: {_shopItemPrefab.name}");
                    prefabShopItemUI = _shopItemPrefab.AddComponent<ShopItemUI>();
                }
                
                GameObject itemUI = Instantiate(_shopItemPrefab, _itemContainer);
                itemUI.SetActive(true); // Make sure the instantiated item is active
                
                // Position the item properly in a grid layout
                RectTransform itemRect = itemUI.GetComponent<RectTransform>();
                if (itemRect != null)
                {
                    // Simple positioning logic for visibility
                    int itemIndex = _itemUIElements.Count;
                    float xOffset = (itemIndex % 3) * 200f; // 3 items per row
                    float yOffset = -(itemIndex / 3) * 240f; // New row every 3 items
                    
                    itemRect.anchoredPosition = new Vector2(xOffset, yOffset);
                    itemRect.sizeDelta = new Vector2(180, 220);
                    
                    Debug.Log($"📍 Positioned item {itemIndex} at ({xOffset}, {yOffset})");
                }
                
                ShopItemUI shopItemUI = itemUI.GetComponent<ShopItemUI>();
                
                if (shopItemUI != null)
                {
                    shopItemUI.Setup(item);
                    _itemUIElements.Add(shopItemUI);
                    Debug.Log($"✅ Successfully created shop item UI for: {item.itemName}");
                }
                else
                {
                    Debug.LogError($"❌ ShopItemUI component missing on instantiated item: {itemUI.name}");
                }
            }
            else
            {
                Debug.LogError("❌ Cannot create shop item UI - missing prefab or container");
                Debug.LogError($"   _shopItemPrefab is null: {_shopItemPrefab == null}");
                Debug.LogError($"   _itemContainer is null: {_itemContainer == null}");
            }
        }
        
        public void ToggleShop()
        {
            if (_isShopOpen)
            {
                CloseShop();
            }
            else
            {
                OpenShop();
            }
        }
        
        public void OpenShop()
        {
            if (_shopPanel != null)
            {
                // Force fix the scale issue
                RectTransform shopRect = _shopPanel.GetComponent<RectTransform>();
                if (shopRect != null)
                {
                    shopRect.localScale = Vector3.one;
                    Debug.Log("🔧 Fixed ShopPanel scale on open");
                }
                
                _shopPanel.SetActive(true);
                _isShopOpen = true;
                
                // Block main menu interactions
                if (_mainMenuCanvasGroup != null)
                {
                    _mainMenuCanvasGroup.interactable = false;
                    _mainMenuCanvasGroup.blocksRaycasts = false;
                    Debug.Log("🚫 Blocked main menu interactions");
                }
                
                // Update all item states
                RefreshShopItems();
                
                // Show cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
                OnShopOpened?.Invoke();
                
                Debug.Log($"🏪 Shop opened! Showing {_itemUIElements.Count} items:");
                for (int i = 0; i < _itemUIElements.Count; i++)
                {
                    if (_itemUIElements[i] != null)
                    {
                        Debug.Log($"   Item {i + 1}: {_itemUIElements[i].name} (Active: {_itemUIElements[i].gameObject.activeSelf})");
                        Debug.Log($"   Item position: {_itemUIElements[i].transform.position}");
                        Debug.Log($"   Item parent: {_itemUIElements[i].transform.parent?.name}");
                    }
                }
                Debug.Log($"   Shop Panel active: {_shopPanel.activeSelf}");
                Debug.Log($"   Shop Panel scale: {_shopPanel.transform.localScale}");
                Debug.Log($"   ItemContainer children: {_itemContainer.childCount}");
            }
            else
            {
                Debug.LogError("❌ Cannot open shop - ShopPanel is null!");
            }
        }
        
        public void CloseShop()
        {
            if (_shopPanel != null)
            {
                _shopPanel.SetActive(false);
                _isShopOpen = false;
                
                // Re-enable main menu interactions
                if (_mainMenuCanvasGroup != null)
                {
                    _mainMenuCanvasGroup.interactable = true;
                    _mainMenuCanvasGroup.blocksRaycasts = true;
                    Debug.Log("✅ Re-enabled main menu interactions");
                }
                
                OnShopClosed?.Invoke();
                
                if (_debugMode)
                {
                    Debug.Log("🏪 Shop closed successfully");
                }
            }
            else
            {
                Debug.LogError("❌ Cannot close shop: _shopPanel is null!");
            }
        }
        
        public void RefreshShopItems()
        {
            foreach (ShopItemUI itemUI in _itemUIElements)
            {
                if (itemUI != null)
                {
                    itemUI.UpdateItemState();
                }
            }
        }
        
        public List<ShopItem> GetItemsByType(ShopItemType itemType)
        {
            return _availableItems.Where(item => item.itemType == itemType && item.isAvailable).ToList();
        }
        
        public List<ShopItem> GetItemsByRarity(ShopItemRarity rarity)
        {
            return _availableItems.Where(item => item.rarity == rarity && item.isAvailable).ToList();
        }
        
        [ContextMenu("Refresh Shop Items")]
        public void RefreshShopItemsContext()
        {
            SetupShop();
        }
        
        private void UpdatePurchaseButtonText()
        {
            if (_mainPurchaseButton == null) return;
            
            Text buttonText = _mainPurchaseButton.GetComponentInChildren<Text>();
            Image buttonImage = _mainPurchaseButton.GetComponent<Image>();
            
            if (buttonText == null || buttonImage == null) return;
            
            // Check ownership and coin status
            bool alreadyOwned = false;
            bool needsRefund = false;
            if (PlayerInventory.Instance != null)
            {
                alreadyOwned = PlayerInventory.Instance.HasItem("Agent.Soldier");
                needsRefund = PlayerInventory.Instance.HasItem("Soldier 66") && !alreadyOwned;
            }
            
            if (alreadyOwned)
            {
                buttonText.text = "✅ SOLDIER 66 - OWNED! ✅";
                buttonText.color = Color.yellow;
                _mainPurchaseButton.interactable = false;
                buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 0.9f);
            }
            else if (needsRefund)
            {
                buttonText.text = "🔧 FIX OWNERSHIP (FREE) 🔧";
                buttonText.color = Color.cyan;
                _mainPurchaseButton.interactable = true;
                buttonImage.color = new Color(0f, 0.9f, 0.9f, 1f);
            }
            else if (CurrencyManager.Instance != null && CurrencyManager.Instance.CurrentCoins < 400)
            {
                buttonText.text = $"💸 NOT ENOUGH COINS ({CurrencyManager.Instance.CurrentCoins}/400) 💸";
                buttonText.color = Color.red;
                _mainPurchaseButton.interactable = false;
                buttonImage.color = new Color(0.5f, 0.2f, 0.2f, 0.9f);
            }
            else
            {
                buttonText.text = "🛒 BUY SOLDIER 66 - 400 COINS 🛒";
                buttonText.color = Color.white;
                _mainPurchaseButton.interactable = true;
                buttonImage.color = new Color(0f, 0.9f, 0.2f, 1f);
            }
            
            Debug.Log($"🔄 Updated button text to: {buttonText.text}");
        }

        private void CreateMainPurchaseButton()
        {
            // Place the purchase button inside the shop panel for better integration
            Transform buttonParent = _shopPanel != null ? _shopPanel.transform : FindObjectOfType<Canvas>()?.transform;
            if (buttonParent == null)
            {
                Debug.LogError("❌ No parent found for purchase button");
                return;
            }
            
            // Create a big, obvious purchase button
            GameObject buttonObj = new GameObject("PURCHASE CHARACTER BUTTON");
            buttonObj.transform.SetParent(buttonParent, false);
            
            // Set it up as a UI element - place it prominently in the shop
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.15f);  // Center bottom area of shop
            buttonRect.anchorMax = new Vector2(0.5f, 0.15f);
            buttonRect.anchoredPosition = Vector2.zero;
            buttonRect.sizeDelta = new Vector2(400, 60); // Even bigger button
            
            // Add visual components
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0f, 0.9f, 0.2f, 1f); // Bright green, fully opaque
            
            Button button = buttonObj.AddComponent<Button>();
            
            // Create text for the button
            GameObject textObj = new GameObject("Button Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = Vector2.zero;
            
            Text buttonText = textObj.AddComponent<Text>();
            buttonText.fontSize = 22; // Bigger text
            buttonText.fontStyle = FontStyle.Bold;
            buttonText.color = Color.white;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // Check if player already owns Soldier 66 (Agent.Soldier)
            bool alreadyOwned = false;
            bool needsRefund = false;
            if (PlayerInventory.Instance != null)
            {
                alreadyOwned = PlayerInventory.Instance.HasItem("Agent.Soldier");
                // Check if they have the wrong ID (meaning they paid but didn't get the right item)
                needsRefund = PlayerInventory.Instance.HasItem("Soldier 66") && !alreadyOwned;
            }
            
            if (alreadyOwned)
            {
                buttonText.text = "✅ SOLDIER 66 - OWNED! ✅";
                buttonText.color = Color.yellow;
                button.interactable = false;
                buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 0.9f); // Gray out
            }
            else if (needsRefund)
            {
                buttonText.text = "🔧 FIX OWNERSHIP (FREE) 🔧";
                buttonText.color = Color.cyan;
            }
            else if (CurrencyManager.Instance != null && CurrencyManager.Instance.CurrentCoins < 400)
            {
                buttonText.text = "💸 NOT ENOUGH COINS (400 NEEDED) 💸";
                buttonText.color = Color.red;
                button.interactable = false;
                buttonImage.color = new Color(0.5f, 0.2f, 0.2f, 0.9f); // Red tint
            }
            else
            {
                buttonText.text = "🛒 BUY SOLDIER 66 - 400 COINS 🛒";
            }
            
            // Set up button functionality
            button.onClick.AddListener(() => {
                // Create a temporary ShopItem for Soldier 66 with the correct mapping
                ShopItem soldier66Item = ScriptableObject.CreateInstance<ShopItem>();
                soldier66Item.itemName = "Soldier 66";
                soldier66Item.name = "Soldier"; // This is what PlayerInventory uses for mapping to "Agent.Soldier"
                soldier66Item.description = "Elite soldier character";
                soldier66Item.cost = 400;
                soldier66Item.isAvailable = true;
                soldier66Item.itemType = ShopItemType.CharacterSkin;
                soldier66Item.rarity = ShopItemRarity.Common;
                
                // Check what action we need to take
                bool needsRefund = PlayerInventory.Instance != null && 
                                 PlayerInventory.Instance.HasItem("Soldier 66") && 
                                 !PlayerInventory.Instance.HasItem("Agent.Soldier");
                
                if (needsRefund)
                {
                    // Fix the ownership by converting "Soldier 66" to "Agent.Soldier" (free fix)
                    PlayerInventory.Instance.FixSoldier66Ownership();
                    
                    buttonText.text = "✅ SOLDIER 66 - FIXED & OWNED! ✅";
                    buttonText.color = Color.yellow;
                    button.interactable = false;
                    buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 0.9f); // Gray out
                    
                    Debug.Log("🔧 Fixed Soldier 66 ownership mapping for free!");
                }
                else if (CurrencyManager.Instance != null && CurrencyManager.Instance.CurrentCoins >= 400)
                {
                    // Normal purchase flow
                    if (PlayerInventory.Instance != null)
                    {
                        bool success = PlayerInventory.Instance.PurchaseItem(soldier66Item);
                        if (success)
                        {
                            Debug.Log("🎉 Successfully purchased Soldier 66!");
                            
                            // Update button text to show ownership
                            buttonText.text = "✅ SOLDIER 66 - OWNED! ✅";
                            buttonText.color = Color.yellow;
                            button.interactable = false;
                            buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 0.9f); // Gray out
                        }
                        else
                        {
                            Debug.Log("❌ Purchase failed - already owned or other error");
                            buttonText.text = "⚠️ ALREADY OWNED OR ERROR!";
                            buttonText.color = Color.red;
                        }
                    }
                    else
                    {
                        Debug.Log("❌ Purchase failed - PlayerInventory not found");
                        buttonText.text = "❌ SYSTEM ERROR!";
                        buttonText.color = Color.red;
                    }
                }
                else
                {
                    Debug.Log("❌ Purchase failed - not enough coins");
                    buttonText.text = "💸 NOT ENOUGH COINS! NEED 400";
                    buttonText.color = Color.red;
                }
            });
            
            // Store reference
            _mainPurchaseButton = button;
            
            Debug.Log("🎮 Created main purchase button - look for the big green button!");
        }
    }
}