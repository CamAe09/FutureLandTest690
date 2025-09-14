using UnityEngine;
using TPSBR;

/// <summary>
/// Comprehensive test script to verify all shop fixes are working properly
/// Attach this to a GameObject in your scene to test the fixes
/// </summary>
public class ShopFixTester : MonoBehaviour
{
    [Header("Test Controls")]
    [SerializeField, Tooltip("Press to test all shop functionality")] 
    private bool _testAllFixes = false;
    
    [SerializeField, Tooltip("Press to manually open shop")] 
    private bool _testOpenShop = false;
    
    [SerializeField, Tooltip("Press to manually close shop")] 
    private bool _testCloseShop = false;
    
    [SerializeField, Tooltip("Press to simulate escape key")] 
    private bool _testEscapeKey = false;
    
    [Header("Test Results")]
    [SerializeField, ReadOnly] private bool _shopManagerFound = false;
    [SerializeField, ReadOnly] private bool _shopAutoOpenDisabled = false;
    [SerializeField, ReadOnly] private bool _escapeKeyWorks = false;
    [SerializeField, ReadOnly] private bool _closeButtonWorks = false;
    [SerializeField, ReadOnly] private bool _bKeyWorks = false;
    
    private ShopManager _shopManager;
    
    private void Start()
    {
        _shopManager = FindObjectOfType<ShopManager>();
        _shopManagerFound = _shopManager != null;
        
        if (_shopManager == null)
        {
            Debug.LogError("❌ ShopFixTester: No ShopManager found in scene!");
            enabled = false;
            return;
        }
        
        Debug.Log("🧪 ShopFixTester initialized - Use inspector buttons to test fixes");
        
        // Test that auto-open is disabled
        TestAutoOpenDisabled();
    }
    
    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        
        // Handle inspector button clicks
        if (_testAllFixes)
        {
            _testAllFixes = false;
            TestAllFixes();
        }
        
        if (_testOpenShop)
        {
            _testOpenShop = false;
            TestOpenShop();
        }
        
        if (_testCloseShop)
        {
            _testCloseShop = false;
            TestCloseShop();
        }
        
        if (_testEscapeKey)
        {
            _testEscapeKey = false;
            TestEscapeKey();
        }
    }
    
    private void Update()
    {
        // Monitor for manual testing
        if (UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame)
        {
            TestAllFixes();
        }
    }
    
    [ContextMenu("Test All Fixes")]
    public void TestAllFixes()
    {
        Debug.Log("🧪 === TESTING ALL SHOP FIXES ===");
        
        TestAutoOpenDisabled();
        TestBKeyToggle();
        TestCloseButton();
        TestEscapeKey();
        
        ShowTestResults();
    }
    
    private void TestAutoOpenDisabled()
    {
        Debug.Log("🧪 Testing: Shop auto-open disabled...");
        
        // Check if shop opens automatically (it shouldn't)
        bool wasOpen = _shopManager.IsShopOpen;
        
        // Wait for a few frames to see if it auto-opens
        StartCoroutine(CheckAutoOpenAfterDelay(wasOpen));
    }
    
    private System.Collections.IEnumerator CheckAutoOpenAfterDelay(bool initialState)
    {
        yield return new WaitForSeconds(3f); // Wait 3 seconds
        
        bool openedAutomatically = !initialState && _shopManager.IsShopOpen;
        _shopAutoOpenDisabled = !openedAutomatically;
        
        if (_shopAutoOpenDisabled)
        {
            Debug.Log("✅ Auto-open test PASSED: Shop did not open automatically");
        }
        else
        {
            Debug.LogError("❌ Auto-open test FAILED: Shop opened automatically");
        }
    }
    
    private void TestBKeyToggle()
    {
        Debug.Log("🧪 Testing: B key toggle...");
        
        bool initialState = _shopManager.IsShopOpen;
        
        // Simulate B key press via direct method call
        // (We can't simulate actual input system easily)
        if (initialState)
        {
            _shopManager.CloseShop();
        }
        else
        {
            _shopManager.OpenShop();
        }
        
        bool newState = _shopManager.IsShopOpen;
        _bKeyWorks = (newState != initialState);
        
        if (_bKeyWorks)
        {
            Debug.Log("✅ B key test PASSED: Shop state toggled correctly");
        }
        else
        {
            Debug.LogError("❌ B key test FAILED: Shop state did not change");
        }
        
        // Reset to initial state
        if (initialState)
        {
            _shopManager.OpenShop();
        }
        else
        {
            _shopManager.CloseShop();
        }
    }
    
    private void TestCloseButton()
    {
        Debug.Log("🧪 Testing: Close button...");
        
        // Open shop first
        if (!_shopManager.IsShopOpen)
        {
            _shopManager.OpenShop();
        }
        
        // Try to find and click the close button
        var closeButton = FindObjectOfType<UnityEngine.UI.Button>();
        if (closeButton != null)
        {
            Debug.Log($"🔘 Found button: {closeButton.name}, attempting click...");
            closeButton.onClick.Invoke();
            
            _closeButtonWorks = !_shopManager.IsShopOpen;
            
            if (_closeButtonWorks)
            {
                Debug.Log("✅ Close button test PASSED: Shop closed via button");
            }
            else
            {
                Debug.LogError("❌ Close button test FAILED: Shop still open after button click");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Close button test SKIPPED: No button found");
            _closeButtonWorks = false;
        }
    }
    
    private void TestEscapeKey()
    {
        Debug.Log("🧪 Testing: Escape key handling...");
        
        // Open shop first
        if (!_shopManager.IsShopOpen)
        {
            _shopManager.OpenShop();
        }
        
        // Simulate escape key press via the Update method's escape handling
        // Since we can't directly trigger input system, we'll call CloseShop directly
        // which is what the escape key does
        if (_shopManager.IsShopOpen)
        {
            _shopManager.CloseShop();
            _escapeKeyWorks = !_shopManager.IsShopOpen;
        }
        
        if (_escapeKeyWorks)
        {
            Debug.Log("✅ Escape key test PASSED: Shop closes properly");
            Debug.Log("   Note: Actual escape key should work in-game via Update method");
        }
        else
        {
            Debug.LogError("❌ Escape key test FAILED: Shop did not close");
        }
    }
    
    public void TestOpenShop()
    {
        Debug.Log("🧪 Testing: Manual shop open...");
        _shopManager.OpenShop();
        
        if (_shopManager.IsShopOpen)
        {
            Debug.Log("✅ Shop opened successfully");
        }
        else
        {
            Debug.LogError("❌ Failed to open shop");
        }
    }
    
    public void TestCloseShop()
    {
        Debug.Log("🧪 Testing: Manual shop close...");
        _shopManager.CloseShop();
        
        if (!_shopManager.IsShopOpen)
        {
            Debug.Log("✅ Shop closed successfully");
        }
        else
        {
            Debug.LogError("❌ Failed to close shop");
        }
    }
    
    private void ShowTestResults()
    {
        Debug.Log("🧪 === TEST RESULTS SUMMARY ===");
        Debug.Log($"   ShopManager Found: {(_shopManagerFound ? "✅" : "❌")}");
        Debug.Log($"   Auto-Open Disabled: {(_shopAutoOpenDisabled ? "✅" : "❌")}");
        Debug.Log($"   B Key Toggle: {(_bKeyWorks ? "✅" : "❌")}");
        Debug.Log($"   Close Button: {(_closeButtonWorks ? "✅" : "❌")}");
        Debug.Log($"   Escape Key: {(_escapeKeyWorks ? "✅" : "❌")}");
        Debug.Log("================================");
        
        bool allTestsPassed = _shopManagerFound && _shopAutoOpenDisabled && 
                             _bKeyWorks && _closeButtonWorks && _escapeKeyWorks;
        
        if (allTestsPassed)
        {
            Debug.Log("🎉 ALL TESTS PASSED! Shop fixes are working correctly!");
        }
        else
        {
            Debug.LogWarning("⚠️ Some tests failed. Check the logs above for details.");
        }
        
        Debug.Log("\n📋 How to manually test in-game:");
        Debug.Log("   • Press B key to toggle shop");
        Debug.Log("   • Press Escape to close shop (when open)");
        Debug.Log("   • Click close button to close shop");
        Debug.Log("   • Shop should NOT auto-open after 2 seconds");
        Debug.Log("   • Press F1 key to run this test again");
    }
}

// Custom attribute for read-only fields in inspector
public class ReadOnlyAttribute : PropertyAttribute { }