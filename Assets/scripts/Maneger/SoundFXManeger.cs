
using UnityEngine;


public class SoundFxManeger : MonoBehaviour
{
public static SoundFxManeger instance;
    [SerializeField] private AudioSource soundFxobject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void playsoundFX(AudioClip[] audioClip, Transform spawntransform, float volume)
    {
        int rand = Random.Range(0, audioClip.Length);
        AudioSource audioSource = Instantiate(soundFxobject, spawntransform.position, Quaternion.identity);
        audioSource.clip = audioClip[rand];
        audioSource.volume = volume;
        audioSource.Play();
        float ClipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, ClipLength);
    }
}
