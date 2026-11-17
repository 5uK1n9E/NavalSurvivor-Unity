using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    public AudioClip hitSound;
    public AudioClip boomSound;

    private AudioSource source;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
    }

    public void PlayHit()
    {
        if (hitSound != null) source.PlayOneShot(hitSound);
    }

    public void PlayBoom()
    {
        if (boomSound != null) source.PlayOneShot(boomSound);
    }
}