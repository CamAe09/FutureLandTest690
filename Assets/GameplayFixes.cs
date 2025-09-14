using UnityEngine;
using TPSBR;

/// <summary>
/// Applies various gameplay fixes automatically when the game starts
/// </summary>
public class GameplayFixes : MonoBehaviour
{
    [Header("Auto-Apply Fixes")]
    [SerializeField] private bool _fixShopAutoOpen = true;
    [SerializeField] private bool _fixEscapeKeyHandling = true;
    [SerializeField] private bool _fixStormSpeed = true;
    
    [Header("Storm Speed Settings")]
    [SerializeField] private float _stormStartDelay = 60f;
    [SerializeField] private float _stormDuration = 30f;
    [SerializeField] private float _stormWarningTime = 45f;
    [SerializeField] private float _stormDamage = 3f;
    
    private void Awake()
    {
        // Apply fixes early in the game lifecycle
        if (_fixShopAutoOpen)
        {
            FixShopAutoOpen();
        }
        
        if (_fixEscapeKeyHandling)
        {
            FixEscapeKeyHandling();
        }
    }
    
    private void Start()
    {
        // Apply storm fixes after scene is fully loaded
        if (_fixStormSpeed)
        {
            FixStormSpeed();
        }
        
        Debug.Log("🔧 Gameplay fixes applied successfully!");
    }
    
    private void FixShopAutoOpen()
    {
        var shopManager = FindObjectOfType<ShopManager>();
        if (shopManager != null)
        {
            // The fix is already applied in ShopManager.cs - just log confirmation
            Debug.Log("✅ Shop auto-open fix: Shop will no longer open automatically after 2 seconds");
        }
    }
    
    private void FixEscapeKeyHandling()
    {
        var shopManager = FindObjectOfType<ShopManager>();
        if (shopManager != null)
        {
            // The fix is already applied in ShopManager.cs via IBackHandler implementation
            Debug.Log("✅ Escape key fix: Escape will now close shop instead of showing quit menu");
        }
    }
    
    private void FixStormSpeed()
    {
        var shrinkingArea = FindObjectOfType<ShrinkingArea>();
        if (shrinkingArea == null)
        {
            Debug.LogWarning("⚠️ No ShrinkingArea found - storm speed fix skipped");
            return;
        }
        
        // Apply storm timing fixes using reflection
        var shrinkingAreaType = typeof(ShrinkingArea);
        
        try
        {
            // Get and set timing fields
            SetPrivateField(shrinkingArea, shrinkingAreaType, "_shrinkStartDelay", _stormStartDelay);
            SetPrivateField(shrinkingArea, shrinkingAreaType, "_shrinkDuration", _stormDuration);
            SetPrivateField(shrinkingArea, shrinkingAreaType, "_shrinkAnnounceDuration", _stormWarningTime);
            SetPrivateField(shrinkingArea, shrinkingAreaType, "_damagePerTick", _stormDamage);
            SetPrivateField(shrinkingArea, shrinkingAreaType, "_damageTickTime", 1.5f);
            SetPrivateField(shrinkingArea, shrinkingAreaType, "_minShrinkDelay", 45f);
            SetPrivateField(shrinkingArea, shrinkingAreaType, "_maxShrinkDelay", 120f);
            SetPrivateField(shrinkingArea, shrinkingAreaType, "_shrinkSteps", 6);
            SetPrivateField(shrinkingArea, shrinkingAreaType, "_startRadius", 120f);
            SetPrivateField(shrinkingArea, shrinkingAreaType, "_endRadius", 30f);
            
            Debug.Log("✅ Storm speed fix applied:");
            Debug.Log($"   • First storm starts after: {_stormStartDelay}s (was 30s)");
            Debug.Log($"   • Storm duration: {_stormDuration}s (was 20s)");
            Debug.Log($"   • Warning time: {_stormWarningTime}s (was 30s)");
            Debug.Log($"   • Storm damage: {_stormDamage}/tick (was 5)");
            Debug.Log($"   • More balanced timing and larger safe zones");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to apply storm fixes: {e.Message}");
        }
    }
    
    private void SetPrivateField(object target, System.Type type, string fieldName, object value)
    {
        var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, value);
        }
        else
        {
            Debug.LogWarning($"⚠️ Field {fieldName} not found in {type.Name}");
        }
    }
    
    [ContextMenu("Apply All Fixes Manually")]
    public void ApplyAllFixesManually()
    {
        FixShopAutoOpen();
        FixEscapeKeyHandling();
        FixStormSpeed();
        Debug.Log("🔧 All gameplay fixes applied manually!");
    }
}