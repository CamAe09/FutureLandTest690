using UnityEngine;

namespace TPSBR
{
    /// <summary>
    /// DEPRECATED: This script had build compatibility issues.
    /// Use ShopUISetup_NEW.cs instead for build-compatible shop setup.
    /// </summary>
    public class ShopUISetup : MonoBehaviour
    {
        [ContextMenu("⚠️ Use ShopUISetup_NEW Instead")]
        public void ShowDeprecationMessage()
        {
            Debug.LogWarning("⚠️ ShopUISetup is deprecated due to build compilation issues.");
            Debug.Log("💡 Use ShopUISetup_NEW.cs instead for build-compatible shop setup.");
            
            // Try to find the new version
            var newSetup = FindObjectOfType<ShopUISetup_NEW>();
            if (newSetup == null)
            {
                Debug.Log("🔧 Creating ShopUISetup_NEW component...");
                gameObject.AddComponent<ShopUISetup_NEW>();
                Debug.Log("✅ ShopUISetup_NEW component added! Use the context menu to create your shop.");
            }
            else
            {
                Debug.Log("✅ ShopUISetup_NEW already exists in the scene!");
            }
        }
        
        [ContextMenu("🛠️ Create Essential Shop Components")]
        public void CreateEssentialComponents()
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
        
        private void Start()
        {
            Debug.LogWarning($"⚠️ GameObject '{name}' is using deprecated ShopUISetup script!");
            Debug.Log("💡 Right-click this component and select 'Use ShopUISetup_NEW Instead' to upgrade.");
        }
    }
}