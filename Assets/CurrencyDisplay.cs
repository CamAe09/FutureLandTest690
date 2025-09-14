using UnityEngine;
using TMPro;

namespace TPSBR
{
    public class CurrencyDisplay : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _currencyText;
        [SerializeField] private string _currencyPrefix = "$";
        [SerializeField] private bool _showThousandsSeparator = true;
        
        private void Start()
        {
            if (CurrencyManager.Instance != null)
            {
                UpdateCurrencyDisplay(CurrencyManager.Instance.CurrentCoins);
            }
        }
        
        private void OnEnable()
        {
            CurrencyManager.OnCurrencyChanged += UpdateCurrencyDisplay;
        }
        
        private void OnDisable()
        {
            CurrencyManager.OnCurrencyChanged -= UpdateCurrencyDisplay;
        }
        
        private void UpdateCurrencyDisplay(int amount)
        {
            if (_currencyText != null)
            {
                if (_showThousandsSeparator)
                {
                    _currencyText.text = $"{_currencyPrefix}{amount:N0}";
                }
                else
                {
                    _currencyText.text = $"{_currencyPrefix}{amount}";
                }
            }
        }
    }
}