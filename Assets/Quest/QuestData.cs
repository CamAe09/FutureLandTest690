using UnityEngine;

namespace TPSBR
{
    [System.Serializable]
    public enum QuestType
    {
        Daily,
        Combat,
        Weekly,
        Progression,
        Special
    }

    [System.Serializable]
    public enum QuestDifficulty
    {
        Easy,
        Medium,
        Hard,
        Expert
    }

    [System.Serializable]
    public enum QuestObjectiveType
    {
        PlayMatches,
        SurviveTime,
        LootBuildings,
        FinishTopPercent,
        TravelDistance,
        SurviveStormCircles,
        GetEliminations,
        DealDamage,
        CloseRangeEliminations,
        HeadshotEliminations,
        WinMatches,
        FinishTopTen,
        TotalEliminations,
        SurviveFinalCircle,
        TakeStormDamage,
        UseWeaponTypes,
        LandInLocations,
        WinWithoutStormDamage,
        LandInHighRiskAreas,
        UseHealingItems
    }

    [CreateAssetMenu(fileName = "New Quest", menuName = "TPSBR/Quest")]
    public class QuestData : ScriptableObject
    {
        [Header("Quest Information")]
        public string questName;
        [TextArea(3, 5)]
        public string description;
        public Sprite questIcon;
        
        [Header("Quest Settings")]
        public QuestType questType;
        public QuestDifficulty difficulty;
        public QuestObjectiveType objectiveType;
        
        [Header("Objective")]
        public int targetAmount = 1;
        public string objectiveDescription;
        
        [Header("Rewards")]
        public int coinReward = 25;
        
        [Header("Timing")]
        public bool hasTimeLimit = false;
        public float timeLimitHours = 24f;
        
        [Header("Prerequisites")]
        public QuestData[] prerequisiteQuests;
        public int minimumLevel = 0;
        
        public string GetQuestTypeColor()
        {
            return questType switch
            {
                QuestType.Daily => "#4CAF50",
                QuestType.Combat => "#FF5722", 
                QuestType.Weekly => "#2196F3",
                QuestType.Progression => "#9C27B0",
                QuestType.Special => "#FFD700",
                _ => "#FFFFFF"
            };
        }
        
        public string GetFormattedReward()
        {
            return $"{coinReward} coins";
        }
    }
}