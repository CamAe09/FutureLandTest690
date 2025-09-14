using UnityEngine;
using UnityEngine.UI;
using TPSBR.UI;

namespace TPSBR
{
    public class MenuShopButton : MonoBehaviour
    {
        [Header("Shop Button")]
        [SerializeField] private UIButton _shopButton;

        private void Start()
        {
            // Find the shop button if not assigned
            if (_shopButton == null)
            {
                _shopButton = GetComponent<UIButton>();
            }

            // Connect the button click to open shop
            if (_shopButton != null)
            {
                _shopButton.onClick.AddListener(OpenShop);
                Debug.Log("🛒 Shop button connected to ShopManager");
            }
            else
            {
                Debug.LogError("❌ UIButton component not found on MenuShopButton!");
            }
        }

        private void OpenShop()
        {
            Debug.Log("🛒 Shop button clicked - opening shop...");

            // Find and open the shop
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.OpenShop();
            }
            else
            {
                Debug.LogError("❌ ShopManager.Instance not found! Make sure Shop System GameObject is active.");
            }
        }

        private void OnDestroy()
        {
            // Clean up the button listener
            if (_shopButton != null)
            {
                _shopButton.onClick.RemoveListener(OpenShop);
            }
        }
    }
}