using UnityEngine;
using System;

namespace TPSBR
{
    public class CurrencyManager : MonoBehaviour
    {
        [Header("Currency Settings")]
        [SerializeField] private int _startingCoins = 1000;
        [SerializeField] private bool _debugMode = true;
        
        [Header("Quick Actions")]
        [Space]
        [SerializeField] private bool _giveCoins = false;
        [SerializeField] private int _coinsToGive = 5000;
        [Space]
        [SerializeField] private bool _resetToStartingCoins = false;
        [Space]
        [SerializeField] private bool _forceSetCoins = false;
        [SerializeField] private int _forceAmount = 10000;
        
        [Header("🔥 ADMIN CONTROLS 🔥")]
        [Space]
        [SerializeField] private bool _clearAllSaveData = false;

        private int _currentCoins;

        public static CurrencyManager Instance { get; private set; }

        // Events for UI updates
        public static event Action<int> OnCurrencyChanged;

        public int CurrentCoins
        {
            get => _currentCoins;
            private set
            {
                _currentCoins = value;
                OnCurrencyChanged?.Invoke(_currentCoins);

                if (_debugMode)
                {
                    Debug.Log($"💰 Currency changed: {_currentCoins} coins");
                }
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadCurrency();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (_debugMode)
            {
                Debug.Log($"💰 Currency Manager initialized with {CurrentCoins} coins");
            }
        }

        public bool CanAfford(int cost)
        {
            return CurrentCoins >= cost;
        }

        public bool SpendCoins(int amount)
        {
            if (CanAfford(amount))
            {
                CurrentCoins -= amount;
                SaveCurrency();

                if (_debugMode)
                {
                    Debug.Log($"💸 Spent {amount} coins. Remaining: {CurrentCoins}");
                }
                return true;
            }

            if (_debugMode)
            {
                Debug.Log($"❌ Cannot afford {amount} coins. Current: {CurrentCoins}");
            }
            return false;
        }

        public void AddCoins(int amount)
        {
            CurrentCoins += amount;
            SaveCurrency();

            if (_debugMode)
            {
                Debug.Log($"💎 Added {amount} coins. Total: {CurrentCoins}");
            }
        }

        private void Update()
        {
            // Check if the give coins checkbox was clicked
            if (_giveCoins)
            {
                _giveCoins = false; // Reset the checkbox
                AddCoins(_coinsToGive);
                Debug.Log($"💰 GAVE PLAYER {_coinsToGive} COINS! New total: {CurrentCoins}");
                Debug.Log($"💰 Currency Manager now has: {CurrentCoins} coins (after giving {_coinsToGive})");
            }
            
            // Check if reset to starting coins was clicked
            if (_resetToStartingCoins)
            {
                _resetToStartingCoins = false; // Reset the checkbox
                CurrentCoins = _startingCoins;
                SaveCurrency();
                Debug.Log($"🔄 RESET CURRENCY TO {_startingCoins} COINS!");
                Debug.Log($"💰 Currency Manager now has: {CurrentCoins} coins (after reset)");
            }
            
            // Check if force set coins was clicked
            if (_forceSetCoins)
            {
                _forceSetCoins = false; // Reset the checkbox
                PlayerPrefs.DeleteKey("PlayerCoins"); // Clear old save
                CurrentCoins = _forceAmount;
                SaveCurrency();
                Debug.Log($"🔥 FORCE SET CURRENCY TO {_forceAmount} COINS (cleared old save)!");
                Debug.Log($"💰 Currency Manager now has: {CurrentCoins} coins (force set)");
            }
            
            // Check if clear all save data was clicked (ADMIN)
            if (_clearAllSaveData)
            {
                _clearAllSaveData = false; // Reset the checkbox
                ClearAllSaveData();
            }
        }

        private void LoadCurrency()
        {
            _currentCoins = PlayerPrefs.GetInt("PlayerCoins", _startingCoins);
        }

        private void SaveCurrency()
        {
            PlayerPrefs.SetInt("PlayerCoins", _currentCoins);
            PlayerPrefs.Save();
        }

        // Debug methods
        [ContextMenu("Add 100 Coins")]
        public void Add100Coins()
        {
            AddCoins(100);
        }

        [ContextMenu("Add 1000 Coins")]
        public void Add1000Coins()
        {
            AddCoins(1000);
        }

        [ContextMenu("Reset Currency")]
        public void ResetCurrency()
        {
            CurrentCoins = _startingCoins;
            SaveCurrency();
            
            if (_debugMode)
            {
                Debug.Log($"🔄 Currency reset to {_startingCoins} coins");
            }
        }
        
        [ContextMenu("Add 10000 Coins")]
        public void Add10000Coins()
        {
            AddCoins(10000);
        }
        
        private void ClearAllSaveData()
        {
            Debug.Log("🔥 ADMIN: CLEARING ALL SAVE DATA!");
            
            // Clear currency data
            PlayerPrefs.DeleteKey("PlayerCoins");
            CurrentCoins = _startingCoins;
            SaveCurrency();
            Debug.Log($"💰 Reset currency to {_startingCoins} coins");
            
            // Clear inventory data
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.ClearInventory();
                Debug.Log("📦 Cleared player inventory");
            }
            
            // Clear any other game-specific save data (add keys as needed)
            PlayerPrefs.DeleteKey("PlayerLevel");
            PlayerPrefs.DeleteKey("PlayerExperience");
            PlayerPrefs.DeleteKey("PlayerName");
            PlayerPrefs.DeleteKey("LastSelectedAgent");
            PlayerPrefs.DeleteKey("GameProgress");
            PlayerPrefs.DeleteKey("UnlockedLevels");
            PlayerPrefs.DeleteKey("PlayerStats");
            PlayerPrefs.DeleteKey("PlayerSettings");
            
            // Save changes
            PlayerPrefs.Save();
            
            Debug.Log("🔥 ALL SAVE DATA CLEARED! Game reset to fresh state.");
            Debug.Log("ℹ️ You may need to restart the game for all changes to take effect.");
        }
    }
}