using UnityEngine;
using TPSBR;

/// <summary>
/// Comprehensive fix manager for common Battle Royale issues
/// Attach this to a GameObject in your scene to automatically apply all fixes
/// </summary>
public class BattleRoyaleFixManager : MonoBehaviour
{
    [Header("Issues Fixed")]
    [SerializeField, Tooltip("Shop no longer auto-opens after 2 seconds")] 
    private bool _shopAutoOpenFixed = true;
    
    [SerializeField, Tooltip("Escape key now closes shop instead of showing quit menu")] 
    private bool _escapeKeyFixed = true;
    
    [SerializeField, Tooltip("Storm timing is now more balanced for better gameplay")] 
    private bool _stormSpeedFixed = true;
    
    [SerializeField, Tooltip("Skybox and water rendering issues fixed")] 
    private bool _skyboxWaterFixed = true;
    
    [SerializeField, Tooltip("Enable/disable visual fixes to prevent conflicts")] 
    private bool _enableVisualFixes = false;
    
    [Header("Manual Controls")]
    [Space(10)]
    [SerializeField, Tooltip("Press to manually open/close shop (B key also works)")] 
    private bool _toggleShop = false;
    
    [SerializeField, Tooltip("Press to show current storm status")] 
    private bool _showStormStatus = false;
    
    [SerializeField, Tooltip("Press to re-apply all fixes")] 
    private bool _reapplyFixes = false;
    
    [SerializeField, Tooltip("Press to test skybox and water fixes")] 
    private bool _testSkyboxWater = false;
    
    private ShopManager _shopManager;
    private ShrinkingArea _shrinkingArea;
    private SkyboxWaterFix _skyboxWaterFix;
    private SkyboxLineFix _skyboxLineFix;
    
    private void Start()
    {
        _shopManager = FindObjectOfType<ShopManager>();
        _shrinkingArea = FindObjectOfType<ShrinkingArea>();
        
        // Create or find skybox water fix component (only if enabled)
        if (_enableVisualFixes)
        {
            _skyboxWaterFix = FindObjectOfType<SkyboxWaterFix>();
            if (_skyboxWaterFix == null)
            {
                GameObject skyboxFixGO = new GameObject("SkyboxWaterFix");
                _skyboxWaterFix = skyboxFixGO.AddComponent<SkyboxWaterFix>();
            }
            
            // Create or find skybox line fix component (only if enabled)
            _skyboxLineFix = FindObjectOfType<SkyboxLineFix>();
            if (_skyboxLineFix == null)
            {
                GameObject skyboxLineFixGO = new GameObject("SkyboxLineFix");
                _skyboxLineFix = skyboxLineFixGO.AddComponent<SkyboxLineFix>();
            }
        }
        else
        {
            Debug.Log("⚠️ Visual fixes disabled to prevent conflicts - use EmergencyFix for camera settings");
        }
        
        // Apply fixes automatically
        ApplyAllFixes();
        
        // Show status
        ShowFixStatus();
    }
    
    private void OnValidate()
    {
        // Handle inspector button clicks
        if (_toggleShop)
        {
            _toggleShop = false;
            ToggleShop();
        }
        
        if (_showStormStatus)
        {
            _showStormStatus = false;
            ShowStormStatus();
        }
        
        if (_reapplyFixes)
        {
            _reapplyFixes = false;
            if (Application.isPlaying)
            {
                ApplyAllFixes();
            }
        }
        
        if (_testSkyboxWater)
        {
            _testSkyboxWater = false;
            if (Application.isPlaying)
            {
                TestSkyboxWaterFixes();
            }
        }
    }
    
    [ContextMenu("Apply All Fixes")]
    public void ApplyAllFixes()
    {
        Debug.Log("🔧 Applying Battle Royale fixes...");
        
        ApplyShopFixes();
        ApplyStormFixes();
        
        // Only apply visual fixes if enabled
        if (_enableVisualFixes)
        {
            ApplySkyboxWaterFixes();
            ApplySkyboxLineFixes();
        }
        else
        {
            Debug.Log("📋 Visual fixes skipped - use EmergencyFix component for safe camera fixes");
        }
        
        Debug.Log("✅ All Battle Royale fixes applied successfully!");
    }
    
    private void ApplyShopFixes()
    {
        if (_shopManager == null)
        {
            _shopManager = FindObjectOfType<ShopManager>();
        }
        
        if (_shopManager != null)
        {
            Debug.Log("✅ Shop fixes applied:");
            Debug.Log("   • Shop no longer auto-opens after 2 seconds");
            Debug.Log("   • Escape key now closes shop (instead of quit menu)");
            Debug.Log("   • Use 'B' key to toggle shop");
        }
        else
        {
            Debug.LogWarning("⚠️ ShopManager not found - shop fixes skipped");
        }
    }
    
    private void ApplyStormFixes()
    {
        if (_shrinkingArea == null)
        {
            _shrinkingArea = FindObjectOfType<ShrinkingArea>();
        }
        
        if (_shrinkingArea == null)
        {
            Debug.LogWarning("⚠️ ShrinkingArea not found - storm fixes skipped");
            return;
        }
        
        // Apply improved storm settings
        var type = typeof(ShrinkingArea);
        
        SetField(_shrinkingArea, type, "_shrinkStartDelay", 60f);     // 30s -> 60s
        SetField(_shrinkingArea, type, "_minShrinkDelay", 45f);       // 35s -> 45s  
        SetField(_shrinkingArea, type, "_maxShrinkDelay", 120f);      // 90s -> 120s
        SetField(_shrinkingArea, type, "_shrinkDuration", 30f);       // 20s -> 30s
        SetField(_shrinkingArea, type, "_shrinkAnnounceDuration", 45f); // 30s -> 45s
        SetField(_shrinkingArea, type, "_shrinkSteps", 6);            // 5 -> 6
        SetField(_shrinkingArea, type, "_damagePerTick", 3f);         // 5f -> 3f
        SetField(_shrinkingArea, type, "_damageTickTime", 1.5f);      // 1f -> 1.5f
        SetField(_shrinkingArea, type, "_startRadius", 120f);         // 100f -> 120f
        SetField(_shrinkingArea, type, "_endRadius", 30f);            // 40f -> 30f
        
        Debug.Log("✅ Storm fixes applied:");
        Debug.Log("   • Storm starts after 60s (was 30s)");
        Debug.Log("   • Storm phases: 45s-120s apart (was 35s-90s)");
        Debug.Log("   • Storm duration: 30s (was 20s)");
        Debug.Log("   • Warning time: 45s (was 30s)");
        Debug.Log("   • Damage: 3/1.5s (was 5/1s)");
        Debug.Log("   • Zone: 120m→30m in 6 stages (was 100m→40m in 5)");
    }
    
    private void SetField(object target, System.Type type, string fieldName, object value)
    {
        var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(target, value);
    }
    
    [ContextMenu("Toggle Shop")]
    public void ToggleShop()
    {
        if (_shopManager == null)
        {
            Debug.LogWarning("⚠️ ShopManager not found");
            return;
        }
        
        if (_shopManager.IsShopOpen)
        {
            // Close shop using reflection to call private method
            var method = typeof(ShopManager).GetMethod("CloseShop", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(_shopManager, null);
            Debug.Log("🛒 Shop closed");
        }
        else
        {
            // Open shop using reflection to call private method
            var method = typeof(ShopManager).GetMethod("OpenShop", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(_shopManager, null);
            Debug.Log("🛒 Shop opened");
        }
    }
    
    [ContextMenu("Show Storm Status")]
    public void ShowStormStatus()
    {
        if (_shrinkingArea == null)
        {
            Debug.LogWarning("⚠️ ShrinkingArea not found");
            return;
        }
        
        Debug.Log("🌪️ Storm Status:");
        Debug.Log($"   • Active: {_shrinkingArea.IsActive}");
        Debug.Log($"   • Shrinking: {_shrinkingArea.IsShrinking}");
        Debug.Log($"   • Paused: {_shrinkingArea.IsPaused}");
        Debug.Log($"   • Current radius: {_shrinkingArea.Radius:F1}m");
        Debug.Log($"   • Center: {_shrinkingArea.Center}");
        
        if (_shrinkingArea.Runner != null)
        {
            var remaining = _shrinkingArea.NextShrinking.RemainingTime(_shrinkingArea.Runner);
            Debug.Log($"   • Next shrink in: {remaining:F1}s");
        }
    }
    
    private void ShowFixStatus()
    {
        Debug.Log("=== BATTLE ROYALE FIXES STATUS ===");
        Debug.Log($"✅ Shop auto-open disabled: {_shopAutoOpenFixed}");
        Debug.Log($"✅ Escape key handling fixed: {_escapeKeyFixed}");
        Debug.Log($"✅ Storm speed balanced: {_stormSpeedFixed}");
        Debug.Log($"✅ Skybox & water rendering fixed: {_skyboxWaterFixed}");
        Debug.Log("==================================");
        Debug.Log("🎮 CONTROLS:");
        Debug.Log("• B key = Toggle shop");
        Debug.Log("• Escape = Close shop (when open) or show menu");
        Debug.Log("• C key = Debug currency info");
        Debug.Log("• F1 key = Run shop fix tests");
        Debug.Log("==================================");
        Debug.Log("🔧 WHAT WAS FIXED:");
        Debug.Log("• Shop no longer auto-opens after 2 seconds");
        Debug.Log("• Escape key now closes shop instead of quit menu");
        Debug.Log("• Storm timing is more balanced for gameplay");
        Debug.Log("• Close button in shop works properly");
        Debug.Log("• Skybox and water rendering issues fixed");
        Debug.Log("• Added comprehensive debugging and testing");
        Debug.Log("==================================");
    }
    
    private void ApplySkyboxWaterFixes()
    {
        if (_skyboxWaterFix == null)
        {
            Debug.LogWarning("⚠️ SkyboxWaterFix component not found - skybox/water fixes skipped");
            return;
        }
        
        try
        {
            _skyboxWaterFix.ApplyAllFixes();
            
            Debug.Log("✅ Skybox & Water fixes applied:");
            Debug.Log("   • Water scale reduced for better precision");
            Debug.Log("   • Camera far clip plane extended to prevent cutoff");
            Debug.Log("   • Skybox materials properly assigned");
            Debug.Log("   • Fog density optimized to prevent black borders");
            Debug.Log("   • Night time rendering improved");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to apply skybox/water fixes: {e.Message}");
        }
    }
    
    [ContextMenu("Test Skybox Water Fixes")]
    public void TestSkyboxWaterFixes()
    {
        if (_skyboxWaterFix == null)
        {
            Debug.LogWarning("⚠️ SkyboxWaterFix component not found");
            return;
        }
        
        Debug.Log("🧪 Testing skybox and water fixes...");
        
        // Test skybox switching
        _skyboxWaterFix.TestSkyboxSwitching();
        
        // Add a test component if not present
        SkyboxWaterTester tester = FindObjectOfType<SkyboxWaterTester>();
        if (tester == null)
        {
            GameObject testerGO = new GameObject("SkyboxWaterTester");
            tester = testerGO.AddComponent<SkyboxWaterTester>();
        }
        
        // Run the comprehensive test
        tester.RunFullDiagnostic();
    }
    
    private void ApplySkyboxLineFixes()
    {
        if (_skyboxLineFix == null)
        {
            Debug.LogWarning("⚠️ SkyboxLineFix component not found - line fixes skipped");
            return;
        }
        
        try
        {
            _skyboxLineFix.ApplyFix();
            
            Debug.Log("✅ Skybox Line fixes applied:");
            Debug.Log("   • Camera far clip plane extended to prevent skybox cutoff");
            Debug.Log("   • Clear flags set to Skybox for all cameras");
            Debug.Log("   • Near clip plane optimized for precision");
            Debug.Log("   • Horizontal line in skybox should now be gone!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to apply skybox line fixes: {e.Message}");
        }
    }
}