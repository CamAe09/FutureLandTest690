using UnityEngine;
using System;

namespace TPSBR
{
    [System.Serializable]
    public class QuestProgress
    {
        public string questId;
        public int currentProgress;
        public bool isCompleted;
        public bool isRewardClaimed;
        public DateTime startTime;
        public DateTime? completionTime;
        
        public QuestProgress(string id)
        {
            questId = id;
            currentProgress = 0;
            isCompleted = false;
            isRewardClaimed = false;
            startTime = DateTime.Now;
            completionTime = null;
        }
        
        public float GetProgressPercentage(int targetAmount)
        {
            if (targetAmount <= 0) return 1f;
            return Mathf.Clamp01((float)currentProgress / targetAmount);
        }
        
        public void AddProgress(int amount = 1)
        {
            if (isCompleted) return;
            
            currentProgress += amount;
        }
        
        public void CompleteQuest()
        {
            if (!isCompleted)
            {
                isCompleted = true;
                completionTime = DateTime.Now;
            }
        }
        
        public void ClaimReward()
        {
            if (isCompleted)
            {
                isRewardClaimed = true;
            }
        }
        
        public bool IsExpired(float timeLimitHours)
        {
            if (timeLimitHours <= 0) return false;
            
            TimeSpan timeSinceStart = DateTime.Now - startTime;
            return timeSinceStart.TotalHours > timeLimitHours;
        }
        
        public string GetTimeRemaining(float timeLimitHours)
        {
            if (timeLimitHours <= 0) return "No time limit";
            
            TimeSpan timeSinceStart = DateTime.Now - startTime;
            TimeSpan timeRemaining = TimeSpan.FromHours(timeLimitHours) - timeSinceStart;
            
            if (timeRemaining.TotalSeconds <= 0)
                return "Expired";
            
            if (timeRemaining.TotalDays >= 1)
                return $"{timeRemaining.Days}d {timeRemaining.Hours}h";
            else if (timeRemaining.TotalHours >= 1)
                return $"{timeRemaining.Hours}h {timeRemaining.Minutes}m";
            else
                return $"{timeRemaining.Minutes}m";
        }
    }
}