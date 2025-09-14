using UnityEngine;

/// <summary>
/// Provides balanced storm settings that can be easily applied to fix storm speed issues
/// </summary>
[CreateAssetMenu(fileName = "BalancedStormSettings", menuName = "TPSBR/Balanced Storm Settings")]
public class BalancedStormSettings : ScriptableObject
{
    [Header("Timing Settings")]
    [Tooltip("Delay before the first storm starts (default: 30s -> improved: 60s)")]
    public float shrinkStartDelay = 60f;
    
    [Tooltip("Minimum delay between storm phases (default: 35s -> improved: 45s)")]
    public float minShrinkDelay = 45f;
    
    [Tooltip("Maximum delay between storm phases (default: 90s -> improved: 120s)")]
    public float maxShrinkDelay = 120f;
    
    [Tooltip("How long each storm shrinking takes (default: 20s -> improved: 30s)")]
    public float shrinkDuration = 30f;
    
    [Tooltip("Warning time before storm shrinks (default: 30s -> improved: 45s)")]
    public float shrinkAnnounceDuration = 45f;
    
    [Tooltip("Number of storm phases (default: 5 -> improved: 6)")]
    public int shrinkSteps = 6;
    
    [Header("Size Settings")]
    [Tooltip("Starting radius of safe zone (default: 100f -> improved: 120f)")]
    public float startRadius = 120f;
    
    [Tooltip("Final radius of safe zone (default: 40f -> improved: 30f)")]
    public float endRadius = 30f;
    
    [Header("Damage Settings")]
    [Tooltip("Damage per tick outside safe zone (default: 5f -> improved: 3f)")]
    public float damagePerTick = 3f;
    
    [Tooltip("Time between damage ticks (default: 1f -> improved: 1.5f)")]
    public float damageTickTime = 1.5f;
    
    [Header("Player Count Scaling")]
    [Tooltip("Minimum players for min delay (default: 2)")]
    public int minShrinkDelayPlayers = 2;
    
    [Tooltip("Maximum players for max delay (default: 60)")]
    public int maxShrinkDelayPlayers = 60;
    
    [ContextMenu("Apply to Scene")]
    public void ApplyToScene()
    {
        var stormFixer = FindObjectOfType<StormSpeedFix>();
        if (stormFixer == null)
        {
            GameObject fixerGO = new GameObject("Storm Speed Fixer");
            stormFixer = fixerGO.AddComponent<StormSpeedFix>();
        }
        
        // Copy values to the fixer
        var fixerType = typeof(StormSpeedFix);
        var fields = fixerType.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        foreach (var field in fields)
        {
            if (field.Name.Contains("_shrinkStartDelay"))
                field.SetValue(stormFixer, shrinkStartDelay);
            else if (field.Name.Contains("_minShrinkDelay"))
                field.SetValue(stormFixer, minShrinkDelay);
            else if (field.Name.Contains("_maxShrinkDelay"))
                field.SetValue(stormFixer, maxShrinkDelay);
            else if (field.Name.Contains("_shrinkDuration"))
                field.SetValue(stormFixer, shrinkDuration);
            else if (field.Name.Contains("_shrinkAnnounceDuration"))
                field.SetValue(stormFixer, shrinkAnnounceDuration);
            else if (field.Name.Contains("_shrinkSteps"))
                field.SetValue(stormFixer, shrinkSteps);
            else if (field.Name.Contains("_startRadius"))
                field.SetValue(stormFixer, startRadius);
            else if (field.Name.Contains("_endRadius"))
                field.SetValue(stormFixer, endRadius);
            else if (field.Name.Contains("_damagePerTick"))
                field.SetValue(stormFixer, damagePerTick);
            else if (field.Name.Contains("_damageTickTime"))
                field.SetValue(stormFixer, damageTickTime);
        }
        
        stormFixer.ApplyStormFix();
        Debug.Log("✅ Balanced storm settings applied to scene!");
    }
}