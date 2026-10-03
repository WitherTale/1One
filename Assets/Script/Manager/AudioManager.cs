using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }


    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;


    [SerializeField] private AudioClip[] bgmClips;

    public AudioClip jumpSfx;     
    public AudioClip DieSfx;
    public AudioClip clickSfx;
    public AudioClip milestoneSfx;  

    private int currentBgmIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGMForMap(int mapIndex)
    {
        if (bgmClips == null || bgmClips.Length == 0 || bgmSource == null) return;

        int safeIndex = Mathf.Clamp(mapIndex, 0, bgmClips.Length - 1);
        if (currentBgmIndex == safeIndex && bgmSource.isPlaying) return;

        currentBgmIndex = safeIndex;

        if (bgmClips[safeIndex] != null)
        {
            bgmSource.clip = bgmClips[safeIndex];
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
            currentBgmIndex = -1;
        }
    }

  
    public void PlayJump() { if (sfxSource && jumpSfx) sfxSource.PlayOneShot(jumpSfx); }
    public void PlayDie() { if (sfxSource && DieSfx) sfxSource.PlayOneShot(DieSfx); }
    public void PlayClick() { if (sfxSource && clickSfx) sfxSource.PlayOneShot(clickSfx); }
    public void PlayMilestone() { if (sfxSource && milestoneSfx) sfxSource.PlayOneShot(milestoneSfx); }

}