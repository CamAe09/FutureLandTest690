using UnityEngine;

namespace TPSBR
{
    /// <summary>
    /// TEMP REPLACEMENT: This replaces the problematic ShopUISetup.cs
    /// Use ShopUISetup_NEW.cs for full functionality
    /// </summary>
    public class ShopUISetup_TEMP : MonoBehaviour
    {
        [ContextMenu("Use ShopUISetup_NEW Instead")]
        public void ShowMessage()
        {
            Debug.LogWarning("⚠️ This is a temporary replacement for the problematic ShopUISetup.cs");
            Debug.Log("💡 Use ShopUISetup_NEW.cs for full shop setup functionality.");
        }
    }
}