using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager Instance;

    public AudioClip bgm;

    [SerializeField] private Slider m_musicSlider;
    private AudioSource m_audioSource;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            m_audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (bgm != null)
        {
            PlayBGM(false, bgm);
        }

        m_musicSlider.onValueChanged.AddListener(delegate { SetVolume(m_musicSlider.value); });
    }

    public static void SetVolume(float volume)
    {
        Instance.m_audioSource.volume = volume;
    }

    public static void PlayBGM(bool resetSong, AudioClip audioClip = null)
    {
        if (audioClip != null)
        {
            Instance.m_audioSource.clip = audioClip;
        }

        if (Instance.m_audioSource.clip != null)
        {
            if (resetSong)
            {
                Instance.m_audioSource.Stop();
            }

            Instance.m_audioSource.Play();
        }
    }

    public static void PauseBGM()
    {
        Instance.m_audioSource.Pause();
    }
}
