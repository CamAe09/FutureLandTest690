using UnityEngine;

namespace TPSBR
{
    /// <summary>
    /// Helper to fix build compilation issues
    /// </summary>
    public class BuildFixHelper : MonoBehaviour
    {
        [Header("Build Fix Helper")]
        [SerializeField] private bool _fixBuildIssues = false;
        
        private void Update()
        {
            if (_fixBuildIssues)
            {
                _fixBuildIssues = false;
                FixBuildIssues();
            }
        }
        
        [ContextMenu("Fix Build Issues")]
        public void FixBuildIssues()
        {
            Debug.Log("🔧 Checking for build compilation issues...");
            
            // Check if problematic scripts exist
            var shopUISetup = FindObjectOfType<ShopUISetup>();
            if (shopUISetup != null)
            {
                Debug.Log("⚠️ Found ShopUISetup component - may cause build issues");
                Debug.Log("💡 Recommendation: Use ShopUISetup_NEW instead (build-compatible)");
            }
            
            // Check for essential systems
            bool shopManagerExists = FindObjectOfType<ShopManager>() != null;
            bool currencyManagerExists = FindObjectOfType<CurrencyManager>() != null;
            
            Debug.Log($"🏪 Shop Manager: {(shopManagerExists ? "✅ Found" : "❌ Missing")}");
            Debug.Log($"💰 Currency Manager: {(currencyManagerExists ? "✅ Found" : "❌ Missing")}");
            
            if (!shopManagerExists)
            {
                Debug.LogWarning("⚠️ ShopManager missing! Your shop won't work without it.");
            }
            
            if (!currencyManagerExists)
            {
                Debug.LogWarning("⚠️ CurrencyManager missing! Coin system won't work without it.");
            }
            
            Debug.Log("✅ Build issue check complete!");
        }
        
        [ContextMenu("Create Essential Shop Components")]
        public void CreateEssentialShopComponents()
        {
            Debug.Log("🛠️ Creating essential shop components...");
            
            // Create Shop Manager if missing
            if (FindObjectOfType<ShopManager>() == null)
            {
                GameObject shopManagerObj = new GameObject("Shop Manager");
                shopManagerObj.AddComponent<ShopManager>();
                Debug.Log("✅ Created Shop Manager");
            }
            
            // Create Currency Manager if missing
            if (FindObjectOfType<CurrencyManager>() == null)
            {
                GameObject currencyManagerObj = new GameObject("Currency Manager");
                currencyManagerObj.AddComponent<CurrencyManager>();
                Debug.Log("✅ Created Currency Manager");
            }
            
            Debug.Log("🎉 Essential shop components created!");
        }
    }
}