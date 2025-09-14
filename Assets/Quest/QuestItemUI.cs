using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TPSBR
{
    public class QuestItemUI : MonoBehaviour
    {
        [Header("UI References")]
        private Image questIcon;
        private TextMeshProUGUI questName;
        private TextMeshProUGUI questDescription;
        private TextMeshProUGUI questType;
        private TextMeshProUGUI progressText;
        private Image progressBarFill;
        private TextMeshProUGUI coinAmount;
        private Button claimButton;
        private TextMeshProUGUI claimButtonText;
        
        private QuestData currentQuestData;
        private QuestProgress currentQuestProgress;
        
        [Header("Settings")]
        [SerializeField] private bool debugMode = true;
        
        public void Initialize(QuestData questData, QuestProgress questProgress)
        {
            currentQuestData = questData;
            currentQuestProgress = questProgress;
            
            FindUIComponents();
            SetupUI();
            UpdateProgress(questProgress);
            
            if (debugMode)
            {
                Debug.Log($"✅ Quest item UI initialized for: {questData.questName}");
            }
        }
        
        private void FindUIComponents()
        {
            questIcon = transform.Find("Quest Icon")?.GetComponent<Image>();
            
            Transform questInfo = transform.Find("Quest Info");
            if (questInfo != null)
            {
                questName = questInfo.Find("Quest Name")?.GetComponent<TextMeshProUGUI>();
                questDescription = questInfo.Find("Quest Description")?.GetComponent<TextMeshProUGUI>();
                questType = questInfo.Find("Quest Type")?.GetComponent<TextMeshProUGUI>();
            }
            
            Transform progressArea = transform.Find("Progress Area");
            if (progressArea != null)
            {
                progressText = progressArea.Find("Progress Text")?.GetComponent<TextMeshProUGUI>();
                
                Transform progressBarBg = progressArea.Find("Progress Bar Background");
                if (progressBarBg != null)
                {
                    progressBarFill = progressBarBg.Find("Progress Bar Fill")?.GetComponent<Image>();
                }
            }
            
            Transform rewardArea = transform.Find("Reward Area");
            if (rewardArea != null)
            {
                Transform coinReward = rewardArea.Find("Coin Reward");
                if (coinReward != null)
                {
                    coinAmount = coinReward.Find("Coin Amount")?.GetComponent<TextMeshProUGUI>();
                }
            }
            
            claimButton = transform.Find("Claim Button")?.GetComponent<Button>();
            if (claimButton != null)
            {
                claimButtonText = claimButton.transform.Find("Button Text")?.GetComponent<TextMeshProUGUI>();
                claimButton.onClick.RemoveAllListeners();
                claimButton.onClick.AddListener(OnClaimButtonClicked);
            }
            
            if (debugMode && (questName == null || progressText == null))
            {
                Debug.LogWarning($"⚠️ Some UI components not found for quest: {currentQuestData?.questName}");
            }
        }
        
        private void SetupUI()
        {
            if (currentQuestData == null) return;
            
            if (questIcon != null && currentQuestData.questIcon != null)
            {
                questIcon.sprite = currentQuestData.questIcon;
            }
            
            if (questName != null)
            {
                questName.text = currentQuestData.questName;
            }
            
            if (questDescription != null)
            {
                questDescription.text = currentQuestData.description;
            }
            
            if (questType != null)
            {
                questType.text = $"{currentQuestData.questType} Quest";
                
                if (ColorUtility.TryParseHtmlString(currentQuestData.GetQuestTypeColor(), out Color typeColor))
                {
                    questType.color = typeColor;
                }
            }
            
            if (coinAmount != null)
            {
                coinAmount.text = currentQuestData.coinReward.ToString();
            }
            
            Image bgImage = GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = GetQuestBackgroundColor();
            }
        }
        
        public void UpdateProgress(QuestProgress questProgress)
        {
            if (questProgress == null || currentQuestData == null) return;
            
            currentQuestProgress = questProgress;
            
            if (progressText != null)
            {
                progressText.text = $"{questProgress.currentProgress}/{currentQuestData.targetAmount}";
            }
            
            if (progressBarFill != null)
            {
                float progressPercentage = questProgress.GetProgressPercentage(currentQuestData.targetAmount);
                progressBarFill.fillAmount = progressPercentage;
                
                progressBarFill.color = questProgress.isCompleted ? Color.green : 
                                       progressPercentage > 0.5f ? Color.yellow : Color.red;
            }
            
            UpdateClaimButton();
            
            if (currentQuestData.hasTimeLimit && !questProgress.isCompleted)
            {
                UpdateTimeDisplay();
            }
        }
        
        private void UpdateClaimButton()
        {
            if (claimButton == null || claimButtonText == null) return;
            
            if (currentQuestProgress.isCompleted && !currentQuestProgress.isRewardClaimed)
            {
                claimButton.interactable = true;
                claimButtonText.text = "CLAIM";
                claimButton.GetComponent<Image>().color = Color.green;
            }
            else if (currentQuestProgress.isRewardClaimed)
            {
                claimButton.interactable = false;
                claimButtonText.text = "CLAIMED";
                claimButton.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                claimButton.interactable = false;
                claimButtonText.text = $"{currentQuestProgress.currentProgress}/{currentQuestData.targetAmount}";
                claimButton.GetComponent<Image>().color = Color.gray;
            }
        }
        
        private void UpdateTimeDisplay()
        {
            if (progressText != null && currentQuestData.hasTimeLimit)
            {
                string timeRemaining = currentQuestProgress.GetTimeRemaining(currentQuestData.timeLimitHours);
                progressText.text += $"\n⏰ {timeRemaining}";
            }
        }
        
        private Color GetQuestBackgroundColor()
        {
            Color baseColor = new Color(0.15f, 0.15f, 0.15f, 0.95f);
            
            if (currentQuestProgress.isCompleted)
            {
                return Color.Lerp(baseColor, Color.green, 0.2f);
            }
            
            float progress = currentQuestProgress.GetProgressPercentage(currentQuestData.targetAmount);
            if (progress > 0)
            {
                return Color.Lerp(baseColor, Color.yellow, 0.1f);
            }
            
            return baseColor;
        }
        
        private void OnClaimButtonClicked()
        {
            if (currentQuestProgress.isCompleted && !currentQuestProgress.isRewardClaimed)
            {
                if (QuestManager.Instance != null)
                {
                    bool claimed = QuestManager.Instance.ClaimQuestReward(currentQuestData.name);
                    if (claimed)
                    {
                        UpdateClaimButton();
                        
                        ShowRewardFeedback();
                        
                        if (debugMode)
                        {
                            Debug.Log($"💰 Claimed reward for quest: {currentQuestData.questName} (+{currentQuestData.coinReward} coins)");
                        }
                    }
                }
            }
        }
        
        private void ShowRewardFeedback()
        {
            if (claimButtonText != null)
            {
                string originalText = claimButtonText.text;
                claimButtonText.text = $"+{currentQuestData.coinReward}";
                claimButtonText.color = Color.yellow;
                
                Invoke(nameof(ResetClaimButtonText), 1f);
            }
        }
        
        private void ResetClaimButtonText()
        {
            if (claimButtonText != null)
            {
                claimButtonText.text = "CLAIMED";
                claimButtonText.color = Color.white;
            }
        }
        
        private void Update()
        {
            if (currentQuestData != null && currentQuestData.hasTimeLimit && 
                currentQuestProgress != null && !currentQuestProgress.isCompleted)
            {
                if (currentQuestProgress.IsExpired(currentQuestData.timeLimitHours))
                {
                    HandleExpiredQuest();
                }
            }
        }
        
        private void HandleExpiredQuest()
        {
            Image bgImage = GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = Color.Lerp(bgImage.color, Color.red, 0.3f);
            }
            
            if (progressText != null)
            {
                progressText.color = Color.red;
                progressText.text += "\n❌ EXPIRED";
            }
            
            if (claimButton != null)
            {
                claimButton.interactable = false;
            }
        }
    }
}