using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TempInstaller : MonoBehaviour
{
    void Start()
    {
        CreateShopUI();
        Destroy(gameObject, 1f);
    }
    
    void CreateShopUI()
    {
        // Create Shop Canvas
        GameObject canvasGO = new GameObject("Shop Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Create Shop Panel
        GameObject shopPanel = new GameObject("Shop Panel");
        shopPanel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = shopPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image panelBg = shopPanel.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        shopPanel.SetActive(false);
        
        // Create Header
        GameObject header = new GameObject("Shop Header");
        header.transform.SetParent(shopPanel.transform, false);
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.anchoredPosition = new Vector2(0, -40);
        headerRect.sizeDelta = new Vector2(0, 60);
        
        TextMeshProUGUI headerText = header.AddComponent<TextMeshProUGUI>();
        headerText.text = "ITEM SHOP";
        headerText.fontSize = 42;
        headerText.fontStyle = FontStyles.Bold;
        headerText.color = Color.white;
        headerText.alignment = TextAlignmentOptions.Center;
        
        // Create Currency Display
        GameObject currencyDisplay = new GameObject("Currency Display");
        currencyDisplay.transform.SetParent(shopPanel.transform, false);
        RectTransform currencyRect = currencyDisplay.AddComponent<RectTransform>();
        currencyRect.anchorMin = new Vector2(1, 1);
        currencyRect.anchorMax = new Vector2(1, 1);
        currencyRect.anchoredPosition = new Vector2(-120, -20);
        currencyRect.sizeDelta = new Vector2(220, 50);
        
        Image currencyBg = currencyDisplay.AddComponent<Image>();
        currencyBg.color = new Color(0, 0, 0, 0.5f);
        
        TextMeshProUGUI currencyText = currencyDisplay.AddComponent<TextMeshProUGUI>();
        currencyText.text = "$1,000";
        currencyText.fontSize = 28;
        currencyText.fontStyle = FontStyles.Bold;
        currencyText.color = Color.yellow;
        currencyText.alignment = TextAlignmentOptions.Center;
        
        currencyDisplay.AddComponent<TPSBR.CurrencyDisplay>();
        
        // Create Close Button
        GameObject closeBtn = new GameObject("Close Button");
        closeBtn.transform.SetParent(shopPanel.transform, false);
        RectTransform closeRect = closeBtn.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1, 1);
        closeRect.anchorMax = new Vector2(1, 1);
        closeRect.anchoredPosition = new Vector2(-25, -25);
        closeRect.sizeDelta = new Vector2(40, 40);
        
        Button closeButton = closeBtn.AddComponent<Button>();
        Image closeBg = closeBtn.AddComponent<Image>();
        closeBg.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);
        
        GameObject closeText = new GameObject("X");
        closeText.transform.SetParent(closeBtn.transform, false);
        RectTransform closeTextRect = closeText.AddComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.offsetMin = Vector2.zero;
        closeTextRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI closeTextComp = closeText.AddComponent<TextMeshProUGUI>();
        closeTextComp.text = "×";
        closeTextComp.fontSize = 28;
        closeTextComp.fontStyle = FontStyles.Bold;
        closeTextComp.color = Color.white;
        closeTextComp.alignment = TextAlignmentOptions.Center;
        
        // Create Items Container
        GameObject container = new GameObject("Items Container");
        container.transform.SetParent(shopPanel.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.05f, 0.1f);
        containerRect.anchorMax = new Vector2(0.95f, 0.85f);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;
        
        GridLayoutGroup grid = container.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(220, 300);
        grid.spacing = new Vector2(15, 15);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 4;
        grid.childAlignment = TextAnchor.UpperCenter;
        
        // Setup Shop System
        GameObject shopSystem = GameObject.Find("Shop System");
        if (shopSystem == null)
        {
            shopSystem = new GameObject("Shop System");
        }
        
        if (!shopSystem.GetComponent<TPSBR.CurrencyManager>())
            shopSystem.AddComponent<TPSBR.CurrencyManager>();
            
        if (!shopSystem.GetComponent<TPSBR.PlayerInventory>())
            shopSystem.AddComponent<TPSBR.PlayerInventory>();
            
        if (!shopSystem.GetComponent<TPSBR.ShopManager>())
            shopSystem.AddComponent<TPSBR.ShopManager>();
        
        Debug.Log("✅ Shop UI System Created!");
        Debug.Log("📋 Next Steps:");
        Debug.Log("   1. Select 'Shop System' GameObject");
        Debug.Log("   2. In ShopManager component, assign:");
        Debug.Log($"      - Shop Panel: {shopPanel.name}");
        Debug.Log($"      - Item Container: {container.name}");
        Debug.Log($"      - Close Button: {closeButton.name}");
        Debug.Log("   3. Create ShopItem prefab and assign to Shop Item Prefab field");
        Debug.Log("   4. Create shop items: Right-click → Create → TPSBR → Shop Item");
        Debug.Log("   5. Press B to test the shop!");
    }
}