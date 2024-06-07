using Unity.Netcode;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private AudioClip jumpAudioClip;
    [SerializeField] private AudioClip waterSplashAudioClip;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [ClientRpc]
    public void PlayAudioClientRpc(string audioClip)
    {
        switch (audioClip)
        {
            case "JumpAudio":
                audioSource.PlayOneShot(jumpAudioClip);
                break;

            case "SplashAudio":
                audioSource.PlayOneShot(waterSplashAudioClip);
                break;
        }
    }
}
