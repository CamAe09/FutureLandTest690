using UnityEngine;

namespace TPSBR
{
    [CreateAssetMenu(fileName = "New Shop Item", menuName = "TPSBR/Shop Item")]
    public class ShopItem : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemName = "New Item";
        [TextArea(3, 5)]
        public string description = "Item description";
        public Sprite icon;
        
        [Header("Pricing")]
        public int cost = 100;
        public bool isAvailable = true;
        
        [Header("Item Type")]
        public ShopItemType itemType = ShopItemType.Cosmetic;
        
        [Header("Unlock Requirements")]
        public int requiredLevel = 1;
        public bool isPremium = false;
        
        [Header("Visual")]
        public Color rarityColor = Color.white;
        public ShopItemRarity rarity = ShopItemRarity.Common;
        
        // Optional prefab for 3D items
        [Header("3D Preview")]
        public GameObject itemPrefab;
    }
    
    public enum ShopItemType
    {
        Weapon,
        WeaponSkin,
        CharacterSkin,
        Cosmetic,
        Consumable,
        Currency
    }
    
    public enum ShopItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}