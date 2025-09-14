using UnityEngine;
using System.Collections.Generic;

namespace TPSBR
{
    public class BattleRoyaleQuestTracker : MonoBehaviour
    {
        [Header("Quest Tracking Settings")]
        [SerializeField] private bool debugMode = true;
        [SerializeField] private bool enableQuestTracking = true;
        
        [Header("Match State")]
        [SerializeField] private bool isInMatch = false;
        [SerializeField] private float matchStartTime;
        [SerializeField] private Vector3 lastPosition;
        [SerializeField] private float totalDistanceTraveled = 0f;
        [SerializeField] private int currentStormCircle = 0;
        [SerializeField] private bool hasTakenStormDamage = false;
        
        [Header("Combat Tracking")]
        [SerializeField] private int matchEliminations = 0;
        [SerializeField] private float totalDamageDealt = 0f;
        [SerializeField] private HashSet<string> weaponTypesUsed = new HashSet<string>();
        [SerializeField] private HashSet<string> locationsVisited = new HashSet<string>();
        [SerializeField] private int buildingsLooted = 0;
        [SerializeField] private int healingItemsUsed = 0;
        
        public static BattleRoyaleQuestTracker Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            if (debugMode)
            {
                Debug.Log("🎯 Battle Royale Quest Tracker initialized");
            }
        }
        
        private void Update()
        {
            if (isInMatch && enableQuestTracking)
            {
                TrackMovement();
            }
        }
        
        public void StartMatch()
        {
            if (!enableQuestTracking) return;
            
            isInMatch = true;
            matchStartTime = Time.time;
            lastPosition = GetPlayerPosition();
            totalDistanceTraveled = 0f;
            currentStormCircle = 0;
            matchEliminations = 0;
            totalDamageDealt = 0f;
            weaponTypesUsed.Clear();
            locationsVisited.Clear();
            buildingsLooted = 0;
            healingItemsUsed = 0;
            hasTakenStormDamage = false;
            
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.PlayMatches);
            }
            
            if (debugMode)
            {
                Debug.Log("🎮 Match started - quest tracking enabled");
            }
        }
        
        public void EndMatch(int finalPlacement, int totalPlayers, bool isVictoryRoyale)
        {
            if (!enableQuestTracking || !isInMatch) return;
            
            isInMatch = false;
            
            if (QuestManager.Instance != null)
            {
                float survivalTime = Time.time - matchStartTime;
                
                if (survivalTime >= 300f)
                {
                    QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.SurviveTime);
                }
                
                if (IsTopPercentage(finalPlacement, totalPlayers, 0.5f))
                {
                    QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.FinishTopPercent, 1, finalPlacement);
                }
                
                if (finalPlacement <= 10)
                {
                    QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.FinishTopTen);
                }
                
                if (IsTopPercentage(finalPlacement, totalPlayers, 0.1f))
                {
                    QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.SurviveFinalCircle);
                }
                
                if (isVictoryRoyale)
                {
                    QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.WinMatches);
                    
                    if (!hasTakenStormDamage)
                    {
                        QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.WinWithoutStormDamage);
                    }
                }
                
                if (totalDistanceTraveled >= 1000f)
                {
                    QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.TravelDistance);
                }
                
                QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.TotalEliminations, matchEliminations);
                
                foreach (string weaponType in weaponTypesUsed)
                {
                    QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.UseWeaponTypes, 1, weaponType);
                }
                
                foreach (string location in locationsVisited)
                {
                    QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.LandInLocations, 1, location);
                }
            }
            
            if (debugMode)
            {
                Debug.Log($"🏁 Match ended - Placement: {finalPlacement}/{totalPlayers}, Victory: {isVictoryRoyale}");
                Debug.Log($"📊 Stats - Eliminations: {matchEliminations}, Distance: {totalDistanceTraveled:F1}m, Damage: {totalDamageDealt:F1}");
            }
        }
        
        public void OnEliminationScored(string weaponType = "", bool isHeadshot = false, float distance = 0f)
        {
            if (!enableQuestTracking || !isInMatch) return;
            
            matchEliminations++;
            
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.GetEliminations);
                
                if (matchEliminations == 1)
                {
                    QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.GetEliminations);
                }
                
                if (isHeadshot)
                {
                    QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.HeadshotEliminations, 1, true);
                }
                
                if (distance <= 10f)
                {
                    QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.CloseRangeEliminations, 1, distance);
                }
                
                if (!string.IsNullOrEmpty(weaponType))
                {
                    weaponTypesUsed.Add(weaponType);
                }
            }
            
            if (debugMode)
            {
                Debug.Log($"💀 Elimination scored - Weapon: {weaponType}, Headshot: {isHeadshot}, Distance: {distance:F1}m");
            }
        }
        
        public void OnDamageDealt(float damage)
        {
            if (!enableQuestTracking || !isInMatch) return;
            
            totalDamageDealt += damage;
            
            if (QuestManager.Instance != null && totalDamageDealt >= 200f)
            {
                QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.DealDamage);
            }
            
            if (debugMode && damage > 0)
            {
                Debug.Log($"🎯 Damage dealt: {damage:F1} (Total: {totalDamageDealt:F1})");
            }
        }
        
        public void OnBuildingLooted()
        {
            if (!enableQuestTracking || !isInMatch) return;
            
            buildingsLooted++;
            
            if (QuestManager.Instance != null && buildingsLooted >= 3)
            {
                QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.LootBuildings);
            }
            
            if (debugMode)
            {
                Debug.Log($"🏠 Building looted ({buildingsLooted} total)");
            }
        }
        
        public void OnLocationEntered(string locationName, bool isHighRisk = false)
        {
            if (!enableQuestTracking || !isInMatch || string.IsNullOrEmpty(locationName)) return;
            
            if (!locationsVisited.Contains(locationName))
            {
                locationsVisited.Add(locationName);
                
                if (QuestManager.Instance != null)
                {
                    if (isHighRisk)
                    {
                        QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.LandInHighRiskAreas, 1, locationName);
                    }
                }
                
                if (debugMode)
                {
                    Debug.Log($"📍 Entered location: {locationName} (High Risk: {isHighRisk})");
                }
            }
        }
        
        public void OnStormCircleUpdate(int circleNumber)
        {
            if (!enableQuestTracking || !isInMatch) return;
            
            currentStormCircle = circleNumber;
            
            if (QuestManager.Instance != null && currentStormCircle >= 3)
            {
                QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.SurviveStormCircles);
            }
            
            if (debugMode)
            {
                Debug.Log($"🌪️ Storm circle {circleNumber}");
            }
        }
        
        public void OnStormDamageTaken(float damage)
        {
            if (!enableQuestTracking || !isInMatch) return;
            
            if (!hasTakenStormDamage)
            {
                hasTakenStormDamage = true;
                
                if (QuestManager.Instance != null)
                {
                    QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.TakeStormDamage);
                }
                
                if (debugMode)
                {
                    Debug.Log($"⚡ Storm damage taken: {damage:F1}");
                }
            }
        }
        
        public void OnHealingItemUsed(string itemType = "")
        {
            if (!enableQuestTracking || !isInMatch) return;
            
            healingItemsUsed++;
            
            if (QuestManager.Instance != null && healingItemsUsed >= 10)
            {
                QuestManager.Instance.UpdateQuestProgress(QuestObjectiveType.UseHealingItems);
            }
            
            if (debugMode)
            {
                Debug.Log($"💊 Healing item used: {itemType} (Total: {healingItemsUsed})");
            }
        }
        
        private void TrackMovement()
        {
            Vector3 currentPosition = GetPlayerPosition();
            
            if (lastPosition != Vector3.zero)
            {
                float distance = Vector3.Distance(lastPosition, currentPosition);
                if (distance > 0.1f && distance < 50f)
                {
                    totalDistanceTraveled += distance;
                }
            }
            
            lastPosition = currentPosition;
        }
        
        private Vector3 GetPlayerPosition()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                return player.transform.position;
            }
            
            return Vector3.zero;
        }
        
        private bool IsTopPercentage(int placement, int totalPlayers, float percentage)
        {
            if (totalPlayers <= 0) return false;
            float playerPercentage = (float)placement / totalPlayers;
            return playerPercentage <= percentage;
        }
        
        [ContextMenu("Test Start Match")]
        public void TestStartMatch()
        {
            StartMatch();
        }
        
        [ContextMenu("Test End Match (Victory)")]
        public void TestEndMatchVictory()
        {
            EndMatch(1, 100, true);
        }
        
        [ContextMenu("Test Elimination")]
        public void TestElimination()
        {
            OnEliminationScored("Assault Rifle", true, 15f);
        }
        
        [ContextMenu("Test Quest Progress")]
        public void TestQuestProgress()
        {
            OnBuildingLooted();
            OnBuildingLooted();
            OnBuildingLooted();
            OnDamageDealt(250f);
            OnLocationEntered("Military Base", true);
            OnStormCircleUpdate(3);
        }
    }
}