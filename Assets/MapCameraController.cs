using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TPSBR
{
    public class MapCameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Camera _mapCamera;
        [SerializeField] private float _cameraHeight = 200f;
        [SerializeField] private float _cameraSize = 100f;
        [SerializeField] private LayerMask _mapLayers = ~0;

        [Header("Map Controls")]
        [SerializeField] private Key _toggleMapKey = Key.M;
        [SerializeField] private bool _allowPanning = true;
        [SerializeField] private float _panSpeed = 50f;
        [SerializeField] private float _zoomSpeed = 10f;
        [SerializeField] private float _minZoom = 20f;
        [SerializeField] private float _maxZoom = 200f;

        [Header("Map UI")]
        [SerializeField] private Canvas _mapUI;
        [SerializeField] private GameObject _mapBackground;

        [Header("Fog Settings")]
        [SerializeField] private bool _disableFogForMap = true;

        [Header("Debug")]
        [SerializeField] private bool _debugMode = true;
        [SerializeField] private bool _autoFindTerrainCenter = true;

        private bool _mapActive = false;
        private Vector3 _originalCameraPosition;
        private Camera _mainCamera;
        private Vector2 _panInput;
        private float _zoomInput;
        private bool _originalFogState;

        public static MapCameraController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            _mapLayers = ~LayerMask.GetMask("UI");
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            _originalCameraPosition = transform.position;

            // Store original fog state
            _originalFogState = RenderSettings.fog;

            if (_autoFindTerrainCenter)
            {
                FindAndCenterOverTerrain();
            }

            SetupMapCamera();
            SetMapActive(false);

            if (_debugMode)
            {
                Debug.Log($"Map Camera Controller initialized. Original fog state: {_originalFogState}");
                Debug.Log($"Fog start distance: {RenderSettings.fogStartDistance}, End distance: {RenderSettings.fogEndDistance}");
            }
        }

        private void FindAndCenterOverTerrain()
        {
            Terrain terrain = FindObjectOfType<Terrain>();
            if (terrain != null)
            {
                Vector3 terrainCenter = terrain.transform.position + terrain.terrainData.size * 0.5f;
                terrainCenter.y = transform.position.y;
                transform.position = terrainCenter;

                if (_debugMode)
                {
                    Debug.Log($"Auto-centered map camera over terrain at: {terrainCenter}");
                    Debug.Log($"Terrain size: {terrain.terrainData.size}");
                }
            }
            else
            {
                GameObject[] allObjects = FindObjectsOfType<GameObject>();
                if (allObjects.Length > 0)
                {
                    Vector3 center = Vector3.zero;
                    int count = 0;

                    foreach (GameObject obj in allObjects)
                    {
                        if (obj.transform.parent == null &&
                            !obj.name.Contains("Camera") &&
                            !obj.name.Contains("Canvas") &&
                            !obj.name.Contains("EventSystem"))
                        {
                            center += obj.transform.position;
                            count++;
                        }
                    }

                    if (count > 0)
                    {
                        center /= count;
                        center.y = transform.position.y;
                        transform.position = center;

                        if (_debugMode)
                        {
                            Debug.Log($"Auto-centered map camera over scene center at: {center}");
                        }
                    }
                }
            }
        }

        private void Update()
        {
            HandleInput();

            if (_mapActive && _allowPanning)
            {
                HandleMapControls();
            }
        }

        private void SetupMapCamera()
        {
            if (_mapCamera == null)
            {
                GameObject camObj = new GameObject("Map Camera");
                camObj.transform.parent = transform;
                _mapCamera = camObj.AddComponent<Camera>();

                if (_debugMode)
                {
                    Debug.Log("Created new map camera");
                }
            }

            // Configure camera for top-down map view
            _mapCamera.orthographic = true;
            _mapCamera.orthographicSize = _cameraSize;
            _mapCamera.cullingMask = _mapLayers;
            _mapCamera.clearFlags = CameraClearFlags.Skybox;
            _mapCamera.depth = 10;

            // Set far clipping plane to be much further to avoid fog cutoff
            _mapCamera.farClipPlane = 2000f;

            Vector3 cameraPos = transform.position + Vector3.up * _cameraHeight;
            _mapCamera.transform.position = cameraPos;
            _mapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            if (_debugMode)
            {
                Debug.Log($"Map camera positioned at: {cameraPos}");
                Debug.Log($"Far clip plane set to: {_mapCamera.farClipPlane}");
            }

            if (_mapUI == null)
            {
                SetupMapUI();
            }
        }

        private void SetupMapUI()
        {
            GameObject uiObj = new GameObject("Map UI");
            uiObj.transform.parent = transform;

            _mapUI = uiObj.AddComponent<Canvas>();
            _mapUI.renderMode = RenderMode.ScreenSpaceOverlay;
            _mapUI.sortingOrder = 100;

            var scaler = uiObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            uiObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            CreateMapBackground();
        }

        private void CreateMapBackground()
        {
            if (_mapBackground == null)
            {
                GameObject bgObj = new GameObject("Map Background");
                bgObj.transform.parent = _mapUI.transform;

                var rectTransform = bgObj.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                var image = bgObj.AddComponent<UnityEngine.UI.Image>();
                image.color = new Color(0f, 0f, 0f, 0.3f);

                _mapBackground = bgObj;
            }
        }

        private void HandleInput()
        {
            if (Keyboard.current[_toggleMapKey].wasPressedThisFrame)
            {
                ToggleMap();
            }

            if (_mapActive && _allowPanning)
            {
                _panInput = Vector2.zero;

                if (Keyboard.current.wKey.isPressed) _panInput.y += 1f;
                if (Keyboard.current.sKey.isPressed) _panInput.y -= 1f;
                if (Keyboard.current.aKey.isPressed) _panInput.x -= 1f;
                if (Keyboard.current.dKey.isPressed) _panInput.x += 1f;

                _zoomInput = 0f;
                if (Mouse.current.scroll.ReadValue().y != 0f)
                {
                    _zoomInput = Mouse.current.scroll.ReadValue().y;
                }
            }
        }

        private void HandleMapControls()
        {
            if (_panInput.magnitude > 0f)
            {
                Vector3 moveDir = new Vector3(_panInput.x, 0f, _panInput.y);
                _mapCamera.transform.position += moveDir * _panSpeed * Time.deltaTime;
            }

            if (Mathf.Abs(_zoomInput) > 0f)
            {
                _mapCamera.orthographicSize -= _zoomInput * _zoomSpeed;
                _mapCamera.orthographicSize = Mathf.Clamp(_mapCamera.orthographicSize, _minZoom, _maxZoom);
            }
        }

        public void ToggleMap()
        {
            SetMapActive(!_mapActive);
        }

        public void SetMapActive(bool active)
        {
            _mapActive = active;

            _mapCamera.gameObject.SetActive(active);
            _mapUI.gameObject.SetActive(active);

            if (active)
            {
                // Disable fog for map view
                if (_disableFogForMap)
                {
                    RenderSettings.fog = false;
                    if (_debugMode)
                    {
                        Debug.Log("Fog disabled for map view");
                    }
                }

                _mapCamera.rect = new Rect(0, 0, 1, 1);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (_debugMode)
                {
                    Debug.Log($"MAP OPENED - Camera at: {_mapCamera.transform.position}");
                    Debug.Log($"Fog state: {RenderSettings.fog}");
                }
            }
            else
            {
                // Restore original fog state
                if (_disableFogForMap)
                {
                    RenderSettings.fog = _originalFogState;
                    if (_debugMode)
                    {
                        Debug.Log($"Fog restored to original state: {_originalFogState}");
                    }
                }

                _mapCamera.rect = new Rect(0, 0, 0, 0);

                if (_debugMode)
                {
                    Debug.Log("MAP CLOSED");
                }
            }
        }

        // Rest of your methods stay the same...
        public void AddPOIAtPosition(Vector3 worldPosition, string poiName, Color poiColor = default)
        {
            if (poiColor == default) poiColor = Color.white;

            GameObject poiObj = new GameObject($"POI - {poiName}");
            poiObj.transform.position = worldPosition;
            poiObj.transform.parent = transform;

            var poiMarker = poiObj.AddComponent<POIMarker>();
            poiMarker.POIName = poiName;
            poiMarker.POIColor = poiColor;

            Debug.Log($"Added POI '{poiName}' at {worldPosition}");
        }

        public void CenterMapOnPosition(Vector3 worldPosition)
        {
            _mapCamera.transform.position = new Vector3(worldPosition.x, _cameraHeight, worldPosition.z);
        }

        [ContextMenu("Test Fog Settings")]
        public void TestFogSettings()
        {
            Debug.Log("=== FOG DEBUG INFO ===");
            Debug.Log($"Fog Enabled: {RenderSettings.fog}");
            Debug.Log($"Fog Mode: {RenderSettings.fogMode}");
            Debug.Log($"Fog Color: {RenderSettings.fogColor}");
            Debug.Log($"Fog Start Distance: {RenderSettings.fogStartDistance}");
            Debug.Log($"Fog End Distance: {RenderSettings.fogEndDistance}");
            Debug.Log($"Fog Density: {RenderSettings.fogDensity}");
            Debug.Log($"Map Camera Height: {_cameraHeight}");
        }

        private void OnDrawGizmos()
        {
            if (_debugMode && _mapCamera != null)
            {
                // Draw camera view bounds
                Gizmos.color = Color.yellow;
                Vector3 camPos = _mapCamera.transform.position;
                float size = _mapCamera.orthographicSize;

                Gizmos.DrawWireCube(new Vector3(camPos.x, 0, camPos.z), new Vector3(size * 2, 0, size * 2));

                // Draw line from camera to ground
                Gizmos.color = Color.red;
                Gizmos.DrawLine(camPos, new Vector3(camPos.x, 0, camPos.z));

                // Draw fog range if fog is enabled
                if (RenderSettings.fog)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(Vector3.zero, RenderSettings.fogStartDistance);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(Vector3.zero, RenderSettings.fogEndDistance);
                }
            }
        }
    }
}
