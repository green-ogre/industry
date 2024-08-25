using UnityEngine;

public class AudioMaster : MonoBehaviour
{
    public static AudioMaster instance;

    public float volume = 1f;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    public void PlayClip(AudioClip clip, Vector3 point, float clipVolume)
    {
        AudioSource.PlayClipAtPoint(clip, point, volume * clipVolume);
    }
}
