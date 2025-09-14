using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TPSBR
{
    public class FortniteStyleShopUI : MonoBehaviour
    {
        [Header("Shop Setup")]
        public Transform parentCanvas;
        public Transform itemContainer;
        
        public static void CreateFullScreenShop(Transform parent, Transform itemContainer)
        {
            // Set parent to full screen
            RectTransform parentRect = parent.GetComponent<RectTransform>();
            if (parentRect != null)
            {
                parentRect.localScale = Vector3.one;
                parentRect.anchorMin = Vector2.zero;
                parentRect.anchorMax = Vector2.one;
                parentRect.anchoredPosition = Vector2.zero;
                parentRect.sizeDelta = Vector2.zero;
            }
            
            // Create background
            CreateBackground(parent);
            
            // Create header
            CreateHeader(parent);
            
            // Create tabs
            CreateTabs(parent);
            
            // Setup item grid
            SetupItemGrid(itemContainer);
            
            Debug.Log("✅ Created Fortnite-style full-screen shop UI");
        }
        
        private static void CreateBackground(Transform parent)
        {
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(parent, false);
            
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.anchoredPosition = Vector2.zero;
            bgRect.sizeDelta = Vector2.zero;
            
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.2f, 0.4f, 0.95f); // Dark blue like Fortnite
        }
        
        private static void CreateHeader(Transform parent)
        {
            GameObject header = new GameObject("Header");
            header.transform.SetParent(parent, false);
            
            RectTransform headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.anchoredPosition = new Vector2(0, -30);
            headerRect.sizeDelta = new Vector2(0, 60);
            
            Image headerBG = header.AddComponent<Image>();
            headerBG.color = new Color(0.05f, 0.1f, 0.2f, 0.8f);
            
            // Currency display
            CreateCurrencyDisplay(header.transform);
            
            // Close button
            CreateCloseButton(header.transform);
            
            // Title
            GameObject title = new GameObject("Title");
            title.transform.SetParent(header.transform, false);
            
            RectTransform titleRect = title.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
            titleText.text = "ITEM SHOP";
            titleText.fontSize = 24;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.fontStyle = FontStyles.Bold;
        }
        
        private static void CreateCurrencyDisplay(Transform parent)
        {
            GameObject currencyPanel = new GameObject("Currency Panel");
            currencyPanel.transform.SetParent(parent, false);
            
            RectTransform currencyRect = currencyPanel.AddComponent<RectTransform>();
            currencyRect.anchorMin = new Vector2(1, 0.5f);
            currencyRect.anchorMax = new Vector2(1, 0.5f);
            currencyRect.anchoredPosition = new Vector2(-100, 0);
            currencyRect.sizeDelta = new Vector2(180, 40);
            
            Image currencyBG = currencyPanel.AddComponent<Image>();
            currencyBG.color = new Color(0.1f, 0.1f, 0.3f, 0.8f);
            currencyBG.sprite = CreateRoundedSprite();
            
            // Currency icon
            GameObject icon = new GameObject("Currency Icon");
            icon.transform.SetParent(currencyPanel.transform, false);
            
            RectTransform iconRect = icon.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0.5f);
            iconRect.anchorMax = new Vector2(0, 0.5f);
            iconRect.anchoredPosition = new Vector2(20, 0);
            iconRect.sizeDelta = new Vector2(24, 24);
            
            Image iconImage = icon.AddComponent<Image>();
            iconImage.color = Color.yellow;
            
            // Currency text
            GameObject currencyText = new GameObject("Currency Text");
            currencyText.transform.SetParent(currencyPanel.transform, false);
            
            RectTransform textRect = currencyText.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.3f, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI text = currencyText.AddComponent<TextMeshProUGUI>();
            text.text = "1,250";
            text.fontSize = 18;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Left;
            text.fontStyle = FontStyles.Bold;
        }
        
        private static void CreateCloseButton(Transform parent)
        {
            GameObject closeBtn = new GameObject("Close Button");
            closeBtn.transform.SetParent(parent, false);
            
            RectTransform btnRect = closeBtn.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0, 0.5f);
            btnRect.anchorMax = new Vector2(0, 0.5f);
            btnRect.anchoredPosition = new Vector2(40, 0);
            btnRect.sizeDelta = new Vector2(40, 40);
            
            Image btnImage = closeBtn.AddComponent<Image>();
            btnImage.color = new Color(0.7f, 0.2f, 0.2f, 0.8f);
            
            Button button = closeBtn.AddComponent<Button>();
            
            // X text
            GameObject xText = new GameObject("X Text");
            xText.transform.SetParent(closeBtn.transform, false);
            
            RectTransform xRect = xText.AddComponent<RectTransform>();
            xRect.anchorMin = Vector2.zero;
            xRect.anchorMax = Vector2.one;
            xRect.anchoredPosition = Vector2.zero;
            xRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI xTmp = xText.AddComponent<TextMeshProUGUI>();
            xTmp.text = "✖";
            xTmp.fontSize = 20;
            xTmp.color = Color.white;
            xTmp.alignment = TextAlignmentOptions.Center;
            xTmp.fontStyle = FontStyles.Bold;
        }
        
        private static void CreateTabs(Transform parent)
        {
            GameObject tabsPanel = new GameObject("Tabs Panel");
            tabsPanel.transform.SetParent(parent, false);
            
            RectTransform tabsRect = tabsPanel.AddComponent<RectTransform>();
            tabsRect.anchorMin = new Vector2(0, 1);
            tabsRect.anchorMax = new Vector2(1, 1);
            tabsRect.anchoredPosition = new Vector2(0, -100);
            tabsRect.sizeDelta = new Vector2(0, 50);
            
            HorizontalLayoutGroup tabsLayout = tabsPanel.AddComponent<HorizontalLayoutGroup>();
            tabsLayout.childAlignment = TextAnchor.MiddleCenter;
            tabsLayout.spacing = 10;
            tabsLayout.padding = new RectOffset(50, 50, 5, 5);
            
            // Create tabs
            string[] tabNames = { "FEATURED", "DAILY", "SPECIAL OFFERS" };
            Color[] tabColors = { 
                new Color(0.2f, 0.6f, 1f, 1f), // Blue for active
                new Color(0.3f, 0.3f, 0.3f, 1f), // Gray for inactive
                new Color(0.3f, 0.3f, 0.3f, 1f)
            };
            
            for (int i = 0; i < tabNames.Length; i++)
            {
                CreateTab(tabsPanel.transform, tabNames[i], tabColors[i], i == 0);
            }
        }
        
        private static void CreateTab(Transform parent, string tabName, Color color, bool isActive)
        {
            GameObject tab = new GameObject($"Tab - {tabName}");
            tab.transform.SetParent(parent, false);
            
            RectTransform tabRect = tab.AddComponent<RectTransform>();
            tabRect.sizeDelta = new Vector2(160, 40);
            
            Image tabImage = tab.AddComponent<Image>();
            tabImage.color = color;
            
            Button tabButton = tab.AddComponent<Button>();
            
            // Tab text
            GameObject tabText = new GameObject("Tab Text");
            tabText.transform.SetParent(tab.transform, false);
            
            RectTransform textRect = tabText.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI text = tabText.AddComponent<TextMeshProUGUI>();
            text.text = tabName;
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            text.fontStyle = FontStyles.Bold;
        }
        
        private static void SetupItemGrid(Transform itemContainer)
        {
            if (itemContainer == null) return;
            
            RectTransform containerRect = itemContainer.GetComponent<RectTransform>();
            if (containerRect != null)
            {
                containerRect.anchorMin = new Vector2(0, 0);
                containerRect.anchorMax = new Vector2(1, 1);
                containerRect.anchoredPosition = new Vector2(0, -75);
                containerRect.sizeDelta = new Vector2(-100, -200);
            }
            
            GridLayoutGroup grid = itemContainer.GetComponent<GridLayoutGroup>();
            if (grid == null)
            {
                grid = itemContainer.gameObject.AddComponent<GridLayoutGroup>();
            }
            
            grid.cellSize = new Vector2(200, 250);
            grid.spacing = new Vector2(15, 15);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.Flexible;
            grid.padding = new RectOffset(50, 50, 20, 20);
        }
        
        private static Sprite CreateRoundedSprite()
        {
            // Create a simple rounded rectangle sprite
            Texture2D texture = new Texture2D(32, 32);
            Color[] colors = new Color[32 * 32];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.white;
            }
            texture.SetPixels(colors);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        }
    }
}