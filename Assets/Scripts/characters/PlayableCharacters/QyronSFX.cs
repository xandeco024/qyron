using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class qyronSFX : MonoBehaviour
{
    private AudioSource qyronAudioSource;

    [SerializeField] private List<AudioClip> qyronSFXClips;

    private void Awake()
    {

    }

    void Start()
    {
        qyronAudioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        
    }

    public void PlayAttackSFX(string attackSFXIndex)
    {
        foreach (AudioClip sfx in qyronSFXClips)
        {
            if (sfx.name == attackSFXIndex)
            {
                qyronAudioSource.PlayOneShot(sfx);
                return;
            }
        }
        Debug.LogWarning("AudioClip " + attackSFXIndex + " not found in the audio list.");
    }

    public void PlayMissSFX(string missSFXIndex)
    {
        foreach (AudioClip sfx in qyronSFXClips)
        {
            if (sfx.name == missSFXIndex)
            {
                qyronAudioSource.PlayOneShot(sfx);
                return;
            }
        }
        Debug.LogWarning("AudioClip " + missSFXIndex + " not found in the audio list.");
    }

    public void PlayMovementSFX(string movementSFXIndex) 
    {
        foreach (AudioClip sfx in qyronSFXClips)
        {
            if (sfx.name == movementSFXIndex)
            {
                qyronAudioSource.PlayOneShot(sfx);
                return;
            }
        }
        Debug.LogWarning("AudioClip " + movementSFXIndex + " not found in the audio list.");
    }
}
