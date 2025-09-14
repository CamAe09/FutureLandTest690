using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TPSBR
{
    public class FortniteStyleShopItem : MonoBehaviour
    {
        public static GameObject CreateFortniteStyleItem(Transform parent)
        {
            GameObject itemCard = new GameObject("Fortnite Shop Item");
            itemCard.transform.SetParent(parent, false);
            
            RectTransform cardRect = itemCard.AddComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(200, 250);
            
            // Main card background
            Image cardBG = itemCard.AddComponent<Image>();
            cardBG.color = new Color(0.1f, 0.1f, 0.2f, 0.9f);
            
            // Rarity border (top part)
            CreateRarityBorder(itemCard.transform);
            
            // Item image area
            CreateItemImageArea(itemCard.transform);
            
            // Item name and info
            CreateItemInfo(itemCard.transform);
            
            // Price section
            CreatePriceSection(itemCard.transform);
            
            // Add hover effects
            AddHoverEffects(itemCard);
            
            // Add ShopItemUI component
            ShopItemUI shopItemUI = itemCard.AddComponent<ShopItemUI>();
            
            return itemCard;
        }
        
        private static void CreateRarityBorder(Transform parent)
        {
            GameObject rarityBorder = new GameObject("Rarity Border");
            rarityBorder.transform.SetParent(parent, false);
            
            RectTransform borderRect = rarityBorder.AddComponent<RectTransform>();
            borderRect.anchorMin = new Vector2(0, 0.7f);
            borderRect.anchorMax = new Vector2(1, 1);
            borderRect.anchoredPosition = Vector2.zero;
            borderRect.sizeDelta = Vector2.zero;
            
            Image borderImage = rarityBorder.AddComponent<Image>();
            borderImage.color = new Color(0.2f, 0.8f, 1f, 1f); // Default blue rarity
            
            // Gradient effect (top part)
            GameObject gradient = new GameObject("Gradient");
            gradient.transform.SetParent(rarityBorder.transform, false);
            
            RectTransform gradientRect = gradient.AddComponent<RectTransform>();
            gradientRect.anchorMin = new Vector2(0, 0);
            gradientRect.anchorMax = new Vector2(1, 0.3f);
            gradientRect.anchoredPosition = Vector2.zero;
            gradientRect.sizeDelta = Vector2.zero;
            
            Image gradientImage = gradient.AddComponent<Image>();
            gradientImage.color = new Color(0.2f, 0.8f, 1f, 0.3f);
        }
        
        private static void CreateItemImageArea(Transform parent)
        {
            GameObject imageArea = new GameObject("Item Image Area");
            imageArea.transform.SetParent(parent, false);
            
            RectTransform imageRect = imageArea.AddComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(0.1f, 0.4f);
            imageRect.anchorMax = new Vector2(0.9f, 0.85f);
            imageRect.anchoredPosition = Vector2.zero;
            imageRect.sizeDelta = Vector2.zero;
            
            Image itemImage = imageArea.AddComponent<Image>();
            itemImage.color = new Color(1f, 1f, 1f, 1f);
            
            // Placeholder image (you can replace this with actual item icons)
            GameObject placeholder = new GameObject("Placeholder");
            placeholder.transform.SetParent(imageArea.transform, false);
            
            RectTransform placeholderRect = placeholder.AddComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.anchoredPosition = Vector2.zero;
            placeholderRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI placeholderText = placeholder.AddComponent<TextMeshProUGUI>();
            placeholderText.text = "🎯";
            placeholderText.fontSize = 48;
            placeholderText.color = Color.white;
            placeholderText.alignment = TextAlignmentOptions.Center;
        }
        
        private static void CreateItemInfo(Transform parent)
        {
            GameObject infoArea = new GameObject("Item Info");
            infoArea.transform.SetParent(parent, false);
            
            RectTransform infoRect = infoArea.AddComponent<RectTransform>();
            infoRect.anchorMin = new Vector2(0, 0.25f);
            infoRect.anchorMax = new Vector2(1, 0.4f);
            infoRect.anchoredPosition = Vector2.zero;
            infoRect.sizeDelta = Vector2.zero;
            
            // Item name
            GameObject nameObj = new GameObject("Item Name");
            nameObj.transform.SetParent(infoArea.transform, false);
            
            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.5f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.anchoredPosition = Vector2.zero;
            nameRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "ITEM NAME";
            nameText.fontSize = 14;
            nameText.color = Color.white;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.fontStyle = FontStyles.Bold;
            
            // Item type
            GameObject typeObj = new GameObject("Item Type");
            typeObj.transform.SetParent(infoArea.transform, false);
            
            RectTransform typeRect = typeObj.AddComponent<RectTransform>();
            typeRect.anchorMin = new Vector2(0, 0);
            typeRect.anchorMax = new Vector2(1, 0.5f);
            typeRect.anchoredPosition = Vector2.zero;
            typeRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI typeText = typeObj.AddComponent<TextMeshProUGUI>();
            typeText.text = "Outfit";
            typeText.fontSize = 12;
            typeText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            typeText.alignment = TextAlignmentOptions.Center;
        }
        
        private static void CreatePriceSection(Transform parent)
        {
            GameObject priceArea = new GameObject("Price Section");
            priceArea.transform.SetParent(parent, false);
            
            RectTransform priceRect = priceArea.AddComponent<RectTransform>();
            priceRect.anchorMin = new Vector2(0, 0);
            priceRect.anchorMax = new Vector2(1, 0.25f);
            priceRect.anchoredPosition = Vector2.zero;
            priceRect.sizeDelta = Vector2.zero;
            
            Image priceBG = priceArea.AddComponent<Image>();
            priceBG.color = new Color(0.05f, 0.05f, 0.15f, 0.8f);
            
            // Currency icon
            GameObject currencyIcon = new GameObject("Currency Icon");
            currencyIcon.transform.SetParent(priceArea.transform, false);
            
            RectTransform iconRect = currencyIcon.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.2f, 0.5f);
            iconRect.anchorMax = new Vector2(0.2f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = new Vector2(20, 20);
            
            Image iconImage = currencyIcon.AddComponent<Image>();
            iconImage.color = Color.yellow;
            
            // Price text
            GameObject priceText = new GameObject("Price Text");
            priceText.transform.SetParent(priceArea.transform, false);
            
            RectTransform priceTextRect = priceText.AddComponent<RectTransform>();
            priceTextRect.anchorMin = new Vector2(0.3f, 0);
            priceTextRect.anchorMax = new Vector2(1, 1);
            priceTextRect.anchoredPosition = Vector2.zero;
            priceTextRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI price = priceText.AddComponent<TextMeshProUGUI>();
            price.text = "1,200";
            price.fontSize = 16;
            price.color = Color.white;
            price.alignment = TextAlignmentOptions.Left;
            price.fontStyle = FontStyles.Bold;
        }
        
        private static void AddHoverEffects(GameObject itemCard)
        {
            Button button = itemCard.AddComponent<Button>();
            
            // Add scale animation on hover
            itemCard.AddComponent<HoverEffect>();
        }
    }
    
    // Simple hover effect component
    public class HoverEffect : MonoBehaviour, UnityEngine.EventSystems.IPointerEnterHandler, UnityEngine.EventSystems.IPointerExitHandler
    {
        private Vector3 originalScale;
        private RectTransform rectTransform;
        
        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            originalScale = rectTransform.localScale;
        }
        
        public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            rectTransform.localScale = originalScale * 1.05f;
        }
        
        public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            rectTransform.localScale = originalScale;
        }
    }
}