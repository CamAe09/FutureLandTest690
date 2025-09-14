using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TPSBR
{
    public class SimpleShopItemCreator : MonoBehaviour
    {
        [ContextMenu("Create Simple Shop Item Prefab")]
        public void CreateSimpleShopItemPrefab()
        {
            GameObject prefab = CreateItemPrefab();
            if (prefab != null)
            {
                Debug.Log("✅ Simple Shop Item Prefab created!");
                Debug.Log("📋 Next Steps:");
                Debug.Log("   1. Drag this GameObject to Project folder to make it a prefab");
                Debug.Log("   2. Assign the prefab to ShopManager's 'Shop Item Prefab' field");
                Debug.Log("   3. You may need to manually assign UI references in the prefab's ShopItemUI component");
            }
        }
        
        public GameObject CreateItemPrefab()
        {
            // Create the main item GameObject
            GameObject itemPrefab = new GameObject("Shop Item");
            
            // Set up RectTransform
            RectTransform rect = itemPrefab.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 250);
            
            // Add background image
            Image bg = itemPrefab.AddComponent<Image>();
            bg.color = new Color(0.25f, 0.25f, 0.25f, 0.9f);
            
            // Create Item Name
            GameObject nameObj = new GameObject("Item Name");
            nameObj.transform.SetParent(itemPrefab.transform, false);
            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.7f);
            nameRect.anchorMax = new Vector2(1, 0.9f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "Item Name";
            nameText.fontSize = 16;
            nameText.fontStyle = FontStyles.Bold;
            nameText.color = Color.white;
            nameText.alignment = TextAlignmentOptions.Center;
            
            // Create Item Description  
            GameObject descObj = new GameObject("Item Description");
            descObj.transform.SetParent(itemPrefab.transform, false);
            RectTransform descRect = descObj.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0.4f);
            descRect.anchorMax = new Vector2(1, 0.7f);
            descRect.offsetMin = Vector2.zero;
            descRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
            descText.text = "Description";
            descText.fontSize = 12;
            descText.color = Color.gray;
            descText.alignment = TextAlignmentOptions.Center;
            descText.enableWordWrapping = true;
            
            // Create Cost Text
            GameObject costObj = new GameObject("Cost Text");
            costObj.transform.SetParent(itemPrefab.transform, false);
            RectTransform costRect = costObj.AddComponent<RectTransform>();
            costRect.anchorMin = new Vector2(0, 0.25f);
            costRect.anchorMax = new Vector2(1, 0.4f);
            costRect.offsetMin = Vector2.zero;
            costRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI costText = costObj.AddComponent<TextMeshProUGUI>();
            costText.text = "$100";
            costText.fontSize = 14;
            costText.fontStyle = FontStyles.Bold;
            costText.color = Color.yellow;
            costText.alignment = TextAlignmentOptions.Center;
            
            // Create Purchase Button
            GameObject buttonObj = new GameObject("Purchase Button");
            buttonObj.transform.SetParent(itemPrefab.transform, false);
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.1f, 0.05f);
            buttonRect.anchorMax = new Vector2(0.9f, 0.2f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            Button button = buttonObj.AddComponent<Button>();
            Image buttonBg = buttonObj.AddComponent<Image>();
            buttonBg.color = Color.green;
            
            // Button Text
            GameObject buttonTextObj = new GameObject("Button Text");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);
            RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = "BUY";
            buttonText.fontSize = 14;
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center;
            
            // Create empty objects for other components
            GameObject itemIcon = new GameObject("Item Icon");
            itemIcon.transform.SetParent(itemPrefab.transform, false);
            itemIcon.AddComponent<Image>();
            itemIcon.SetActive(false); // Hidden placeholder
            
            GameObject rarityBorder = new GameObject("Rarity Border");  
            rarityBorder.transform.SetParent(itemPrefab.transform, false);
            rarityBorder.AddComponent<Image>();
            rarityBorder.SetActive(false); // Hidden placeholder
            
            GameObject ownedIndicator = new GameObject("Owned Indicator");
            ownedIndicator.transform.SetParent(itemPrefab.transform, false);
            ownedIndicator.AddComponent<Image>();
            ownedIndicator.SetActive(false); // Hidden by default
            
            GameObject coinIcon = new GameObject("Coin Icon");
            coinIcon.transform.SetParent(itemPrefab.transform, false);
            coinIcon.AddComponent<Image>();
            coinIcon.SetActive(false); // Hidden placeholder
            
            // Add ShopItemUI component
            ShopItemUI shopItemUI = itemPrefab.AddComponent<ShopItemUI>();
            
            return itemPrefab;
        }
    }
}