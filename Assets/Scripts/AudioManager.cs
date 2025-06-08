using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public SoundLibrary soundLibrary;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        // --- Singleton Pattern ---
        // Đảm bảo chỉ có một AudioManager duy nhất trong game
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Không hủy đối tượng này khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // -------------------------
        DOTween.Init();
        // Tự tạo các AudioSource
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        PlayMusic(soundLibrary.backgroundMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    // Hàm để các script khác gọi để chơi hiệu ứng âm thanh
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}