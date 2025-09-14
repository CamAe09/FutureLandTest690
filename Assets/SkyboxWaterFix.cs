using UnityEngine;

/// <summary>
/// Fixes skybox and water rendering issues in battle royale games
/// Addresses issues like water cutting in/out and black borders between sky and water
/// </summary>
public class SkyboxWaterFix : MonoBehaviour
{
    [Header("Debug & Testing")]
    [SerializeField] private bool _debugMode = true;
    [SerializeField] private bool _autoFixOnStart = true;
    
    [Header("Water Settings")]
    [SerializeField] private GameObject _waterObject;
    [SerializeField] private float _waterHeightOffset = 0.5f;
    [SerializeField] private Vector3 _improvedWaterScale = new Vector3(5000f, 1f, 5000f);
    
    [Header("Camera Settings")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _improvedFarClipPlane = 15000f;
    [SerializeField] private float _improvedNearClipPlane = 0.1f;
    
    [Header("Skybox Materials")]
    [SerializeField] private Material _defaultDaySkybox;
    [SerializeField] private Material _defaultNightSkybox;
    [SerializeField] private Material _fallbackSkybox;
    
    [Header("Inspector Buttons")]
    [SerializeField] private bool _applyWaterFix = false;
    [SerializeField] private bool _applyCameraFix = false;
    [SerializeField] private bool _applySkyboxFix = false;
    [SerializeField] private bool _testAllFixes = false;
    
    private void Start()
    {
        if (_autoFixOnStart)
        {
            ApplyAllFixes();
        }
    }
    
    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        
        if (_applyWaterFix)
        {
            _applyWaterFix = false;
            FixWaterIssues();
        }
        
        if (_applyCameraFix)
        {
            _applyCameraFix = false;
            FixCameraSettings();
        }
        
        if (_applySkyboxFix)
        {
            _applySkyboxFix = false;
            FixSkyboxSettings();
        }
        
        if (_testAllFixes)
        {
            _testAllFixes = false;
            ApplyAllFixes();
        }
    }
    
    [ContextMenu("Apply All Fixes")]
    public void ApplyAllFixes()
    {
        Debug.Log("🌊 === APPLYING SKYBOX & WATER FIXES ===");
        
        FindRequiredObjects();
        FixCameraSettings();
        FixWaterIssues();
        FixSkyboxSettings();
        FixTimeOfDayController();
        
        Debug.Log("✅ All skybox and water fixes applied!");
    }
    
    private void FindRequiredObjects()
    {
        // Find water object if not assigned
        if (_waterObject == null)
        {
            GameObject water = GameObject.FindGameObjectWithTag("Water");
            if (water == null)
            {
                water = GameObject.Find("Water");
            }
            
            if (water != null)
            {
                _waterObject = water;
                Debug.Log($"🔍 Found water object: {water.name}");
            }
            else
            {
                Debug.LogWarning("⚠️ No water object found!");
            }
        }
        
        // Find main camera if not assigned
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                _mainCamera = FindObjectOfType<Camera>();
            }
            
            if (_mainCamera != null)
            {
                Debug.Log($"🔍 Found camera: {_mainCamera.name}");
            }
        }
        
        // Find skybox materials if not assigned
        if (_defaultDaySkybox == null)
        {
            _defaultDaySkybox = Resources.Load<Material>("TPSBR/Materials/Skybox");
            if (_defaultDaySkybox == null)
            {
                // Try to find any skybox material
                Material[] allMaterials = Resources.FindObjectsOfTypeAll<Material>();
                foreach (Material mat in allMaterials)
                {
                    if (mat.shader != null && mat.shader.name.Contains("Skybox"))
                    {
                        _defaultDaySkybox = mat;
                        Debug.Log($"🔍 Found skybox material: {mat.name}");
                        break;
                    }
                }
            }
        }
    }
    
    public void FixWaterIssues()
    {
        Debug.Log("🌊 Fixing water rendering issues...");
        
        if (_waterObject == null)
        {
            Debug.LogWarning("⚠️ No water object found to fix!");
            return;
        }
        
        // Fix water transform issues
        Transform waterTransform = _waterObject.transform;
        
        // Reduce the extreme scale that can cause precision issues
        waterTransform.localScale = _improvedWaterScale;
        
        // Ensure water is at a reasonable height
        Vector3 currentPos = waterTransform.position;
        waterTransform.position = new Vector3(currentPos.x, _waterHeightOffset, currentPos.z);
        
        // Fix water material settings for better distance rendering
        Renderer waterRenderer = _waterObject.GetComponent<Renderer>();
        if (waterRenderer != null)
        {
            Material waterMaterial = waterRenderer.material;
            if (waterMaterial != null)
            {
                // Disable shadow casting for water (can cause cutting issues)
                waterRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                waterRenderer.receiveShadows = true;
                
                // Ensure proper render queue for transparency
                if (waterMaterial.renderQueue < 3000)
                {
                    waterMaterial.renderQueue = 3000; // Transparent queue
                }
                
                Debug.Log("✅ Water material settings optimized");
            }
        }
        
        // Ensure water collider matches new scale
        Collider waterCollider = _waterObject.GetComponent<Collider>();
        if (waterCollider != null)
        {
            // For box colliders, make sure they cover the needed area
            if (waterCollider is BoxCollider boxCollider)
            {
                boxCollider.size = Vector3.one; // Since we're using scale for size
            }
        }
        
        Debug.Log($"✅ Water fixes applied - Scale: {_improvedWaterScale}, Position Y: {_waterHeightOffset}");
    }
    
    public void FixCameraSettings()
    {
        Debug.Log("📷 Fixing camera settings for skybox and water...");
        
        if (_mainCamera == null)
        {
            Debug.LogWarning("⚠️ No main camera found to fix!");
            return;
        }
        
        // Fix camera clipping planes to prevent skybox/water cutoff
        _mainCamera.farClipPlane = _improvedFarClipPlane;
        _mainCamera.nearClipPlane = _improvedNearClipPlane;
        
        // Ensure camera clear flags are set for skybox
        _mainCamera.clearFlags = CameraClearFlags.Skybox;
        
        // Set camera background color as fallback (useful if skybox fails)
        _mainCamera.backgroundColor = new Color(0.5f, 0.8f, 1f, 1f); // Light blue
        
        Debug.Log($"✅ Camera settings fixed - Far: {_improvedFarClipPlane}, Near: {_improvedNearClipPlane}");
    }
    
    public void FixSkyboxSettings()
    {
        Debug.Log("🌌 Fixing skybox settings...");
        
        // Apply current skybox or fallback
        Material currentSkybox = RenderSettings.skybox;
        
        if (currentSkybox == null)
        {
            if (_defaultDaySkybox != null)
            {
                RenderSettings.skybox = _defaultDaySkybox;
                Debug.Log($"✅ Applied default day skybox: {_defaultDaySkybox.name}");
            }
            else if (_fallbackSkybox != null)
            {
                RenderSettings.skybox = _fallbackSkybox;
                Debug.Log($"✅ Applied fallback skybox: {_fallbackSkybox.name}");
            }
            else
            {
                // Create a procedural skybox as last resort
                CreateFallbackSkybox();
            }
        }
        
        // Fix fog settings to prevent black borders
        FixFogSettings();
        
        // Force skybox refresh
        DynamicGI.UpdateEnvironment();
    }
    
    private void CreateFallbackSkybox()
    {
        Debug.Log("🔧 Creating fallback procedural skybox...");
        
        // Create a simple procedural skybox material
        Material proceduralSkybox = new Material(Shader.Find("Skybox/Procedural"));
        if (proceduralSkybox != null)
        {
            // Set nice default values
            proceduralSkybox.SetFloat("_AtmosphereThickness", 1.0f);
            proceduralSkybox.SetFloat("_SunSize", 0.04f);
            proceduralSkybox.SetFloat("_SunSizeConvergence", 5f);
            proceduralSkybox.SetColor("_SkyTint", new Color(0.5f, 0.5f, 0.5f, 1f));
            proceduralSkybox.SetColor("_GroundColor", new Color(0.369f, 0.349f, 0.341f, 1f));
            proceduralSkybox.SetFloat("_Exposure", 1.3f);
            
            RenderSettings.skybox = proceduralSkybox;
            _fallbackSkybox = proceduralSkybox;
            
            Debug.Log("✅ Fallback procedural skybox created and applied");
        }
    }
    
    private void FixFogSettings()
    {
        // Enable fog to help blend sky and water
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        
        // Get current time of day for appropriate fog
        TimeOfDayController timeController = FindObjectOfType<TimeOfDayController>();
        if (timeController != null)
        {
            string currentTime = timeController.GetCurrentTimeName().ToLower();
            
            if (currentTime.Contains("night"))
            {
                RenderSettings.fogColor = new Color(0.1f, 0.15f, 0.3f, 1f); // Dark blue
                RenderSettings.fogDensity = 0.012f;
            }
            else if (currentTime.Contains("evening"))
            {
                RenderSettings.fogColor = new Color(0.8f, 0.6f, 0.4f, 1f); // Orange
                RenderSettings.fogDensity = 0.008f;
            }
            else
            {
                RenderSettings.fogColor = new Color(0.7f, 0.8f, 0.9f, 1f); // Light blue
                RenderSettings.fogDensity = 0.005f;
            }
        }
        else
        {
            // Default day fog
            RenderSettings.fogColor = new Color(0.7f, 0.8f, 0.9f, 1f);
            RenderSettings.fogDensity = 0.005f;
        }
        
        Debug.Log($"✅ Fog settings applied - Color: {RenderSettings.fogColor}, Density: {RenderSettings.fogDensity}");
    }
    
    public void FixTimeOfDayController()
    {
        Debug.Log("🕐 Fixing TimeOfDayController skybox assignments...");
        
        TimeOfDayController timeController = FindObjectOfType<TimeOfDayController>();
        if (timeController == null)
        {
            Debug.LogWarning("⚠️ No TimeOfDayController found!");
            return;
        }
        
        // Use reflection to access and fix the skybox materials
        System.Type timeControllerType = timeController.GetType();
        
        // Get the settings fields
        var morningField = timeControllerType.GetField("morningSettings");
        var noonField = timeControllerType.GetField("noonSettings");
        var eveningField = timeControllerType.GetField("eveningSettings");
        var nightField = timeControllerType.GetField("nightSettings");
        
        if (morningField != null && _defaultDaySkybox != null)
        {
            var morningSettings = morningField.GetValue(timeController);
            var skyboxField = morningSettings.GetType().GetField("skyboxMaterial");
            if (skyboxField != null)
            {
                skyboxField.SetValue(morningSettings, _defaultDaySkybox);
                Debug.Log("✅ Morning skybox assigned");
            }
        }
        
        if (noonField != null && _defaultDaySkybox != null)
        {
            var noonSettings = noonField.GetValue(timeController);
            var skyboxField = noonSettings.GetType().GetField("skyboxMaterial");
            if (skyboxField != null)
            {
                skyboxField.SetValue(noonSettings, _defaultDaySkybox);
                Debug.Log("✅ Noon skybox assigned");
            }
        }
        
        if (eveningField != null && _defaultDaySkybox != null)
        {
            var eveningSettings = eveningField.GetValue(timeController);
            var skyboxField = eveningSettings.GetType().GetField("skyboxMaterial");
            if (skyboxField != null)
            {
                skyboxField.SetValue(eveningSettings, _defaultDaySkybox);
                Debug.Log("✅ Evening skybox assigned");
            }
        }
        
        if (nightField != null)
        {
            var nightSettings = nightField.GetValue(timeController);
            var skyboxField = nightSettings.GetType().GetField("skyboxMaterial");
            if (skyboxField != null)
            {
                Material nightSkybox = _defaultNightSkybox ?? _defaultDaySkybox ?? _fallbackSkybox;
                skyboxField.SetValue(nightSettings, nightSkybox);
                Debug.Log("✅ Night skybox assigned");
            }
        }
    }
    
    [ContextMenu("Reset Water to Default")]
    public void ResetWaterToDefault()
    {
        if (_waterObject != null)
        {
            _waterObject.transform.localScale = new Vector3(10000f, 1f, 10000f);
            _waterObject.transform.position = new Vector3(_waterObject.transform.position.x, 1f, _waterObject.transform.position.z);
            Debug.Log("🔄 Water reset to original settings");
        }
    }
    
    [ContextMenu("Test Skybox Switching")]
    public void TestSkyboxSwitching()
    {
        TimeOfDayController timeController = FindObjectOfType<TimeOfDayController>();
        if (timeController != null)
        {
            Debug.Log("🧪 Testing skybox switching...");
            timeController.SetMorning();
            Invoke(nameof(TestNoon), 2f);
            Invoke(nameof(TestEvening), 4f);
            Invoke(nameof(TestNight), 6f);
        }
    }
    
    private void TestNoon() => FindObjectOfType<TimeOfDayController>()?.SetNoon();
    private void TestEvening() => FindObjectOfType<TimeOfDayController>()?.SetEvening();
    private void TestNight() => FindObjectOfType<TimeOfDayController>()?.SetNight();
}