using UnityEngine;

/// <summary>
/// Emergency fix to resolve 999+ errors and networking issues
/// This script only fixes essential camera settings without any input system usage
/// </summary>
public class EmergencyFix : MonoBehaviour
{
    [Header("🚨 EMERGENCY FIX")]
    [SerializeField] private bool applyEmergencyFix = false;
    
    [Header("🌫️ MINIMAL FOG (0.0005)")]
    [SerializeField] private bool includeMinimalFog = true;
    [SerializeField] private float fogDensity = 0.0005f;
    
    void OnValidate()
    {
        if (applyEmergencyFix)
        {
            applyEmergencyFix = false;
            if (Application.isPlaying)
            {
                ApplyEmergencyFix();
            }
        }
    }
    
    void Start()
    {
        // Only apply camera fixes, nothing else that could interfere
        ApplyEmergencyFix();
    }
    
    [ContextMenu("Apply Emergency Fix")]
    public void ApplyEmergencyFix()
    {
        Debug.Log("🚨 APPLYING EMERGENCY FIX...");
        
        // Fix only camera settings, nothing that could break networking
        Camera[] cameras = FindObjectsOfType<Camera>();
        
        foreach (Camera cam in cameras)
        {
            // Fix the skybox line issue by extending far clip plane
            if (cam.farClipPlane < 5000f)
            {
                cam.farClipPlane = 15000f;
                Debug.Log($"✅ Fixed {cam.name}: Far clip plane → 15000");
            }
            
            // Ensure skybox clear flags
            if (cam.clearFlags != CameraClearFlags.Skybox)
            {
                cam.clearFlags = CameraClearFlags.Skybox;
                Debug.Log($"✅ Fixed {cam.name}: Clear flags → Skybox");
            }
        }
        
        // Apply minimal fog settings if requested
        if (includeMinimalFog)
        {
            ApplyMinimalFog();
        }
        
        Debug.Log("✅ EMERGENCY FIX COMPLETE - Camera settings fixed");
        Debug.Log("📋 The skybox line should now be gone without breaking networking");
    }
    
    private void ApplyMinimalFog()
    {
        // Enable very light fog to eliminate horizon line
        RenderSettings.fog = true;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        
        // Set fog color to match ambient/skybox
        Color fogColor = GetIntelligentFogColor();
        RenderSettings.fogColor = fogColor;
        
        Debug.Log($"✅ Minimal fog applied: Density {fogDensity}, Color {fogColor}");
    }
    
    private Color GetIntelligentFogColor()
    {
        // Base on ambient sky color with slight brightness boost
        Color baseColor = RenderSettings.ambientSkyColor;
        
        Color fogColor = new Color(
            Mathf.Clamp01(baseColor.r + 0.1f),
            Mathf.Clamp01(baseColor.g + 0.1f),
            Mathf.Clamp01(baseColor.b + 0.05f),
            1f
        );
        
        // Ensure minimum brightness for natural appearance
        float brightness = (fogColor.r + fogColor.g + fogColor.b) / 3f;
        if (brightness < 0.4f)
        {
            float boost = 0.4f - brightness;
            fogColor.r += boost;
            fogColor.g += boost;
            fogColor.b += boost;
        }
        
        return fogColor;
    }
}