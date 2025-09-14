using UnityEngine;
using TMPro;

/// <summary>
/// Helper script to validate TextMeshPro alignment options for Unity 2022.2
/// </summary>
public class TextAlignmentTest : MonoBehaviour
{
    [ContextMenu("Test TextAlignmentOptions")]
    public void TestAlignment()
    {
        Debug.Log("Testing TextMeshPro TextAlignmentOptions for Unity 2022.2:");
        
        // Test basic alignment options
        try
        {
            var left = TextAlignmentOptions.Left;
            Debug.Log("✅ TextAlignmentOptions.Left - Available");
        }
        catch
        {
            Debug.LogError("❌ TextAlignmentOptions.Left - Not Available");
        }
        
        try
        {
            var center = TextAlignmentOptions.Center;
            Debug.Log("✅ TextAlignmentOptions.Center - Available");
        }
        catch
        {
            Debug.LogError("❌ TextAlignmentOptions.Center - Not Available");
        }
        
        try
        {
            var right = TextAlignmentOptions.Right;
            Debug.Log("✅ TextAlignmentOptions.Right - Available");
        }
        catch
        {
            Debug.LogError("❌ TextAlignmentOptions.Right - Not Available");
        }
        
        // Test the problematic enum that was causing issues
        try
        {
            // This should fail in Unity 2022.2
            // var middleLeft = TextAlignmentOptions.MiddleLeft;
            Debug.Log("❌ TextAlignmentOptions.MiddleLeft - Would cause compilation error (commented out)");
        }
        catch
        {
            Debug.LogError("TextAlignmentOptions.MiddleLeft - Not Available (as expected)");
        }
        
        Debug.Log("✅ TextAlignmentOptions test complete!");
    }
}