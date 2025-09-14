using UnityEngine;

namespace TPSBR
{
    public class InstallShop : MonoBehaviour
    {
        void Start()
        {
            ShopUIInstaller.InstallShopUI();
            
            // Create some sample shop items
            CreateSampleShopItems();
            
            // Clean up installer
            Destroy(gameObject);
        }
        
        private void CreateSampleShopItems()
        {
            Debug.Log("📦 To create shop items:");
            Debug.Log("   1. Right-click in Project window");
            Debug.Log("   2. Choose Create → TPSBR → Shop Item");
            Debug.Log("   3. Configure the item (name, cost, description, etc.)");
            Debug.Log("   4. Add the ShopItem to the ShopManager's Available Items list");
            Debug.Log("   5. Press B in Play mode to test!");
        }
    }
}