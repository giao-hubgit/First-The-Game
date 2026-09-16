using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicsSettings : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>();

    private const string RES_INDEX_KEY = "ResolutionIndex";
    private const string FULLSCREEN_KEY = "IsFullscreen";

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

        savedResIndex = Mathf.Clamp(savedResIndex, 0, filteredResolutions.Count - 1);

        resolutionDropdown.value = savedResIndex;
        resolutionDropdown.RefreshShownValue();

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = savedFullscreen;
        }

        SetResolution(savedResIndex);
        SetFullscreen(savedFullscreen);
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
}