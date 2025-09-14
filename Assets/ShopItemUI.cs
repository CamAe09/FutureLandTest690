using UnityEngine;
using UnityEngine.UI;

namespace TPSBR
{
    public class ShopItemUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image _itemIcon;
        [SerializeField] private Text _itemName;
        [SerializeField] private Text _itemDescription;
        [SerializeField] private Text _itemCost;
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private Text _purchaseButtonText;
        [SerializeField] private Image _rarityBorder;
        [SerializeField] private GameObject _ownedIndicator;
        [SerializeField] private Image _coinIcon;
        
        private ShopItem _item;
        
        public void Setup(ShopItem item)
        {
            _item = item;
            
            // Comprehensive validation before proceeding
            if (_item == null) 
            {
                Debug.LogError("❌ ShopItem is null in Setup method!");
                return;
            }
            
            if (gameObject == null)
            {
                Debug.LogError("❌ GameObject is null in Setup method!");
                return;
            }
            
            if (transform == null)
            {
                Debug.LogError("❌ Transform is null in Setup method!");
                return;
            }
            
            try 
            {
                Debug.Log($"🛠️ Setting up simple shop item: {_item.itemName}");
                
                // Ensure this GameObject is properly set up as a UI element
                if (!EnsureProperUISetup())
                {
                    Debug.LogError("❌ Failed to setup UI properly - aborting item setup");
                    return;
                }
                
                // Create a simple visual layout
                CreateVisibleItemLayout();
                
                // Set icon color based on rarity (no sprite needed)
                if (_itemIcon != null)
                {
                    if (_item.icon != null)
                    {
                        _itemIcon.sprite = _item.icon;
                        _itemIcon.color = Color.white;
                    }
                    else
                    {
                        // No sprite, use rarity color
                        _itemIcon.color = GetRarityColor(_item.rarity);
                    }
                }
                
                // Setup purchase button click functionality
                if (_purchaseButton != null)
                {
                    _purchaseButton.onClick.RemoveAllListeners();
                    _purchaseButton.onClick.AddListener(() => {
                        Debug.Log($"🛒 Clicked on {_item.itemName}!");
                        OnPurchaseClicked();
                    });
                }
                
                // Update text content after layout is created
                UpdateItemContent();
                
                // Store item reference for purchase processing
                gameObject.name = $"Shop Item - {_item.itemName}";
                
                Debug.Log($"✅ Setup complete for simple shop item: {_item.itemName} - {_item.cost} coins");
                Debug.Log($"   Item can be clicked to purchase!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error in ShopItemUI.Setup: {e.Message}");
                Debug.LogError($"❌ Stack trace: {e.StackTrace}");
            }
        }
        
        private void UpdateItemContent()
        {
            try
            {
                if (_item == null) return;
                
                // Update name text
                if (_itemName != null)
                {
                    _itemName.text = _item.itemName;
                }
                
                // Update cost text
                if (_itemCost != null)
                {
                    _itemCost.text = $"{_item.cost} Coins";
                }
                
                // Update description if available
                if (_itemDescription != null)
                {
                    _itemDescription.text = _item.description;
                }
                
                Debug.Log($"📝 Updated item content for: {_item.itemName}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error updating item content: {e.Message}");
            }
        }
        
        private bool EnsureProperUISetup()
        {
            try
            {
                // Validate core components exist
                if (gameObject == null)
                {
                    Debug.LogError("❌ GameObject is null in EnsureProperUISetup!");
                    return false;
                }
                
                if (transform == null)
                {
                    Debug.LogError("❌ Transform is null in EnsureProperUISetup!");
                    return false;
                }
                
                Debug.Log($"🔍 Ensuring proper UI setup for: {gameObject.name}");
                
                // Ensure we have RectTransform (required for UI)
                RectTransform rect = GetComponent<RectTransform>();
                if (rect == null)
                {
                    Debug.Log("   Adding RectTransform component...");
                    rect = gameObject.AddComponent<RectTransform>();
                    if (rect == null)
                    {
                        Debug.LogError("❌ Failed to add RectTransform component!");
                        return false;
                    }
                    Debug.Log("   ✅ Successfully added RectTransform");
                }
                
                // Ensure proper Canvas setup
                Canvas parentCanvas = GetComponentInParent<Canvas>();
                if (parentCanvas == null)
                {
                    Debug.LogWarning("⚠️ No Canvas found in parent hierarchy - UI may not display correctly");
                }
                
                Debug.Log($"   ✅ UI setup validated for: {gameObject.name}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error in EnsureProperUISetup: {e.Message}");
                Debug.LogError($"❌ Stack trace: {e.StackTrace}");
                return false;
            }
        }
        
        private void CreateVisibleItemLayout()
        {
            try
            {
                Debug.Log($"🎨 Creating simple visual layout for: {gameObject.name}");
                
                // Ensure we have a proper UI setup
                if (gameObject == null)
                {
                    Debug.LogError("❌ GameObject is null!");
                    return;
                }
                
                // Get or create RectTransform
                RectTransform rect = GetComponent<RectTransform>();
                if (rect == null) 
                {
                    Debug.Log("   Adding RectTransform component...");
                    rect = gameObject.AddComponent<RectTransform>();
                }
                
                // Set size
                rect.sizeDelta = new Vector2(180, 220);
                
                // Add background image
                Image bgImage = GetComponent<Image>();
                if (bgImage == null) 
                {
                    Debug.Log("   Adding Image component...");
                    bgImage = gameObject.AddComponent<Image>();
                }
                bgImage.color = new Color(1f, 1f, 1f, 1f); // Bright white background for visibility
                
                // Make the entire item clickable by adding Button component
                if (_purchaseButton == null)
                {
                    _purchaseButton = GetComponent<Button>();
                    if (_purchaseButton == null) 
                    {
                        Debug.Log("   Adding Button component...");
                        _purchaseButton = gameObject.AddComponent<Button>();
                    }
                }
                
                if (_purchaseButton != null)
                {
                    _purchaseButton.targetGraphic = bgImage; // Use background as button graphic
                    
                    // Add button visual effects with bright colors
                    var colors = _purchaseButton.colors;
                    colors.normalColor = new Color(1f, 1f, 1f, 1f); // Bright white
                    colors.highlightedColor = new Color(0.8f, 0.9f, 1f, 1f); // Light blue on hover
                    colors.pressedColor = new Color(0.6f, 0.8f, 1f, 1f); // Blue on press
                    colors.selectedColor = colors.highlightedColor;
                    _purchaseButton.colors = colors;
                    
                    Debug.Log($"🔘 Added Button component to {gameObject.name}");
                }
                
                // Create simple visual areas (no text needed)
                CreateSimpleIconArea();
                CreateSimpleNameArea();
                CreateSimpleDescArea();
                CreateSimplePriceArea();
                
                Debug.Log($"✅ Created simple visual layout for shop item: {gameObject.name}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating shop item layout: {e.Message}");
                Debug.LogError($"Stack trace: {e.StackTrace}");
            }
        }
        
        private void CreateSimpleIconArea()
        {
            try
            {
                GameObject iconArea = new GameObject("Item Icon");
                iconArea.transform.SetParent(transform, false);
                RectTransform iconRect = iconArea.AddComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(0.1f, 0.5f);
                iconRect.anchorMax = new Vector2(0.9f, 0.9f);
                iconRect.anchoredPosition = Vector2.zero;
                iconRect.sizeDelta = Vector2.zero;
                
                _itemIcon = iconArea.AddComponent<Image>();
                _itemIcon.color = Color.white; // Default white, will be set to rarity color
                _itemIcon.raycastTarget = false; // Don't block button clicks
                
                Debug.Log("   ✅ Created simple icon area");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating icon area: {e.Message}");
            }
        }
        
        private void CreateSimpleNameArea()
        {
            try
            {
                if (transform == null)
                {
                    Debug.LogError("❌ Transform is null when creating name area!");
                    return;
                }
                
                GameObject nameArea = new GameObject("Name Area");
                nameArea.transform.SetParent(transform, false);
                RectTransform nameRect = nameArea.AddComponent<RectTransform>();
                nameRect.anchorMin = new Vector2(0.05f, 0.35f);
                nameRect.anchorMax = new Vector2(0.95f, 0.48f);
                nameRect.anchoredPosition = Vector2.zero;
                nameRect.sizeDelta = Vector2.zero;
                
                Image nameBg = nameArea.AddComponent<Image>();
                nameBg.color = new Color(1f, 1f, 1f, 0.8f); // White background for name
                nameBg.raycastTarget = false;
                
                // Create separate child GameObject for text
                GameObject textChild = new GameObject("Name Text");
                textChild.transform.SetParent(nameArea.transform, false);
                RectTransform textRect = textChild.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.anchoredPosition = Vector2.zero;
                textRect.sizeDelta = Vector2.zero;
                
                Text nameText = textChild.AddComponent<Text>();
                if (nameText != null)
                {
                    nameText.text = "Loading..."; // Placeholder text
                    nameText.color = Color.black;
                    nameText.fontSize = 14;
                    nameText.alignment = TextAnchor.MiddleCenter;
                    nameText.raycastTarget = false;
                    nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                _itemName = nameText;
                
                Debug.Log("   ✅ Created simple name area with text component");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating name area: {e.Message}");
                Debug.LogError($"❌ Stack trace: {e.StackTrace}");
            }
        }
        
        private void CreateSimpleDescArea()
        {
            try
            {
                if (transform == null)
                {
                    Debug.LogError("❌ Transform is null when creating description area!");
                    return;
                }
                
                GameObject descArea = new GameObject("Description Area");
                descArea.transform.SetParent(transform, false);
                RectTransform descRect = descArea.AddComponent<RectTransform>();
                descRect.anchorMin = new Vector2(0.05f, 0.25f);
                descRect.anchorMax = new Vector2(0.95f, 0.35f);
                descRect.anchoredPosition = Vector2.zero;
                descRect.sizeDelta = Vector2.zero;
                
                Image descBg = descArea.AddComponent<Image>();
                descBg.color = new Color(0.8f, 0.8f, 0.8f, 0.6f); // Gray background for description
                descBg.raycastTarget = false;
                
                // Create separate child GameObject for text
                GameObject textChild = new GameObject("Description Text");
                textChild.transform.SetParent(descArea.transform, false);
                RectTransform textRect = textChild.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.anchoredPosition = Vector2.zero;
                textRect.sizeDelta = Vector2.zero;
                
                Text descText = textChild.AddComponent<Text>();
                if (descText != null)
                {
                    descText.text = "Loading..."; // Placeholder text
                    descText.color = Color.black;
                    descText.fontSize = 10;
                    descText.alignment = TextAnchor.MiddleCenter;
                    descText.raycastTarget = false;
                    descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                _itemDescription = descText;
                
                Debug.Log("   ✅ Created simple description area with text component");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating description area: {e.Message}");
                Debug.LogError($"❌ Stack trace: {e.StackTrace}");
            }
        }
        
        private void CreateSimplePriceArea()
        {
            try
            {
                if (transform == null)
                {
                    Debug.LogError("❌ Transform is null when creating price area!");
                    return;
                }
                
                GameObject priceArea = new GameObject("Price Area");
                priceArea.transform.SetParent(transform, false);
                RectTransform priceRect = priceArea.AddComponent<RectTransform>();
                priceRect.anchorMin = new Vector2(0.05f, 0.05f);
                priceRect.anchorMax = new Vector2(0.95f, 0.25f);
                priceRect.anchoredPosition = Vector2.zero;
                priceRect.sizeDelta = Vector2.zero;
                
                Image priceBg = priceArea.AddComponent<Image>();
                priceBg.color = new Color(1f, 1f, 0f, 0.8f); // Yellow background for price
                priceBg.raycastTarget = false;
                
                // Create separate child GameObject for text
                GameObject textChild = new GameObject("Price Text");
                textChild.transform.SetParent(priceArea.transform, false);
                RectTransform textRect = textChild.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.anchoredPosition = Vector2.zero;
                textRect.sizeDelta = Vector2.zero;
                
                Text costText = textChild.AddComponent<Text>();
                if (costText != null)
                {
                    costText.text = "Loading..."; // Placeholder text
                    costText.color = Color.black;
                    costText.fontSize = 16;
                    costText.fontStyle = FontStyle.Bold;
                    costText.alignment = TextAnchor.MiddleCenter;
                    costText.raycastTarget = false;
                    costText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                _itemCost = costText;
                
                Debug.Log("   ✅ Created simple price area with text component");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating price area: {e.Message}");
                Debug.LogError($"❌ Stack trace: {e.StackTrace}");
            }
        }
        
        private void CreateIconArea()
        {
            if (_itemIcon != null) return;
            
            try
            {
                GameObject iconArea = new GameObject("Item Icon");
                iconArea.transform.SetParent(transform, false);
                RectTransform iconRect = iconArea.AddComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(0.1f, 0.5f);
                iconRect.anchorMax = new Vector2(0.9f, 0.9f);
                iconRect.anchoredPosition = Vector2.zero;
                iconRect.sizeDelta = Vector2.zero;
                
                _itemIcon = iconArea.AddComponent<Image>();
                _itemIcon.color = Color.white;
                _itemIcon.raycastTarget = false; // Don't block button clicks
                
                Debug.Log("   Created icon area");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating icon area: {e.Message}");
            }
        }
        
        private void CreateNameArea()
        {
            if (_itemName != null) return;
            
            try
            {
                GameObject nameArea = new GameObject("Item Name");
                nameArea.transform.SetParent(transform, false);
                RectTransform nameRect = nameArea.AddComponent<RectTransform>();
                nameRect.anchorMin = new Vector2(0.05f, 0.35f);
                nameRect.anchorMax = new Vector2(0.95f, 0.48f);
                nameRect.anchoredPosition = Vector2.zero;
                nameRect.sizeDelta = Vector2.zero;
                
                // Use regular Text component for compatibility
                _itemName = nameArea.AddComponent<Text>();
                if (_itemName == null)
                {
                    Debug.LogWarning("⚠️ Text component failed for name, using basic approach...");
                    // Create minimal colored background instead
                    Image nameBg = nameArea.AddComponent<Image>();
                    nameBg.color = Color.white;
                    Debug.Log("   Created name area with minimal display");
                    return;
                }
                
                _itemName.text = "ITEM NAME";
                _itemName.fontSize = 14;
                _itemName.color = Color.white;
                _itemName.fontStyle = FontStyle.Bold;
                _itemName.alignment = TextAnchor.MiddleCenter;
                _itemName.raycastTarget = false; // Don't block button clicks
                _itemName.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                
                Debug.Log("   Created name area");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating name area: {e.Message}");
            }
        }
        
        private void CreateDescriptionArea()
        {
            if (_itemDescription != null) return;
            
            try
            {
                GameObject descArea = new GameObject("Item Description");
                descArea.transform.SetParent(transform, false);
                RectTransform descRect = descArea.AddComponent<RectTransform>();
                descRect.anchorMin = new Vector2(0.05f, 0.25f);
                descRect.anchorMax = new Vector2(0.95f, 0.35f);
                descRect.anchoredPosition = Vector2.zero;
                descRect.sizeDelta = Vector2.zero;
                
                // Use regular Text component for compatibility
                _itemDescription = descArea.AddComponent<Text>();
                if (_itemDescription == null)
                {
                    Debug.LogWarning("⚠️ Text component failed for description, using basic approach...");
                    // Create minimal colored background instead
                    Image descBg = descArea.AddComponent<Image>();
                    descBg.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
                    Debug.Log("   Created description area with minimal display");
                    return;
                }
                
                _itemDescription.text = "Item Type";
                _itemDescription.fontSize = 10;
                _itemDescription.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                _itemDescription.alignment = TextAnchor.MiddleCenter;
                _itemDescription.raycastTarget = false; // Don't block button clicks
                _itemDescription.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                
                Debug.Log("   Created description area");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating description area: {e.Message}");
            }
        }
        
        private void CreatePriceArea()
        {
            if (_itemCost != null) return;
            
            try
            {
                // Validate prerequisites
                if (gameObject == null)
                {
                    Debug.LogError("❌ GameObject is null in CreatePriceArea!");
                    return;
                }
                
                if (transform == null)
                {
                    Debug.LogError("❌ Transform is null in CreatePriceArea!");
                    return;
                }
                
                GameObject priceArea = new GameObject("Price Section");
                if (priceArea == null)
                {
                    Debug.LogError("❌ Failed to create Price Section GameObject!");
                    return;
                }
                
                priceArea.transform.SetParent(transform, false);
                RectTransform priceRect = priceArea.AddComponent<RectTransform>();
                if (priceRect == null)
                {
                    Debug.LogError("❌ Failed to add RectTransform to Price Section!");
                    return;
                }
                
                priceRect.anchorMin = new Vector2(0.05f, 0.05f);
                priceRect.anchorMax = new Vector2(0.95f, 0.25f);
                priceRect.anchoredPosition = Vector2.zero;
                priceRect.sizeDelta = Vector2.zero;
                
                // Add buy button background
                Image priceBackground = priceArea.AddComponent<Image>();
                if (priceBackground != null)
                {
                    priceBackground.color = new Color(0.2f, 0.6f, 0.9f, 0.8f); // Blue button background
                    priceBackground.raycastTarget = false; // Don't block main button clicks
                }
                
                // Use regular Text component for compatibility
                _itemCost = priceArea.AddComponent<Text>();
                if (_itemCost == null)
                {
                    Debug.LogWarning("⚠️ Text component failed, creating minimal display...");
                    Debug.LogError("❌ Text component creation failed!");
                    return;
                }
                
                // Configure Text component
                _itemCost.text = "BUY - 500";
                _itemCost.fontSize = 12;
                _itemCost.color = Color.white;
                _itemCost.fontStyle = FontStyle.Bold;
                _itemCost.alignment = TextAnchor.MiddleCenter;
                _itemCost.raycastTarget = false; // Don't block button clicks
                _itemCost.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                
                // Store reference for purchase button text updates
                _purchaseButtonText = _itemCost;
                
                Debug.Log("   Created price area successfully with TextMeshProUGUI");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error creating price area: {e.Message}");
                Debug.LogError($"❌ Stack trace: {e.StackTrace}");
                
                // Try creating a minimal version as last resort
                CreateMinimalPriceDisplay();
            }
        }
        
        private void CreateMinimalPriceDisplay()
        {
            try
            {
                Debug.Log("   Attempting minimal price display...");
                
                GameObject simplePrice = new GameObject("Simple Price");
                simplePrice.transform.SetParent(transform, false);
                
                RectTransform rect = simplePrice.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.1f, 0.1f);
                rect.anchorMax = new Vector2(0.9f, 0.3f);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
                
                // Just add a colored background to show something exists
                Image bg = simplePrice.AddComponent<Image>();
                bg.color = Color.yellow;
                
                Debug.Log("   ✅ Created minimal price display");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Even minimal price display failed: {e.Message}");
            }
        }
        
        private string GetItemTypeDisplayName(ShopItemType itemType)
        {
            switch (itemType)
            {
                case ShopItemType.Weapon: return "Weapon";
                case ShopItemType.WeaponSkin: return "Weapon Skin";
                case ShopItemType.CharacterSkin: return "Outfit";
                case ShopItemType.Cosmetic: return "Cosmetic";
                case ShopItemType.Consumable: return "Consumable";
                case ShopItemType.Currency: return "Currency";
                default: return "Item";
            }
        }
        
        private Color GetRarityColor(ShopItemRarity rarity)
        {
            switch (rarity)
            {
                case ShopItemRarity.Common: return new Color(0.6f, 0.6f, 0.6f, 1f); // Gray
                case ShopItemRarity.Uncommon: return new Color(0.2f, 0.8f, 0.2f, 1f); // Green
                case ShopItemRarity.Rare: return new Color(0.2f, 0.6f, 1f, 1f); // Blue
                case ShopItemRarity.Epic: return new Color(0.8f, 0.3f, 1f, 1f); // Purple
                case ShopItemRarity.Legendary: return new Color(1f, 0.6f, 0.1f, 1f); // Orange
                default: return _item.rarityColor; // Fallback to item's color
            }
        }
        
        public void UpdateItemState()
        {
            if (_item == null) return;
            
            bool isOwned = PlayerInventory.Instance.HasItem(_item.name);
            bool canAfford = CurrencyManager.Instance.CanAfford(_item.cost);
            
            // Update owned indicator
            if (_ownedIndicator != null)
                _ownedIndicator.SetActive(isOwned);
                
            // Update purchase button
            if (_purchaseButton != null)
            {
                _purchaseButton.interactable = !isOwned && canAfford;
                
                if (_purchaseButtonText != null)
                {
                    if (isOwned)
                    {
                        _purchaseButtonText.text = "OWNED";
                        _purchaseButtonText.color = Color.green;
                    }
                    else if (canAfford)
                    {
                        _purchaseButtonText.text = "BUY";
                        _purchaseButtonText.color = Color.white;
                    }
                    else
                    {
                        _purchaseButtonText.text = "CAN'T AFFORD";
                        _purchaseButtonText.color = Color.red;
                    }
                }
            }
        }
        
        private void OnPurchaseClicked()
        {
            if (_item == null) 
            {
                Debug.LogError("❌ Cannot purchase: _item is null!");
                return;
            }
            
            Debug.Log($"🛒 Purchase attempt for: {_item.itemName} (Cost: {_item.cost} coins)");
            Debug.Log($"   Player current coins: {CurrencyManager.Instance.CurrentCoins}");
            Debug.Log($"   Player can afford: {CurrencyManager.Instance.CanAfford(_item.cost)}");
            
            // Get the correct agent ID for ownership check
            string agentID = GetAgentIDFromShopItem(_item);
            Debug.Log($"   Shop item '{_item.name}' maps to agent ID: {agentID}");
            Debug.Log($"   Player already owns: {PlayerInventory.Instance.HasItem(agentID)}");
            
            if (PlayerInventory.Instance.PurchaseItem(_item))
            {
                UpdateItemState();
                
                // Refresh all shop items
                ShopManager.Instance.RefreshShopItems();
                
                Debug.Log($"✅ Successfully purchased: {_item.itemName}");
                Debug.Log($"   Remaining coins: {CurrencyManager.Instance.CurrentCoins}");
            }
            else
            {
                Debug.LogWarning($"❌ Failed to purchase {_item.itemName}");
            }
        }
        
        private void OnEnable()
        {
            CurrencyManager.OnCurrencyChanged += OnCurrencyChanged;
            PlayerInventory.OnItemPurchased += OnItemPurchased;
        }
        
        private void OnDisable()
        {
            CurrencyManager.OnCurrencyChanged -= OnCurrencyChanged;
            PlayerInventory.OnItemPurchased -= OnItemPurchased;
        }
        
        private void OnCurrencyChanged(int newAmount)
        {
            UpdateItemState();
        }
        
        private void OnItemPurchased(string itemId)
        {
            UpdateItemState();
        }
        
        private string GetAgentIDFromShopItem(ShopItem item)
        {
            // Map shop item names to proper agent IDs (same mapping as PlayerInventory)
            switch (item.name)
            {
                case "Soldier":
                    return "Agent.Soldier";
                default:
                    // For other items, use the item name as-is
                    return item.name;
            }
        }
    }
}