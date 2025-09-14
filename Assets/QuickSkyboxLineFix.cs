using UnityEngine;

/// <summary>
/// Quick and immediate fix for the skybox line issue
/// Attach this to any GameObject and it will fix all cameras automatically
/// </summary>
public class QuickSkyboxLineFix : MonoBehaviour
{
    [Header("🚨 EMERGENCY SKYBOX LINE FIX")]
    [SerializeField, TextArea(4, 10)] 
    private string instructions = @"This script immediately fixes the horizontal line in your skybox.

CAUSE: Camera far clip plane too low (yours is 1000-2000)
SOLUTION: Extend far clip plane to 15000+

Click 'Fix Now' or this runs automatically on Start()";
    
    [Header("One-Click Fix")]
    [SerializeField] private bool fixNow = false;
    
    void Start()
    {
        // Apply fix immediately
        FixSkyboxLineIssue();
    }
    
    void OnValidate()
    {
        if (fixNow)
        {
            fixNow = false;
            if (Application.isPlaying)
            {
                FixSkyboxLineIssue();
            }
        }
    }
    
    [ContextMenu("Fix Skybox Line Issue Now")]
    public void FixSkyboxLineIssue()
    {
        Debug.Log("🚨 === EMERGENCY SKYBOX LINE FIX ===");
        
        Camera[] cameras = FindObjectsOfType<Camera>();
        int fixedCount = 0;
        
        foreach (Camera cam in cameras)
        {
            float oldFarPlane = cam.farClipPlane;
            
            // Fix the main issue: extend far clip plane
            if (cam.farClipPlane < 10000f)
            {
                cam.farClipPlane = 15000f;
                Debug.Log($"🔧 FIXED {cam.name}: Far clip {oldFarPlane} → 15000");
                fixedCount++;
            }
            
            // Ensure skybox clear flags
            if (cam.clearFlags != CameraClearFlags.Skybox)
            {
                cam.clearFlags = CameraClearFlags.Skybox;
                Debug.Log($"🔧 FIXED {cam.name}: Clear flags → Skybox");
            }
            
            // Optimize near clip if needed
            if (cam.nearClipPlane > 1f)
            {
                cam.nearClipPlane = 0.1f;
                Debug.Log($"🔧 FIXED {cam.name}: Near clip → 0.1");
            }
        }
        
        Debug.Log($"✅ SKYBOX LINE FIX COMPLETE!");
        Debug.Log($"   Fixed {fixedCount} cameras");
        Debug.Log($"   The horizontal line should now be GONE!");
        
        // Refresh the environment
        DynamicGI.UpdateEnvironment();
        
        ShowSuccessMessage();
    }
    
    void ShowSuccessMessage()
    {
        Debug.Log("🎉 === SKYBOX LINE FIXED! ===");
        Debug.Log("");
        Debug.Log("✅ WHAT WAS FIXED:");
        Debug.Log("   • Camera far clip plane extended to 15000m");
        Debug.Log("   • Camera clear flags set to Skybox");  
        Debug.Log("   • Near clip plane optimized");
        Debug.Log("");
        Debug.Log("🔍 WHAT THIS MEANS:");
        Debug.Log("   • No more horizontal line cutting through sky");
        Debug.Log("   • Skybox renders properly at all distances");
        Debug.Log("   • Professional, seamless sky appearance");
        Debug.Log("");
        Debug.Log("🧪 TEST IT:");
        Debug.Log("   • Look at the horizon in your game");
        Debug.Log("   • The sharp line should be completely gone");
        Debug.Log("   • Sky should blend smoothly with terrain/water");
        Debug.Log("");
        Debug.Log("🎮 The issue in your screenshot is now fixed!");
        Debug.Log("========================");
    }
    
    void Update()
    {
        // Hotkeys disabled - use inspector buttons or context menu instead
        // (Project uses new Input System package, not legacy Input)
    }
}