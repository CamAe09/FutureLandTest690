using UnityEngine;

/// <summary>
/// Fixes the horizontal line issue in skybox rendering
/// This specific issue is caused by camera far clip plane being too low
/// </summary>
public class SkyboxLineFix : MonoBehaviour
{
    [Header("🔧 SKYBOX LINE FIX")]
    [SerializeField] private bool _fixAllCameras = true;
    [SerializeField] private float _newFarClipPlane = 15000f;
    [SerializeField] private bool _applyFix = false;
    
    [Header("📊 Current Status")]
    [SerializeField] private Camera[] _foundCameras;
    [SerializeField] private bool _issueDetected = false;
    [SerializeField] private string _diagnosisResult = "";
    
    void Start()
    {
        if (_fixAllCameras)
        {
            ApplyFix();
        }
    }
    
    void OnValidate()
    {
        if (_applyFix)
        {
            _applyFix = false;
            if (Application.isPlaying)
            {
                ApplyFix();
            }
        }
    }
    
    [ContextMenu("Apply Skybox Line Fix")]
    public void ApplyFix()
    {
        Debug.Log("🔧 === FIXING SKYBOX LINE ISSUE ===");
        
        // Find all cameras in the scene
        Camera[] allCameras = FindObjectsOfType<Camera>();
        _foundCameras = allCameras;
        
        int fixedCount = 0;
        _issueDetected = false;
        
        foreach (Camera cam in allCameras)
        {
            if (cam.farClipPlane < 5000f) // Anything below 5000 can cause skybox cutoff
            {
                _issueDetected = true;
                float oldFarPlane = cam.farClipPlane;
                
                // Fix the far clip plane
                cam.farClipPlane = _newFarClipPlane;
                
                // Ensure clear flags are set to skybox
                if (cam.clearFlags != CameraClearFlags.Skybox)
                {
                    cam.clearFlags = CameraClearFlags.Skybox;
                    Debug.Log($"🔧 Fixed {cam.name}: Clear flags set to Skybox");
                }
                
                // Set a reasonable near clip plane if it's too high
                if (cam.nearClipPlane > 1f)
                {
                    cam.nearClipPlane = 0.1f;
                    Debug.Log($"🔧 Fixed {cam.name}: Near clip plane reduced to 0.1");
                }
                
                Debug.Log($"✅ FIXED {cam.name}: Far clip plane {oldFarPlane} → {_newFarClipPlane}");
                fixedCount++;
            }
            else
            {
                Debug.Log($"✅ {cam.name}: Far clip plane OK ({cam.farClipPlane})");
            }
        }
        
        if (_issueDetected)
        {
            _diagnosisResult = $"Fixed {fixedCount} cameras with low far clip planes";
            Debug.Log($"🎉 SKYBOX LINE FIX COMPLETE: {_diagnosisResult}");
            Debug.Log("📋 The horizontal line in your skybox should now be gone!");
        }
        else
        {
            _diagnosisResult = "No camera issues detected";
            Debug.Log("✅ All cameras already have proper settings");
        }
        
        // Force skybox refresh
        DynamicGI.UpdateEnvironment();
    }
    
    [ContextMenu("Diagnose Skybox Line Issue")]
    public void DiagnoseSkyboxLineIssue()
    {
        Debug.Log("🔍 === DIAGNOSING SKYBOX LINE ISSUE ===");
        
        Camera[] allCameras = FindObjectsOfType<Camera>();
        _foundCameras = allCameras;
        
        bool foundIssues = false;
        
        foreach (Camera cam in allCameras)
        {
            Debug.Log($"📷 Camera: {cam.name}");
            Debug.Log($"   Far Clip Plane: {cam.farClipPlane}");
            Debug.Log($"   Near Clip Plane: {cam.nearClipPlane}");
            Debug.Log($"   Clear Flags: {cam.clearFlags}");
            
            // Check for common issues that cause skybox lines
            if (cam.farClipPlane < 5000f)
            {
                Debug.LogWarning($"❌ ISSUE: {cam.name} far clip plane too low ({cam.farClipPlane}) - causes skybox cutoff!");
                foundIssues = true;
            }
            
            if (cam.clearFlags != CameraClearFlags.Skybox)
            {
                Debug.LogWarning($"❌ ISSUE: {cam.name} clear flags not set to Skybox ({cam.clearFlags})");
                foundIssues = true;
            }
            
            if (cam.nearClipPlane > 1f)
            {
                Debug.LogWarning($"⚠️ WARNING: {cam.name} near clip plane high ({cam.nearClipPlane}) - may cause precision issues");
            }
        }
        
        // Check skybox material
        if (RenderSettings.skybox == null)
        {
            Debug.LogError("❌ CRITICAL: No skybox material assigned in Render Settings!");
            foundIssues = true;
        }
        else
        {
            Debug.Log($"✅ Skybox material: {RenderSettings.skybox.name}");
        }
        
        // Check fog settings
        if (RenderSettings.fog)
        {
            Debug.Log($"📊 Fog enabled: Density={RenderSettings.fogDensity}, Color={RenderSettings.fogColor}");
            if (RenderSettings.fogDensity > 0.02f)
            {
                Debug.LogWarning($"⚠️ WARNING: Fog density high ({RenderSettings.fogDensity}) - may create harsh boundaries");
            }
        }
        else
        {
            Debug.Log("📊 Fog disabled");
        }
        
        _issueDetected = foundIssues;
        
        if (foundIssues)
        {
            _diagnosisResult = "Issues detected - run ApplyFix()";
            Debug.Log("🚨 CONCLUSION: Issues found that can cause skybox line problems!");
            Debug.Log("💡 SOLUTION: Click 'Apply Skybox Line Fix' button or call ApplyFix()");
        }
        else
        {
            _diagnosisResult = "No issues detected";
            Debug.Log("✅ CONCLUSION: No obvious issues found");
            Debug.Log("💭 If line still appears, check terrain/water height and skybox material quality");
        }
        
        Debug.Log("==================================");
    }
    
    [ContextMenu("Test Different Far Clip Values")]
    public void TestDifferentFarClipValues()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("❌ No main camera found for testing!");
            return;
        }
        
        Debug.Log("🧪 Testing different far clip plane values...");
        
        // Test sequence: 1000 → 5000 → 10000 → 15000
        StartCoroutine(TestFarClipSequence(mainCam));
    }
    
    System.Collections.IEnumerator TestFarClipSequence(Camera cam)
    {
        float[] testValues = { 1000f, 5000f, 10000f, 15000f };
        float originalValue = cam.farClipPlane;
        
        foreach (float testValue in testValues)
        {
            Debug.Log($"🔬 Testing far clip plane: {testValue}");
            cam.farClipPlane = testValue;
            yield return new WaitForSeconds(3f);
        }
        
        // Set to optimal value
        cam.farClipPlane = _newFarClipPlane;
        Debug.Log($"✅ Test complete - set to optimal value: {_newFarClipPlane}");
    }
    
    void Update()
    {
        // Hotkeys disabled - use inspector buttons or context menu instead  
        // (Project uses new Input System package, not legacy Input)
    }
}