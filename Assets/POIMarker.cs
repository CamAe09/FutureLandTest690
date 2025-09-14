using UnityEngine;
using TMPro;

namespace TPSBR
{
    public class POIMarker : MonoBehaviour
    {
        [Header("POI Settings")]
        [SerializeField] private string _poiName = "Location";
        [SerializeField] private Color _poiColor = Color.white;

        [Header("Text Settings - MASSIVE SIZE")]
        [SerializeField] private float _fontSize = 400f; // HUGE text size!
        [SerializeField] private float _textHeight = 25f;

        [SerializeField] private TextMeshPro _nameLabel;
        [SerializeField] private GameObject _debugMarker;

        private bool _isMapActive = false;

        public string POIName
        {
            get => _poiName;
            set
            {
                _poiName = value;
                UpdateLabel();
            }
        }

        public Color POIColor
        {
            get => _poiColor;
            set
            {
                _poiColor = value;
                UpdateTextColor();
            }
        }

        private void Start()
        {
            SetupPOIMarker();
            SetPOIVisible(false);
        }

        private void Update()
        {
            bool mapActive = IsMapActive();
            if (mapActive != _isMapActive)
            {
                _isMapActive = mapActive;
                SetPOIVisible(_isMapActive);
            }
        }

        private bool IsMapActive()
        {
            var mapController = MapCameraController.Instance;
            if (mapController != null)
            {
                var mapCamera = mapController.GetComponentInChildren<Camera>();
                return mapCamera != null && mapCamera.gameObject.activeInHierarchy;
            }
            return false;
        }

        private void SetPOIVisible(bool visible)
        {
            if (_nameLabel != null)
            {
                _nameLabel.gameObject.SetActive(visible);
            }

            if (_debugMarker != null)
            {
                _debugMarker.SetActive(visible);
            }
        }

        private void SetupPOIMarker()
        {
            Debug.Log($"🏗️ Setting up MASSIVE POI Marker (no outline): {_poiName} at {transform.position}");

            CreateDebugMarker();

            if (_nameLabel == null)
            {
                _nameLabel = CreateMassiveText();
            }

            Vector3 textPos = transform.position + Vector3.up * _textHeight;
            _nameLabel.transform.position = textPos;

            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));

            Debug.Log($"✅ MASSIVE POI '{_poiName}' setup complete with clean text (no outline)!");
        }

        private void CreateDebugMarker()
        {
            _debugMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _debugMarker.transform.parent = transform;
            _debugMarker.transform.localPosition = Vector3.up * 10f;
            _debugMarker.transform.localScale = new Vector3(20f, 8f, 20f);
            _debugMarker.name = "Debug Marker";

            var renderer = _debugMarker.GetComponent<Renderer>();
            renderer.material.color = Color.red;

            if (_debugMarker.GetComponent<Collider>() != null)
                DestroyImmediate(_debugMarker.GetComponent<Collider>());
        }

        private TextMeshPro CreateMassiveText()
        {
            GameObject labelObj = new GameObject("MASSIVE POI Text - No Outline");
            labelObj.transform.parent = transform;

            TextMeshPro label = labelObj.AddComponent<TextMeshPro>();

            // Text content
            label.text = _poiName.ToUpper();

            // MASSIVE font settings - clean, no outline
            label.fontSize = _fontSize;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;

            // Face upward for top-down camera
            labelObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            // Bold styling
            label.fontStyle = FontStyles.Bold;
            label.enableAutoSizing = false;

            // NO OUTLINE - removed all outline properties
            label.outlineWidth = 0f; // No outline

            // HUGE text bounds to accommodate massive text
            label.rectTransform.sizeDelta = new Vector2(800f, 200f);

            // Ensure good material for visibility
            if (label.fontMaterial != null)
            {
                label.fontMaterial.shader = Shader.Find("TextMeshPro/Distance Field");
            }

            Debug.Log($"   - Text: '{label.text}'");
            Debug.Log($"   - MASSIVE Font size: {label.fontSize}");
            Debug.Log($"   - Clean text with NO outline");

            return label;
        }

        private void UpdateLabel()
        {
            if (_nameLabel != null)
            {
                _nameLabel.text = _poiName.ToUpper();
            }
        }

        private void UpdateTextColor()
        {
            if (_nameLabel != null)
            {
                _nameLabel.color = Color.white;
            }
        }

        private void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        [ContextMenu("Make Text Size 300")]
        public void MakeTextSize300()
        {
            _fontSize = 300f;
            if (_nameLabel != null)
            {
                _nameLabel.fontSize = _fontSize;
                _nameLabel.rectTransform.sizeDelta = new Vector2(600f, 150f);
            }
            Debug.Log($"🔤 Set clean text size to 300!");
        }

        [ContextMenu("Make Text Size 500")]
        public void MakeTextSize500()
        {
            _fontSize = 500f;
            if (_nameLabel != null)
            {
                _nameLabel.fontSize = _fontSize;
                _nameLabel.rectTransform.sizeDelta = new Vector2(1000f, 250f);
            }
            Debug.Log($"🔤 Set clean text size to MASSIVE 500!");
        }

        [ContextMenu("Make Text Size 700")]
        public void MakeTextSize700()
        {
            _fontSize = 700f;
            if (_nameLabel != null)
            {
                _nameLabel.fontSize = _fontSize;
                _nameLabel.rectTransform.sizeDelta = new Vector2(1400f, 350f);
            }
            Debug.Log($"🔤 Set clean text size to ENORMOUS 700!");
        }

        [ContextMenu("Force Show")]
        public void ForceShow()
        {
            SetPOIVisible(true);
        }

        [ContextMenu("Test Visibility")]
        public void TestVisibility()
        {
            Debug.Log("=== MASSIVE CLEAN POI VISIBILITY TEST ===");
            Debug.Log($"POI Name: {_poiName}");
            Debug.Log($"MASSIVE Font Size: {_fontSize}");
            Debug.Log($"Clean text (no outline): YES");
            Debug.Log($"Map Active: {_isMapActive}");
            Debug.Log($"Text GameObject Active: {_nameLabel?.gameObject.activeInHierarchy}");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 15f);

            Gizmos.color = Color.yellow;
            Vector3 textPos = transform.position + Vector3.up * _textHeight;
            Gizmos.DrawWireSphere(textPos, 10f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, textPos);
        }
    }
}
