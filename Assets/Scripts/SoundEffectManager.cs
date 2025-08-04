using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager Instance;

    private static AudioSource m_audioSource;
    private static SoundEffectLibrary m_soundEffectLibrary;
    [SerializeField] private Slider m_sfxSlider;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            m_audioSource = GetComponent<AudioSource>();
            m_soundEffectLibrary = GetComponent<SoundEffectLibrary>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Play(string soundName)
    {
        AudioClip audioClip = m_soundEffectLibrary.GetRandomClip(soundName);

        if (audioClip != null)
        {
            m_audioSource.PlayOneShot(audioClip);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); } );
    }

    public static void SetVolume(float volume)
    {
        m_audioSource.volume = volume;
    }

    public void OnValueChanged()
    {
        SetVolume(m_sfxSlider.value);
    }
}
