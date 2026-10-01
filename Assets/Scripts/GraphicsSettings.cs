using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicsSettings : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider screenshakeSlider;
    public TMP_Dropdown FPSDropdown;
    public Toggle vSyncToggle;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>();

    private readonly int[] fpsOptions = new int[] { 30, 60, 120, 144, 240, -1 };

    private const string RES_INDEX_KEY = "ResolutionIndex";
    private const string FULLSCREEN_KEY = "IsFullscreen";
    private const string SCREENSHAKE_KEY = "Screenshake";
    private const string VSYNC_KEY = "VSync";
    private const string FPS_KEY = "FPS";

    void Start()
    {
        resolutions = Screen.resolutions;

        System.Array.Sort(resolutions, (a, b) =>
        {
            if (a.width != b.width)
                return b.width.CompareTo(a.width);
            return b.height.CompareTo(a.height);
        });

        filteredResolutions.Clear();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        HashSet<string> addedResolutions = new HashSet<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;

            if (!addedResolutions.Contains(option))
            {
                addedResolutions.Add(option);
                filteredResolutions.Add(resolutions[i]);
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = filteredResolutions.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);

        int savedResIndex = PlayerPrefs.GetInt(RES_INDEX_KEY, currentResolutionIndex);
        bool savedFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, Screen.fullScreen ? 1 : 0) == 1;
        float savedScreenShake = PlayerPrefs.GetFloat(SCREENSHAKE_KEY, 1f);
        int savedVSync = PlayerPrefs.GetInt(VSYNC_KEY, 1);
        int savedFPS = PlayerPrefs.GetInt(FPS_KEY, 60);

        savedResIndex = Mathf.Clamp(savedResIndex, 0, filteredResolutions.Count - 1);

        if (resolutionDropdown != null)
        {
            resolutionDropdown.value = savedResIndex;
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = savedFullscreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        if (screenshakeSlider != null)
        {
            screenshakeSlider.value = savedScreenShake;
            screenshakeSlider.onValueChanged.AddListener(SetShakeForce);
        }

        if (vSyncToggle != null)
        {
            vSyncToggle.isOn = savedVSync == 1;
            vSyncToggle.onValueChanged.AddListener(SetVSyncFromToggle);
        }

        if (FPSDropdown != null)
        {
            FPSDropdown.ClearOptions();

            List<string> fpsLabels = new List<string>();
            foreach (int fps in fpsOptions)
            {
                if (fps == -1)
                    fpsLabels.Add("Unlimited");
                else
                    fpsLabels.Add(fps + " FPS");
            }

            FPSDropdown.AddOptions(fpsLabels);

            int fpsIndex = System.Array.IndexOf(fpsOptions, savedFPS);
            if (fpsIndex == -1) fpsIndex = 1;

            FPSDropdown.value = fpsIndex;
            FPSDropdown.RefreshShownValue();
            FPSDropdown.onValueChanged.AddListener(SetFPSFromDropdown);
        }

        SetResolution(savedResIndex);
        SetFullscreen(savedFullscreen);
        SetShakeForce(savedScreenShake);
        SetVSync(savedVSync);
        SetFPS(savedFPS);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= filteredResolutions.Count) return;

        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt(RES_INDEX_KEY, resolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0);
    }

    public void SetShakeForce(float shakeForce)
    {
        CameraShakeManager.Instance?.SetAmount(shakeForce);
        PlayerPrefs.SetFloat(SCREENSHAKE_KEY, shakeForce);
    }

    public void SetVSyncFromToggle(bool isVSync)
    {
        SetVSync(isVSync ? 1 : 0);
    }

    public void SetVSync(int vSyncCount)
    {
        QualitySettings.vSyncCount = vSyncCount;
        PlayerPrefs.SetInt(VSYNC_KEY, vSyncCount);
    }

    public void SetFPSFromDropdown(int dropdownIndex)
    {
        if (dropdownIndex >= 0 && dropdownIndex < fpsOptions.Length)
        {
            SetFPS(fpsOptions[dropdownIndex]);
        }
    }

    public void SetFPS(int fps)
    {
        Application.targetFrameRate = fps;
        PlayerPrefs.SetInt(FPS_KEY, fps);
    }
}