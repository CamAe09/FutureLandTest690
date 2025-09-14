using UnityEngine;
using System.Collections;

public class TimeOfDayController : MonoBehaviour
{
    [System.Serializable]
    public class TimeOfDaySettings
    {
        [Header("Time Period")]
        public string timeName;

        [Header("Skybox")]
        public Material skyboxMaterial;

        [Header("Fog Settings")]
        public bool enableFog = true;
        public FogMode fogMode = FogMode.ExponentialSquared;
        public Color fogColor = Color.gray;
        [Range(0f, 0.1f)]
        public float fogDensity = 0.01f;

        [Header("Lighting")]
        public Color ambientColor = Color.gray;
        [Range(0f, 2f)]
        public float ambientIntensity = 1f;
        public Color sunColor = Color.white;
        [Range(0f, 3f)]
        public float sunIntensity = 1f;
    }

    [Header("Time of Day Presets")]
    public TimeOfDaySettings morningSettings = new TimeOfDaySettings { timeName = "Morning" };
    public TimeOfDaySettings noonSettings = new TimeOfDaySettings { timeName = "Noon" };
    public TimeOfDaySettings eveningSettings = new TimeOfDaySettings { timeName = "Evening" };
    public TimeOfDaySettings nightSettings = new TimeOfDaySettings { timeName = "Night" };

    [Header("Sun Light Reference")]
    public Light sunLight;

    [Header("Debug")]
    public bool randomizeOnStart = true;
    public bool showCurrentTime = true;

    private TimeOfDaySettings currentSettings;
    private TimeOfDaySettings[] allSettings;

    void Start()
    {
        // Disable any existing RenderSettingsUpdaters in the scene
        TPSBR.RenderSettingsUpdater[] existingUpdaters = FindObjectsOfType<TPSBR.RenderSettingsUpdater>();
        foreach (var updater in existingUpdaters)
        {
            updater.enabled = false;
            Debug.Log($"TimeOfDay: Disabled RenderSettingsUpdater on {updater.gameObject.name}");
        }

        StartCoroutine(DelayedInitialization());
    }

    IEnumerator DelayedInitialization()
    {
        // Wait for all scene initialization to complete
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        InitializeSettings();

        if (randomizeOnStart)
        {
            SetRandomTimeOfDay();
        }
        else
        {
            SetTimeOfDay(0);
        }

        // Force reapply settings after a delay to override any other scripts
        yield return StartCoroutine(ForceReapplySettings());
    }

    IEnumerator ForceReapplySettings()
    {
        yield return new WaitForSeconds(0.1f);
        ApplyTimeOfDaySettings();

        // Reapply again to be absolutely sure
        yield return new WaitForSeconds(0.1f);
        ApplyTimeOfDaySettings();

        if (showCurrentTime)
        {
            Debug.Log($"TimeOfDay: Final settings applied - {currentSettings.timeName} with fog density: {currentSettings.fogDensity}");
        }
    }

    void InitializeSettings()
    {
        SetupMorningDefaults();
        SetupNoonDefaults();
        SetupEveningDefaults();
        SetupNightDefaults();

        allSettings = new TimeOfDaySettings[] { morningSettings, noonSettings, eveningSettings, nightSettings };
    }

    void SetupMorningDefaults()
    {
        morningSettings.timeName = "Morning";
        morningSettings.fogColor = new Color(0.8f, 0.9f, 1f, 1f);
        morningSettings.fogDensity = 0.003f; // Reduced for better visibility
        morningSettings.ambientColor = new Color(0.7f, 0.8f, 1f, 1f);
        morningSettings.ambientIntensity = 0.8f;
        morningSettings.sunColor = new Color(1f, 0.95f, 0.8f, 1f);
        morningSettings.sunIntensity = 1.2f;
        
        // Auto-assign skybox if available
        if (morningSettings.skyboxMaterial == null)
        {
            morningSettings.skyboxMaterial = FindBestSkyboxMaterial();
        }
    }

    void SetupNoonDefaults()
    {
        noonSettings.timeName = "Noon";
        noonSettings.fogColor = new Color(0.9f, 0.95f, 1f, 1f);
        noonSettings.fogDensity = 0.002f; // Reduced for clear day
        noonSettings.ambientColor = new Color(0.5f, 0.7f, 1f, 1f);
        noonSettings.ambientIntensity = 1.2f;
        noonSettings.sunColor = Color.white;
        noonSettings.sunIntensity = 2f;
        
        // Auto-assign skybox if available
        if (noonSettings.skyboxMaterial == null)
        {
            noonSettings.skyboxMaterial = FindBestSkyboxMaterial();
        }
    }

    void SetupEveningDefaults()
    {
        eveningSettings.timeName = "Evening";
        eveningSettings.fogColor = new Color(1f, 0.7f, 0.5f, 1f);
        eveningSettings.fogDensity = 0.006f; // Slightly more atmospheric
        eveningSettings.ambientColor = new Color(1f, 0.6f, 0.4f, 1f);
        eveningSettings.ambientIntensity = 0.9f;
        eveningSettings.sunColor = new Color(1f, 0.6f, 0.3f, 1f);
        eveningSettings.sunIntensity = 1.5f;
        
        // Auto-assign skybox if available
        if (eveningSettings.skyboxMaterial == null)
        {
            eveningSettings.skyboxMaterial = FindBestSkyboxMaterial();
        }
    }

    void SetupNightDefaults()
    {
        nightSettings.timeName = "Night";
        nightSettings.fogColor = new Color(0.1f, 0.15f, 0.3f, 1f); // Darker, more bluish
        nightSettings.fogDensity = 0.008f; // Slightly reduced to prevent black borders
        nightSettings.ambientColor = new Color(0.15f, 0.2f, 0.4f, 1f); // Brighter ambient
        nightSettings.ambientIntensity = 0.4f; // Increased from 0.3f
        nightSettings.sunColor = new Color(0.6f, 0.7f, 1f, 1f); // Moonlight color
        nightSettings.sunIntensity = 0.3f; // Reduced moon intensity
        
        // Auto-assign skybox if available
        if (nightSettings.skyboxMaterial == null)
        {
            nightSettings.skyboxMaterial = FindBestSkyboxMaterial();
        }
    }

    public void SetRandomTimeOfDay()
    {
        int randomIndex = Random.Range(0, allSettings.Length);
        SetTimeOfDay(randomIndex);
    }

    public void SetTimeOfDay(int timeIndex)
    {
        if (timeIndex < 0 || timeIndex >= allSettings.Length)
        {
            Debug.LogWarning("TimeOfDay: Invalid time index: " + timeIndex);
            return;
        }

        currentSettings = allSettings[timeIndex];
        ApplyTimeOfDaySettings();

        if (showCurrentTime)
        {
            Debug.Log($"TimeOfDay: Set to {currentSettings.timeName}");
        }
    }

    void ApplyTimeOfDaySettings()
    {
        if (currentSettings == null) return;

        Debug.Log($"TimeOfDay: Applying {currentSettings.timeName} with fog density: {currentSettings.fogDensity}");

        // Apply Skybox
        if (currentSettings.skyboxMaterial != null)
        {
            RenderSettings.skybox = currentSettings.skyboxMaterial;
        }

        // Apply Fog Settings
        RenderSettings.fog = currentSettings.enableFog;
        RenderSettings.fogMode = currentSettings.fogMode;
        RenderSettings.fogColor = currentSettings.fogColor;
        RenderSettings.fogDensity = currentSettings.fogDensity;

        // Apply Ambient Lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = currentSettings.ambientColor;
        RenderSettings.ambientIntensity = currentSettings.ambientIntensity;

        // Apply Sun Settings
        if (sunLight != null)
        {
            sunLight.color = currentSettings.sunColor;
            sunLight.intensity = currentSettings.sunIntensity;
            RenderSettings.sun = sunLight;
        }

        // Force skybox update
        DynamicGI.UpdateEnvironment();
    }

    // Public methods to manually set specific times
    public void SetMorning() => SetTimeOfDay(0);
    public void SetNoon() => SetTimeOfDay(1);
    public void SetEvening() => SetTimeOfDay(2);
    public void SetNight() => SetTimeOfDay(3);

    // Get current time name for UI or debugging
    public string GetCurrentTimeName()
    {
        return currentSettings?.timeName ?? "Unknown";
    }
    
    // Helper method to find the best available skybox material
    private Material FindBestSkyboxMaterial()
    {
        // Try to find a good skybox material in the project
        Material[] allMaterials = Resources.FindObjectsOfTypeAll<Material>();
        
        // Priority order: Look for specific skybox shaders
        string[] preferredShaders = {
            "Skybox/Procedural",
            "Skybox/6 Sided",
            "Skybox/Cubemap",
            "Skybox/Panoramic"
        };
        
        foreach (string shaderName in preferredShaders)
        {
            foreach (Material mat in allMaterials)
            {
                if (mat.shader != null && mat.shader.name == shaderName)
                {
                    // Skip if it's the default Unity skybox (usually not what we want)
                    if (!mat.name.Contains("Default") && !mat.name.Contains("unity_builtin"))
                    {
                        Debug.Log($"TimeOfDay: Found skybox material: {mat.name} with shader: {shaderName}");
                        return mat;
                    }
                }
            }
        }
        
        // Fallback: Create a simple procedural skybox
        Material proceduralSkybox = new Material(Shader.Find("Skybox/Procedural"));
        if (proceduralSkybox != null)
        {
            // Set nice default values
            proceduralSkybox.SetFloat("_AtmosphereThickness", 1.0f);
            proceduralSkybox.SetFloat("_SunSize", 0.04f);
            proceduralSkybox.SetColor("_SkyTint", new Color(0.5f, 0.5f, 0.5f, 1f));
            proceduralSkybox.SetColor("_GroundColor", new Color(0.369f, 0.349f, 0.341f, 1f));
            proceduralSkybox.SetFloat("_Exposure", 1.3f);
            
            Debug.Log("TimeOfDay: Created fallback procedural skybox");
            return proceduralSkybox;
        }
        
        Debug.LogWarning("TimeOfDay: Could not find or create a suitable skybox material");
        return null;
    }

    void OnValidate()
    {
        // Update settings in real-time when editing in inspector
        if (Application.isPlaying && currentSettings != null)
        {
            ApplyTimeOfDaySettings();
        }
    }
}
