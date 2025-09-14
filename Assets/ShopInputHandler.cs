using UnityEngine;
using TPSBR;

/// <summary>
/// Handles input for the shop system, ensuring proper escape key behavior
/// This works as a backup to the IBackHandler system
/// </summary>
public class ShopInputHandler : MonoBehaviour
{
    private ShopManager _shopManager;
    
    private void Start()
    {
        _shopManager = FindObjectOfType<ShopManager>();
        if (_shopManager == null)
        {
            Debug.LogWarning("⚠️ ShopInputHandler: No ShopManager found in scene");
            enabled = false;
        }
    }
    
    private void Update()
    {
        if (_shopManager == null || !_shopManager.IsShopOpen)
            return;
            
        // Handle escape key for shop
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("🔑 ShopInputHandler: Escape key pressed, closing shop");
            _shopManager.CloseShop();
        }
    }
}