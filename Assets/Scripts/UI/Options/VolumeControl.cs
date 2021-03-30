using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : PanelElement
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private float prevSfx, prevMusic;

    public void OnSfxChanged(float amount)
    {
        VolumeManager.Instance.SetVolume(amount, "Sfx");
    }

    public void OnMusicChanged(float amount)
    {
        VolumeManager.Instance.SetVolume(amount, "Music");
    }

    public override void InnerOnShow()
    {
        sfxSlider.value = prevSfx = VolumeManager.Instance.GetVolume("Sfx");
        musicSlider.value = prevMusic = VolumeManager.Instance.GetVolume("Music");
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
    }

    public override bool InnerOnCancel()
    {
        VolumeManager.Instance.SetVolume(prevSfx, "Sfx");
        VolumeManager.Instance.SetVolume(prevMusic, "Music");
        return true;
    }

    private void OnDestroy()
    {
        if (sfxSlider)
            sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
        if (musicSlider)
            musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
    }
}
