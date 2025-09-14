using UnityEngine;

[System.Serializable]
public class RunInstaller
{
    // Disabled to prevent conflicts with ShopManager
    // [RuntimeInitializeOnLoadMethod]
    public static void CreateShopUI()
    {
        GameObject installer = new GameObject("Shop UI Installer");
        installer.AddComponent<TempInstaller>();
    }
}