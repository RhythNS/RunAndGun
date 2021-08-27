using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionScreen : PanelElement
{
    [SerializeField]
    private Toggle fullscreenToggle;

    [SerializeField]
    private TMPro.TMP_Dropdown resDropdown;

    private Resolution[] resolutions;

    public override void InnerOnShow()
    {
        fullscreenToggle.isOn = Screen.fullScreen;

        resolutions = Screen.resolutions;

        resDropdown.ClearOptions();

        int currentResIndex = 0;
        List<string> optionList = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string optionName = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            optionList.Add(optionName);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                currentResIndex = i;
        }

        resDropdown.AddOptions(optionList);
        resDropdown.value = currentResIndex;
        resDropdown.RefreshShownValue();

        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        resDropdown.onValueChanged.AddListener(SetResolution);
    }

    public override bool InnerOnConfirm()
    {
        return true;
    }

    private void SetResolution(int ResolutionIndex)
    {
        Resolution resolution = resolutions[ResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void OnDestroy()
    {
        fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
        resDropdown.onValueChanged.RemoveListener(SetResolution);
    }
}
