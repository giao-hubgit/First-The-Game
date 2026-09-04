using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Components")]
    public AudioMixer mainMixer;
    public Slider masterSlider;

    private const string MASTER_KEY = "MasterVolume";

    void Start()
    {
        // Tải âm lượng đã lưu từ lần chơi trước (mặc định là 1 nếu chưa lưu)
        float savedVolume = PlayerPrefs.GetFloat(MASTER_KEY, 1f);

        // Đặt vị trí thanh Slider theo giá trị đã lưu
        if (masterSlider != null)
        {
            masterSlider.value = savedVolume;
        }

        // Áp dụng âm lượng vào AudioMixer
        SetMasterVolume(savedVolume);
    }

    // Hàm này sẽ gắn vào sự kiện OnValueChanged của Slider
    public void SetMasterVolume(float sliderValue)
    {
        // Tai người nghe âm thanh theo cấp số nhân (Logarithm), 
        // do đó cần chuyển đổi giá trị Slider (0.0001 -> 1) sang Decibel (-80dB -> 0dB)
        float volumeInDecibels = Mathf.Log10(sliderValue) * 20;

        // Gán giá trị vào tham số MasterVolume đã expose trong AudioMixer
        mainMixer.SetFloat("MasterVolume", volumeInDecibels);

        // Lưu cài đặt lại
        PlayerPrefs.SetFloat(MASTER_KEY, sliderValue);
    }
}