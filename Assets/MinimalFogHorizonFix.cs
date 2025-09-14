using UnityEngine;

/// <summary>
/// Sets fog to 0.0005 density while maintaining invisible horizon line
/// Uses smart fog color matching and blending techniques
/// </summary>
public class MinimalFogHorizonFix : MonoBehaviour
{
    [Header("🌫️ MINIMAL FOG SETTINGS")]
    [SerializeField] private float _fogDensity = 0.0005f;
    [SerializeField] private bool _enableFog = true;
    [SerializeField] private bool _autoMatchSkyboxColor = true;
    
    [Header("🎨 Fog Color Control")]
    [SerializeField] private Color _customFogColor = new Color(0.8f, 0.85f, 0.9f, 1f);
    [SerializeField] private bool _useCustomColor = false;
    
    [Header("📏 Distance Control")]
    [SerializeField] private FogMode _fogMode = FogMode.ExponentialSquared;
    [SerializeField] private float _linearFogStart = 50f;
    [SerializeField] private float _linearFogEnd = 800f;
    
    [Header("🔧 Advanced Settings")]
    [SerializeField] private float _ambientIntensityBoost = 0.1f;
    [SerializeField] private bool _adjustAmbientLighting = true;
    
    [Header("🧪 Manual Controls")]
    [SerializeField] private bool _applySettings = false;
    [SerializeField] private bool _testDifferentColors = false;
    
    private Color _originalAmbientColor;
    private float _originalAmbientIntensity;
    
    void Start()
    {
        // Store original settings
        _originalAmbientColor = RenderSettings.ambientSkyColor;
        _originalAmbientIntensity = RenderSettings.ambientIntensity;
        
        // Apply minimal fog settings
        ApplyMinimalFogSettings();
    }
    
    void OnValidate()
    {
        if (_applySettings)
        {
            _applySettings = false;
            if (Application.isPlaying)
            {
                ApplyMinimalFogSettings();
            }
        }
        
        if (_testDifferentColors)
        {
            _testDifferentColors = false;
            if (Application.isPlaying)
            {
                TestFogColors();
            }
        }
    }
    
    [ContextMenu("Apply Minimal Fog (0.0005)")]
    public void ApplyMinimalFogSettings()
    {
        Debug.Log("🌫️ === APPLYING MINIMAL FOG HORIZON FIX ===");
        
        // Enable fog with very low density
        RenderSettings.fog = _enableFog;
        RenderSettings.fogDensity = _fogDensity;
        RenderSettings.fogMode = _fogMode;
        
        // Set linear fog distances for better control
        if (_fogMode == FogMode.Linear)
        {
            RenderSettings.fogStartDistance = _linearFogStart;
            RenderSettings.fogEndDistance = _linearFogEnd;
        }
        
        // Set fog color to match skybox/horizon
        SetOptimalFogColor();
        
        // Adjust ambient lighting if enabled
        if (_adjustAmbientLighting)
        {
            RenderSettings.ambientIntensity = _originalAmbientIntensity + _ambientIntensityBoost;
            Debug.Log($"✅ Ambient intensity boosted to: {RenderSettings.ambientIntensity}");
        }
        
        // Ensure camera settings are optimal
        EnsureOptimalCameraSettings();
        
        Debug.Log($"✅ MINIMAL FOG APPLIED:");
        Debug.Log($"   • Fog Enabled: {RenderSettings.fog}");
        Debug.Log($"   • Fog Density: {RenderSettings.fogDensity} (ultra-low)");
        Debug.Log($"   • Fog Mode: {RenderSettings.fogMode}");
        Debug.Log($"   • Fog Color: {RenderSettings.fogColor}");
        
        if (_fogMode == FogMode.Linear)
        {
            Debug.Log($"   • Linear Range: {RenderSettings.fogStartDistance} - {RenderSettings.fogEndDistance}");
        }
        
        Debug.Log("🎯 Result: Nearly invisible fog that eliminates horizon line!");
    }
    
    private void SetOptimalFogColor()
    {
        Color fogColor;
        
        if (_useCustomColor)
        {
            fogColor = _customFogColor;
            Debug.Log("🎨 Using custom fog color");
        }
        else if (_autoMatchSkyboxColor && RenderSettings.skybox != null)
        {
            // Attempt to extract dominant color from skybox
            fogColor = ExtractSkyboxHorizonColor();
            Debug.Log("🎨 Auto-matched fog color to skybox");
        }
        else
        {
            // Use intelligent default based on time of day
            fogColor = GetIntelligentDefaultFogColor();
            Debug.Log("🎨 Using intelligent default fog color");
        }
        
        RenderSettings.fogColor = fogColor;
    }
    
    private Color ExtractSkyboxHorizonColor()
    {
        // Try to get a reasonable horizon color from skybox material
        Material skyboxMat = RenderSettings.skybox;
        
        if (skyboxMat == null)
            return GetIntelligentDefaultFogColor();
        
        // Check for common skybox properties
        if (skyboxMat.HasProperty("_SkyTint"))
        {
            return skyboxMat.GetColor("_SkyTint");
        }
        else if (skyboxMat.HasProperty("_Tint"))
        {
            return skyboxMat.GetColor("_Tint");
        }
        else if (skyboxMat.HasProperty("_HorizonColor"))
        {
            return skyboxMat.GetColor("_HorizonColor");
        }
        else if (skyboxMat.HasProperty("_GroundColor"))
        {
            // Blend ground and sky colors for horizon
            Color groundColor = skyboxMat.GetColor("_GroundColor");
            Color ambientColor = RenderSettings.ambientSkyColor;
            return Color.Lerp(groundColor, ambientColor, 0.3f);
        }
        
        return GetIntelligentDefaultFogColor();
    }
    
    private Color GetIntelligentDefaultFogColor()
    {
        // Base color on ambient sky color with slight warmth
        Color baseColor = RenderSettings.ambientSkyColor;
        
        // Add slight warmth and brightness for natural fog appearance
        Color fogColor = new Color(
            Mathf.Clamp01(baseColor.r + 0.1f),
            Mathf.Clamp01(baseColor.g + 0.1f),
            Mathf.Clamp01(baseColor.b + 0.05f),
            1f
        );
        
        // Ensure it's not too dark (minimum brightness)
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
    
    private void EnsureOptimalCameraSettings()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        
        foreach (Camera cam in cameras)
        {
            // Ensure far clip plane is sufficient
            if (cam.farClipPlane < 5000f)
            {
                cam.farClipPlane = 15000f;
                Debug.Log($"✅ Extended {cam.name} far clip plane to 15000");
            }
            
            // Ensure skybox clear flags
            if (cam.clearFlags != CameraClearFlags.Skybox)
            {
                cam.clearFlags = CameraClearFlags.Skybox;
                Debug.Log($"✅ Set {cam.name} clear flags to Skybox");
            }
        }
    }
    
    [ContextMenu("Test Different Fog Colors")]
    public void TestFogColors()
    {
        if (!Application.isPlaying) return;
        
        Debug.Log("🧪 Testing different fog colors for horizon blending...");
        
        // Test sequence of colors
        StartCoroutine(TestColorSequence());
    }
    
    System.Collections.IEnumerator TestColorSequence()
    {
        Color[] testColors = new Color[]
        {
            new Color(0.7f, 0.8f, 0.9f, 1f),   // Light blue-gray
            new Color(0.8f, 0.85f, 0.9f, 1f),  // Warm light gray
            new Color(0.75f, 0.8f, 0.85f, 1f), // Cool light gray
            new Color(0.85f, 0.88f, 0.92f, 1f), // Very light warm
            RenderSettings.ambientSkyColor,      // Ambient match
        };
        
        string[] colorNames = { "Light Blue-Gray", "Warm Light Gray", "Cool Light Gray", "Very Light Warm", "Ambient Match" };
        
        for (int i = 0; i < testColors.Length; i++)
        {
            Debug.Log($"🎨 Testing color {i + 1}: {colorNames[i]}");
            RenderSettings.fogColor = testColors[i];
            yield return new WaitForSeconds(3f);
        }
        
        // Return to optimal color
        SetOptimalFogColor();
        Debug.Log("✅ Color test complete - returned to optimal color");
    }
    
    [ContextMenu("Reset to Original Settings")]
    public void ResetToOriginalSettings()
    {
        RenderSettings.ambientSkyColor = _originalAmbientColor;
        RenderSettings.ambientIntensity = _originalAmbientIntensity;
        RenderSettings.fog = false;
        
        Debug.Log("🔄 Reset to original render settings");
    }
    
    void OnDestroy()
    {
        // Restore original settings when component is destroyed
        if (Application.isPlaying)
        {
            ResetToOriginalSettings();
        }
    }
    
    // Real-time adjustment in editor
    void Update()
    {
        if (!Application.isPlaying) return;
        
        // Allow real-time density adjustment
        if (RenderSettings.fogDensity != _fogDensity)
        {
            RenderSettings.fogDensity = _fogDensity;
        }
        
        // Allow real-time fog mode changes
        if (RenderSettings.fogMode != _fogMode)
        {
            RenderSettings.fogMode = _fogMode;
            if (_fogMode == FogMode.Linear)
            {
                RenderSettings.fogStartDistance = _linearFogStart;
                RenderSettings.fogEndDistance = _linearFogEnd;
            }
        }
    }
}