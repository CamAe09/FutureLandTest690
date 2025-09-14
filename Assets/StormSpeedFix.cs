using UnityEngine;
using TPSBR;

/// <summary>
/// Fixes storm speed settings to be more balanced for gameplay
/// </summary>
public class StormSpeedFix : MonoBehaviour
{
    [Header("Improved Storm Settings")]
    [SerializeField] private float _shrinkStartDelay = 60f;      // Wait 60s before first storm (was 30s)
    [SerializeField] private float _minShrinkDelay = 45f;        // Min delay between storms (was 35s)
    [SerializeField] private float _maxShrinkDelay = 120f;       // Max delay between storms (was 90s)
    [SerializeField] private float _shrinkDuration = 30f;        // Storm shrink duration (was 20s)
    [SerializeField] private float _shrinkAnnounceDuration = 45f; // Warning time (was 30s)
    [SerializeField] private int _shrinkSteps = 6;               // More stages (was 5)
    [SerializeField] private float _damagePerTick = 3f;          // Reduced damage (was 5f)
    [SerializeField] private float _damageTickTime = 1.5f;       // Slower damage ticks (was 1f)
    
    [Header("Size Settings")]
    [SerializeField] private float _startRadius = 120f;         // Larger starting area (was 100f)
    [SerializeField] private float _endRadius = 30f;            // Smaller final area (was 40f)
    
    private void Start()
    {
        ApplyStormFix();
    }
    
    [ContextMenu("Apply Storm Speed Fix")]
    public void ApplyStormFix()
    {
        // Find the shrinking area component
        ShrinkingArea shrinkingArea = FindObjectOfType<ShrinkingArea>();
        
        if (shrinkingArea == null)
        {
            Debug.LogWarning("⚠️ No ShrinkingArea found in scene. Storm speed fix not applied.");
            return;
        }
        
        // Use reflection to modify private fields since they're serialized
        var shrinkingAreaType = typeof(ShrinkingArea);
        
        // Get private fields using reflection
        var shrinkStartDelayField = shrinkingAreaType.GetField("_shrinkStartDelay", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var minShrinkDelayField = shrinkingAreaType.GetField("_minShrinkDelay", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var maxShrinkDelayField = shrinkingAreaType.GetField("_maxShrinkDelay", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var shrinkDurationField = shrinkingAreaType.GetField("_shrinkDuration", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var shrinkAnnounceDurationField = shrinkingAreaType.GetField("_shrinkAnnounceDuration", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var shrinkStepsField = shrinkingAreaType.GetField("_shrinkSteps", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var damagePerTickField = shrinkingAreaType.GetField("_damagePerTick", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var damageTickTimeField = shrinkingAreaType.GetField("_damageTickTime", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var startRadiusField = shrinkingAreaType.GetField("_startRadius", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var endRadiusField = shrinkingAreaType.GetField("_endRadius", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        // Apply the improved settings
        if (shrinkStartDelayField != null)
            shrinkStartDelayField.SetValue(shrinkingArea, _shrinkStartDelay);
        if (minShrinkDelayField != null)
            minShrinkDelayField.SetValue(shrinkingArea, _minShrinkDelay);
        if (maxShrinkDelayField != null)
            maxShrinkDelayField.SetValue(shrinkingArea, _maxShrinkDelay);
        if (shrinkDurationField != null)
            shrinkDurationField.SetValue(shrinkingArea, _shrinkDuration);
        if (shrinkAnnounceDurationField != null)
            shrinkAnnounceDurationField.SetValue(shrinkingArea, _shrinkAnnounceDuration);
        if (shrinkStepsField != null)
            shrinkStepsField.SetValue(shrinkingArea, _shrinkSteps);
        if (damagePerTickField != null)
            damagePerTickField.SetValue(shrinkingArea, _damagePerTick);
        if (damageTickTimeField != null)
            damageTickTimeField.SetValue(shrinkingArea, _damageTickTime);
        if (startRadiusField != null)
            startRadiusField.SetValue(shrinkingArea, _startRadius);
        if (endRadiusField != null)
            endRadiusField.SetValue(shrinkingArea, _endRadius);
        
        Debug.Log("✅ Storm speed fix applied successfully!");
        Debug.Log($"   Storm starts in: {_shrinkStartDelay}s");
        Debug.Log($"   Storm delay range: {_minShrinkDelay}s - {_maxShrinkDelay}s");
        Debug.Log($"   Storm duration: {_shrinkDuration}s");
        Debug.Log($"   Warning time: {_shrinkAnnounceDuration}s");
        Debug.Log($"   Damage: {_damagePerTick} every {_damageTickTime}s");
        Debug.Log($"   Area: {_startRadius}m → {_endRadius}m in {_shrinkSteps} stages");
    }
    
    [ContextMenu("Show Current Storm Settings")]
    public void ShowCurrentStormSettings()
    {
        ShrinkingArea shrinkingArea = FindObjectOfType<ShrinkingArea>();
        if (shrinkingArea == null)
        {
            Debug.LogWarning("⚠️ No ShrinkingArea found in scene.");
            return;
        }
        
        Debug.Log("🌪️ Current Storm Settings:");
        Debug.Log($"   Center: {shrinkingArea.Center}");
        Debug.Log($"   Current Radius: {shrinkingArea.Radius}");
        Debug.Log($"   Is Active: {shrinkingArea.IsActive}");
        Debug.Log($"   Is Shrinking: {shrinkingArea.IsShrinking}");
        Debug.Log($"   Is Paused: {shrinkingArea.IsPaused}");
        Debug.Log($"   Next shrinking: {shrinkingArea.NextShrinking.RemainingTime(shrinkingArea.Runner)}s");
    }
}