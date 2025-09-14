using UnityEngine;

/// <summary>
/// Comprehensive testing script for skybox and water fixes
/// Helps diagnose and validate the visual improvements
/// </summary>
public class SkyboxWaterTester : MonoBehaviour
{
    [Header("Test Controls")]
    [SerializeField] private bool _testWaterIssues = false;
    [SerializeField] private bool _testSkyboxIssues = false;
    [SerializeField] private bool _testNightBorder = false;
    [SerializeField] private bool _testTimeOfDaySwitch = false;
    [SerializeField] private bool _runFullDiagnostic = false;
    
    [Header("Current Status")]
    [SerializeField, ReadOnly] private bool _waterObjectFound = false;
    [SerializeField, ReadOnly] private bool _skyboxAssigned = false;
    [SerializeField, ReadOnly] private bool _cameraSettingsOK = false;
    [SerializeField, ReadOnly] private bool _fogSettingsOK = false;
    [SerializeField, ReadOnly] private string _currentTimeOfDay = "Unknown";
    
    [Header("Diagnostic Results")]
    [SerializeField, ReadOnly] private Vector3 _waterScale = Vector3.zero;
    [SerializeField, ReadOnly] private float _waterHeight = 0f;
    [SerializeField, ReadOnly] private float _cameraFarPlane = 0f;
    [SerializeField, ReadOnly] private float _fogDensity = 0f;
    [SerializeField, ReadOnly] private string _skyboxShader = "None";
    
    private SkyboxWaterFix _skyboxFix;
    private TimeOfDayController _timeController;
    private Camera _mainCamera;
    private GameObject _waterObject;
    
    void Start()
    {
        FindComponents();
        RunInitialDiagnostic();
    }
    
    void OnValidate()
    {
        if (!Application.isPlaying) return;
        
        if (_testWaterIssues)
        {
            _testWaterIssues = false;
            TestWaterIssues();
        }
        
        if (_testSkyboxIssues)
        {
            _testSkyboxIssues = false;
            TestSkyboxIssues();
        }
        
        if (_testNightBorder)
        {
            _testNightBorder = false;
            TestNightBorderIssue();
        }
        
        if (_testTimeOfDaySwitch)
        {
            _testTimeOfDaySwitch = false;
            TestTimeOfDaySwitch();
        }
        
        if (_runFullDiagnostic)
        {
            _runFullDiagnostic = false;
            RunFullDiagnostic();
        }
    }
    
    void Update()
    {
        // Update real-time status
        UpdateDiagnosticInfo();
        
        // Hotkeys disabled - use inspector buttons instead
        // (Project uses new Input System package, not legacy Input)
    }
    
    void FindComponents()
    {
        _skyboxFix = FindObjectOfType<SkyboxWaterFix>();
        _timeController = FindObjectOfType<TimeOfDayController>();
        _mainCamera = Camera.main ?? FindObjectOfType<Camera>();
        
        _waterObject = GameObject.FindGameObjectWithTag("Water");
        if (_waterObject == null)
        {
            _waterObject = GameObject.Find("Water");
        }
        
        Debug.Log("🔍 SkyboxWaterTester: Components found");
    }
    
    [ContextMenu("Run Full Diagnostic")]
    public void RunFullDiagnostic()
    {
        Debug.Log("🧪 === RUNNING FULL SKYBOX & WATER DIAGNOSTIC ===");
        
        RunInitialDiagnostic();
        TestWaterIssues();
        TestSkyboxIssues();
        TestCameraSettings();
        TestFogSettings();
        
        ShowDiagnosticSummary();
    }
    
    void RunInitialDiagnostic()
    {
        Debug.Log("🔧 Running initial diagnostic...");
        
        _waterObjectFound = _waterObject != null;
        _skyboxAssigned = RenderSettings.skybox != null;
        _cameraSettingsOK = _mainCamera != null && _mainCamera.farClipPlane > 10000f;
        _fogSettingsOK = RenderSettings.fog && RenderSettings.fogDensity < 0.02f;
        
        if (_timeController != null)
        {
            _currentTimeOfDay = _timeController.GetCurrentTimeName();
        }
        
        UpdateDiagnosticInfo();
    }
    
    void UpdateDiagnosticInfo()
    {
        if (_waterObject != null)
        {
            _waterScale = _waterObject.transform.localScale;
            _waterHeight = _waterObject.transform.position.y;
        }
        
        if (_mainCamera != null)
        {
            _cameraFarPlane = _mainCamera.farClipPlane;
        }
        
        _fogDensity = RenderSettings.fogDensity;
        
        if (RenderSettings.skybox != null && RenderSettings.skybox.shader != null)
        {
            _skyboxShader = RenderSettings.skybox.shader.name;
        }
    }
    
    [ContextMenu("Test Water Issues")]
    public void TestWaterIssues()
    {
        Debug.Log("🌊 Testing water rendering issues...");
        
        if (_waterObject == null)
        {
            Debug.LogError("❌ WATER ISSUE: No water object found!");
            return;
        }
        
        // Check water scale (common cause of cutting issues)
        Vector3 scale = _waterObject.transform.localScale;
        if (scale.x > 8000f || scale.z > 8000f)
        {
            Debug.LogWarning($"⚠️ WATER ISSUE: Water scale too large ({scale}) - may cause precision issues");
            Debug.Log("💡 SOLUTION: Apply SkyboxWaterFix to reduce scale");
        }
        else
        {
            Debug.Log($"✅ Water scale OK: {scale}");
        }
        
        // Check water height
        float height = _waterObject.transform.position.y;
        if (height < 0f || height > 10f)
        {
            Debug.LogWarning($"⚠️ WATER ISSUE: Water height unusual ({height}) - may cause visibility issues");
        }
        else
        {
            Debug.Log($"✅ Water height OK: {height}");
        }
        
        // Check water material
        Renderer waterRenderer = _waterObject.GetComponent<Renderer>();
        if (waterRenderer != null)
        {
            if (waterRenderer.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off)
            {
                Debug.LogWarning("⚠️ WATER ISSUE: Water is casting shadows - may cause cutting artifacts");
            }
            else
            {
                Debug.Log("✅ Water shadow casting disabled");
            }
        }
    }
    
    [ContextMenu("Test Skybox Issues")]
    public void TestSkyboxIssues()
    {
        Debug.Log("🌌 Testing skybox rendering issues...");
        
        if (RenderSettings.skybox == null)
        {
            Debug.LogError("❌ SKYBOX ISSUE: No skybox material assigned!");
            Debug.Log("💡 SOLUTION: Apply SkyboxWaterFix to assign skybox materials");
            return;
        }
        
        Material skybox = RenderSettings.skybox;
        Debug.Log($"✅ Skybox assigned: {skybox.name} ({skybox.shader.name})");
        
        // Check if skybox shader is appropriate
        if (skybox.shader.name.Contains("Skybox"))
        {
            Debug.Log("✅ Skybox shader appropriate");
        }
        else
        {
            Debug.LogWarning($"⚠️ SKYBOX ISSUE: Shader '{skybox.shader.name}' may not be suitable for skybox");
        }
        
        // Test TimeOfDayController skybox assignments
        if (_timeController != null)
        {
            System.Type timeControllerType = _timeController.GetType();
            var settingsFields = new[] { "morningSettings", "noonSettings", "eveningSettings", "nightSettings" };
            
            foreach (string fieldName in settingsFields)
            {
                var field = timeControllerType.GetField(fieldName);
                if (field != null)
                {
                    var settings = field.GetValue(_timeController);
                    var skyboxField = settings.GetType().GetField("skyboxMaterial");
                    var skyboxMaterial = (Material)skyboxField.GetValue(settings);
                    
                    if (skyboxMaterial == null)
                    {
                        Debug.LogWarning($"⚠️ SKYBOX ISSUE: No skybox assigned for {fieldName}");
                    }
                    else
                    {
                        Debug.Log($"✅ {fieldName} skybox: {skyboxMaterial.name}");
                    }
                }
            }
        }
    }
    
    [ContextMenu("Test Night Border Issue")]
    public void TestNightBorderIssue()
    {
        Debug.Log("🌙 Testing night time black border issue...");
        
        if (_timeController != null)
        {
            // Switch to night time to test
            _timeController.SetNight();
            
            // Wait a frame then check settings
            StartCoroutine(CheckNightSettings());
        }
        else
        {
            Debug.LogError("❌ No TimeOfDayController found for night testing!");
        }
    }
    
    System.Collections.IEnumerator CheckNightSettings()
    {
        yield return null; // Wait one frame
        
        // Check fog density (main cause of black borders)
        if (RenderSettings.fogDensity > 0.012f)
        {
            Debug.LogWarning($"⚠️ NIGHT ISSUE: Fog density too high ({RenderSettings.fogDensity}) - may cause black borders");
            Debug.Log("💡 SOLUTION: Reduce fog density or apply SkyboxWaterFix");
        }
        else
        {
            Debug.Log($"✅ Night fog density OK: {RenderSettings.fogDensity}");
        }
        
        // Check ambient lighting
        if (RenderSettings.ambientIntensity < 0.2f)
        {
            Debug.LogWarning($"⚠️ NIGHT ISSUE: Ambient intensity too low ({RenderSettings.ambientIntensity}) - may cause pure black areas");
        }
        else
        {
            Debug.Log($"✅ Night ambient intensity OK: {RenderSettings.ambientIntensity}");
        }
        
        // Check skybox
        if (RenderSettings.skybox == null)
        {
            Debug.LogError("❌ NIGHT ISSUE: No skybox for night time!");
        }
        else
        {
            Debug.Log($"✅ Night skybox: {RenderSettings.skybox.name}");
        }
    }
    
    [ContextMenu("Test Time of Day Switch")]
    public void TestTimeOfDaySwitch()
    {
        if (_timeController == null)
        {
            Debug.LogError("❌ No TimeOfDayController found!");
            return;
        }
        
        Debug.Log("🕐 Testing time of day switching...");
        StartCoroutine(CycleTimeOfDay());
    }
    
    System.Collections.IEnumerator CycleTimeOfDay()
    {
        string[] times = { "Morning", "Noon", "Evening", "Night" };
        System.Action[] actions = { 
            _timeController.SetMorning, 
            _timeController.SetNoon, 
            _timeController.SetEvening, 
            _timeController.SetNight 
        };
        
        for (int i = 0; i < times.Length; i++)
        {
            Debug.Log($"🕐 Switching to {times[i]}...");
            actions[i]();
            
            yield return new WaitForSeconds(1f);
            
            // Validate the switch
            if (RenderSettings.skybox != null)
            {
                Debug.Log($"✅ {times[i]} skybox: {RenderSettings.skybox.name}");
            }
            else
            {
                Debug.LogWarning($"⚠️ No skybox for {times[i]}!");
            }
            
            yield return new WaitForSeconds(2f);
        }
        
        Debug.Log("✅ Time of day switching test complete!");
    }
    
    void TestCameraSettings()
    {
        Debug.Log("📷 Testing camera settings...");
        
        if (_mainCamera == null)
        {
            Debug.LogError("❌ CAMERA ISSUE: No main camera found!");
            return;
        }
        
        // Check far clip plane
        if (_mainCamera.farClipPlane < 10000f)
        {
            Debug.LogWarning($"⚠️ CAMERA ISSUE: Far clip plane too low ({_mainCamera.farClipPlane}) - may cause skybox cutoff");
        }
        else
        {
            Debug.Log($"✅ Camera far clip plane OK: {_mainCamera.farClipPlane}");
        }
        
        // Check clear flags
        if (_mainCamera.clearFlags != CameraClearFlags.Skybox)
        {
            Debug.LogWarning($"⚠️ CAMERA ISSUE: Clear flags set to {_mainCamera.clearFlags} instead of Skybox");
        }
        else
        {
            Debug.Log("✅ Camera clear flags set to Skybox");
        }
    }
    
    void TestFogSettings()
    {
        Debug.Log("🌫️ Testing fog settings...");
        
        if (!RenderSettings.fog)
        {
            Debug.LogWarning("⚠️ FOG ISSUE: Fog disabled - may cause harsh transitions");
        }
        else
        {
            Debug.Log("✅ Fog enabled");
        }
        
        if (RenderSettings.fogDensity > 0.02f)
        {
            Debug.LogWarning($"⚠️ FOG ISSUE: Fog density too high ({RenderSettings.fogDensity}) - may obscure distant objects");
        }
        else
        {
            Debug.Log($"✅ Fog density appropriate: {RenderSettings.fogDensity}");
        }
    }
    
    void ShowDiagnosticSummary()
    {
        Debug.Log("🧪 === DIAGNOSTIC SUMMARY ===");
        Debug.Log($"   Water Object: {(_waterObjectFound ? "✅" : "❌")}");
        Debug.Log($"   Skybox Assigned: {(_skyboxAssigned ? "✅" : "❌")}");
        Debug.Log($"   Camera Settings: {(_cameraSettingsOK ? "✅" : "❌")}");
        Debug.Log($"   Fog Settings: {(_fogSettingsOK ? "✅" : "❌")}");
        Debug.Log($"   Current Time: {_currentTimeOfDay}");
        Debug.Log("============================");
        
        bool allGood = _waterObjectFound && _skyboxAssigned && _cameraSettingsOK && _fogSettingsOK;
        
        if (allGood)
        {
            Debug.Log("🎉 ALL SYSTEMS GREEN! Skybox and water should be working properly!");
        }
        else
        {
            Debug.Log("⚠️ Some issues detected. Consider applying SkyboxWaterFix component.");
        }
        
        Debug.Log("\n📋 Manual test instructions:");
        Debug.Log("   • F2 = Run full diagnostic");
        Debug.Log("   • F3 = Test night border issue");
        Debug.Log("   • F4 = Test time of day switching");
        Debug.Log("   • Check water doesn't cut in/out when jumping from plane");
        Debug.Log("   • Verify no black borders between sky and water at night");
    }
}