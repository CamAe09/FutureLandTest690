using UnityEngine;
using System.Collections.Generic;

namespace TPSBR
{
    [System.Serializable]
    public class POILocation
    {
        public string name = "New Location";
        public Vector3 position;
        public Color color = Color.white;
        public bool isActive = true;
    }

    public class SimplePOIManager : MonoBehaviour
    {
        [Header("POI Locations")]
        [SerializeField] private List<POILocation> _predefinedPOIs = new List<POILocation>();

        [Header("Text Settings")]
        [SerializeField] private float _globalFontSize = 400f; // MASSIVE text

        [Header("Debug")]
        [SerializeField] private bool _debugMode = true;

        private List<POIMarker> _spawnedMarkers = new List<POIMarker>();

        public static SimplePOIManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            // No automatic POI creation - you'll add your own!
        }

        private void Start()
        {
            Debug.Log("🗺️ === POI MANAGER STARTING ===");
            Debug.Log($"Ready to spawn your custom POIs!");

            // Only spawn POIs if you've added any to the list in inspector
            if (_predefinedPOIs.Count > 0)
            {
                SpawnPredefinedPOIs();
            }
            else
            {
                Debug.Log("📝 No POIs defined. Add POIs in the inspector or use context menu options.");
            }

            Debug.Log($"🎯 POI Manager ready. Current markers: {_spawnedMarkers.Count}");
        }

        private void SpawnPredefinedPOIs()
        {
            Debug.Log($"🚀 Spawning {_predefinedPOIs.Count} custom POIs...");

            foreach (var poi in _predefinedPOIs)
            {
                if (poi.isActive)
                {
                    SpawnPOI(poi.position, poi.name, poi.color);
                }
            }

            Debug.Log($"🎯 Spawned {_spawnedMarkers.Count} custom POI markers");
        }

        public POIMarker SpawnPOI(Vector3 position, string poiName, Color color = default)
        {
            if (color == default) color = Color.white;

            GameObject poiObj = new GameObject($"POI - {poiName.ToUpper()}");
            poiObj.transform.position = position;
            poiObj.transform.parent = transform;

            var marker = poiObj.AddComponent<POIMarker>();
            marker.POIName = poiName;
            marker.POIColor = color;

            _spawnedMarkers.Add(marker);

            Debug.Log($"✅ POI CREATED: '{poiName.ToUpper()}' at {position}");

            return marker;
        }

        public void RemovePOI(string poiName)
        {
            for (int i = _spawnedMarkers.Count - 1; i >= 0; i--)
            {
                if (_spawnedMarkers[i].POIName.ToUpper() == poiName.ToUpper())
                {
                    Debug.Log($"Removing POI '{poiName.ToUpper()}'");

                    DestroyImmediate(_spawnedMarkers[i].gameObject);
                    _spawnedMarkers.RemoveAt(i);
                }
            }
        }

        // Helper methods for you to add POIs easily
        [ContextMenu("Add POI At Current Position")]
        public void AddPOIAtCurrentPosition()
        {
            Vector3 pos = transform.position;
            SpawnPOI(pos, "New POI", Color.white);
            Debug.Log($"📍 Added POI at POIManager position: {pos}");
        }

        [ContextMenu("Add POI At Camera Position")]
        public void AddPOIAtCameraPosition()
        {
            if (Camera.main != null)
            {
                Vector3 camPos = Camera.main.transform.position;
                camPos.y = 20f; // Ground level
                SpawnPOI(camPos, "Camera POI", Color.yellow);
                Debug.Log($"📷 Added POI at camera position: {camPos}");
            }
        }

        [ContextMenu("Clear All POIs")]
        public void ClearAllPOIs()
        {
            Debug.Log("🧹 Clearing all POIs...");

            for (int i = _spawnedMarkers.Count - 1; i >= 0; i--)
            {
                if (_spawnedMarkers[i] != null)
                {
                    DestroyImmediate(_spawnedMarkers[i].gameObject);
                }
            }

            _spawnedMarkers.Clear();
            Debug.Log("✅ All POIs cleared!");
        }

        // Text size adjustment methods
        [ContextMenu("Make All Text Size 300")]
        public void MakeAllTextSize300()
        {
            Debug.Log("🔤 Making all text size 300...");

            foreach (var marker in _spawnedMarkers)
            {
                if (marker != null)
                {
                    marker.MakeTextSize300();
                }
            }
        }

        [ContextMenu("Make All Text Size 500")]
        public void MakeAllTextSize500()
        {
            Debug.Log("🔤 Making all text MASSIVE size 500...");

            foreach (var marker in _spawnedMarkers)
            {
                if (marker != null)
                {
                    marker.MakeTextSize500();
                }
            }
        }

        [ContextMenu("Make All Text Size 700")]
        public void MakeAllTextSize700()
        {
            Debug.Log("🔤 Making all text ENORMOUS size 700...");

            foreach (var marker in _spawnedMarkers)
            {
                if (marker != null)
                {
                    marker.MakeTextSize700();
                }
            }
        }

        [ContextMenu("List All Current POIs")]
        public void ListAllCurrentPOIs()
        {
            Debug.Log("=== YOUR CURRENT POIs ===");
            Debug.Log($"Predefined POIs count: {_predefinedPOIs.Count}");
            Debug.Log($"Spawned markers count: {_spawnedMarkers.Count}");

            for (int i = 0; i < _spawnedMarkers.Count; i++)
            {
                var marker = _spawnedMarkers[i];
                if (marker != null)
                {
                    Debug.Log($"POI {i + 1}: {marker.POIName} at {marker.transform.position}");
                }
            }
        }

        private void OnValidate()
        {
            // Update POIs when you change values in inspector
            if (Application.isPlaying && _spawnedMarkers.Count > 0)
            {
                Debug.Log("🔄 Updating POI properties from inspector...");

                for (int i = 0; i < _spawnedMarkers.Count && i < _predefinedPOIs.Count; i++)
                {
                    var marker = _spawnedMarkers[i];
                    var poi = _predefinedPOIs[i];

                    if (marker != null)
                    {
                        marker.POIName = poi.name;
                        marker.POIColor = poi.color;
                        marker.transform.position = poi.position;
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            // Draw gizmos for POIs you've defined in inspector
            Gizmos.color = Color.cyan;
            if (_predefinedPOIs != null)
            {
                foreach (var poi in _predefinedPOIs)
                {
                    if (poi.isActive)
                    {
                        Gizmos.DrawWireSphere(poi.position, 10f);
                        Gizmos.DrawLine(poi.position, poi.position + Vector3.up * 30f);
                    }
                }
            }
        }
    }
}
