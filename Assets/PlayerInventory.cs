using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TPSBR
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Inventory")]
        [SerializeField] private bool _debugMode = true;
        
        [Header("Debug Info (Read Only)")]
        [SerializeField, TextArea(3, 10)] private string _debugOwnedItems = "No items loaded yet...";
        
        // Non-serialized to prevent Inspector conflicts during runtime modifications
        private List<string> _ownedItems = new List<string>();

        public static PlayerInventory Instance { get; private set; }

        // Events
        public static event System.Action<string> OnItemPurchased;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadInventory();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // Ensure debug display is updated when the object starts
            UpdateDebugDisplay();
        }

        public bool HasItem(string itemId)
        {
            return _ownedItems.Contains(itemId);
        }

        public bool PurchaseItem(ShopItem item)
        {
            // Get the correct agent ID for this shop item
            string agentID = GetAgentIDFromShopItem(item);
            
            // Check if already owned
            if (HasItem(agentID))
            {
                if (_debugMode)
                {
                    Debug.Log($"⚠️ Already own item: {item.itemName} (Agent ID: {agentID})");
                }
                return false;
            }

            // Check if can afford
            if (!CurrencyManager.Instance.CanAfford(item.cost))
            {
                if (_debugMode)
                {
                    Debug.Log($"💸 Cannot afford {item.itemName} (Cost: {item.cost})");
                }
                return false;
            }

            // Purchase the item
            if (CurrencyManager.Instance.SpendCoins(item.cost))
            {
                _ownedItems.Add(agentID);
                SaveInventory();
                OnItemPurchased?.Invoke(agentID);

                if (_debugMode)
                {
                    Debug.Log($"✅ Purchased: {item.itemName} (Agent ID: {agentID}) for {item.cost} coins");
                }
                return true;
            }

            return false;
        }

        public List<string> GetOwnedItems()
        {
            return new List<string>(_ownedItems);
        }

        private string GetAgentIDFromShopItem(ShopItem item)
        {
            // Map shop item names to proper agent IDs
            switch (item.name)
            {
                case "Soldier":
                    return "Agent.Soldier";
                default:
                    // For other items, use the item name as-is
                    return item.name;
            }
        }

        private void LoadInventory()
        {
            string inventoryData = PlayerPrefs.GetString("PlayerInventory", "");
            if (!string.IsNullOrEmpty(inventoryData))
            {
                _ownedItems = inventoryData.Split(',').ToList();
            }

            UpdateDebugDisplay();

            if (_debugMode)
            {
                Debug.Log($"📦 Loaded inventory with {_ownedItems.Count} items");
            }
        }

        private void SaveInventory()
        {
            string inventoryData = string.Join(",", _ownedItems);
            PlayerPrefs.SetString("PlayerInventory", inventoryData);
            PlayerPrefs.Save();
            
            UpdateDebugDisplay();
        }
        
        private void UpdateDebugDisplay()
        {
            if (_ownedItems.Count == 0)
            {
                _debugOwnedItems = "No items owned yet.\nPurchase items from the shop to see them here.";
            }
            else
            {
                _debugOwnedItems = $"Owned Items ({_ownedItems.Count}):\n" + string.Join("\n", _ownedItems.Select((item, index) => $"  {index + 1}. {item}"));
            }
        }

        [ContextMenu("Clear Inventory")]
        public void ClearInventory()
        {
            _ownedItems.Clear();
            SaveInventory();
            Debug.Log("🧹 Inventory cleared");
        }
        
        [ContextMenu("Fix Soldier 66 Ownership")]
        public void FixSoldier66Ownership()
        {
            // If player has "Soldier 66" but not "Agent.Soldier", fix it
            if (HasItem("Soldier 66") && !HasItem("Agent.Soldier"))
            {
                _ownedItems.Remove("Soldier 66");
                _ownedItems.Add("Agent.Soldier");
                SaveInventory();
                Debug.Log("✅ Fixed Soldier 66 ownership - now mapped to Agent.Soldier");
            }
            else if (HasItem("Agent.Soldier"))
            {
                Debug.Log("✅ Already own Agent.Soldier - no fix needed");
            }
            else
            {
                Debug.Log("❌ Don't own Soldier 66 or Agent.Soldier");
            }
        }
        
        [ContextMenu("Debug Show All Items")]
        public void DebugShowAllItems()
        {
            Debug.Log($"📦 Current Inventory ({_ownedItems.Count} items):");
            for (int i = 0; i < _ownedItems.Count; i++)
            {
                Debug.Log($"   {i + 1}. {_ownedItems[i]}");
            }
            
            if (_ownedItems.Count == 0)
            {
                Debug.Log("   (Empty)");
            }
        }
    }
}