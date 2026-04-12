using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioClip[] playlist; // drag your .wav files here

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        PlayRandomSong();
    }

    void Update()
    {
        // If song finished, play another random one
        if (!audioSource.isPlaying)
        {
            PlayRandomSong();
        }
    }

    void PlayRandomSong()
    {
        if (playlist.Length == 0) return;

        int randomIndex = Random.Range(0, playlist.Length);
        audioSource.clip = playlist[randomIndex];
        audioSource.Play();
    }
}