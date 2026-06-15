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
        // Movido para o Awake para garantir que exista antes de qualquer chamada
        qyronAudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
    }

    void Update()
    {

    }

    // Agora todas as funções aceitam o float volume = 1.0f (padrão)
    public void PlayAttackSFX(string attackSFXIndex, float volume = 1.0f)
    {
        foreach (AudioClip sfx in qyronSFXClips)
        {
            if (sfx.name == attackSFXIndex)
            {
                qyronAudioSource.PlayOneShot(sfx, volume);
                return;
            }
        }
        Debug.LogWarning("AudioClip " + attackSFXIndex + " not found in the audio list.");
    }

    public void PlayMissSFX(string missSFXIndex, float volume = 1.0f)
    {
        foreach (AudioClip sfx in qyronSFXClips)
        {
            if (sfx.name == missSFXIndex)
            {
                qyronAudioSource.PlayOneShot(sfx, volume);
                return;
            }
        }
        Debug.LogWarning("AudioClip " + missSFXIndex + " not found in the audio list.");
    }

    public void PlayMovementSFX(string movementSFXIndex, float volume = 1.0f)
    {
        foreach (AudioClip sfx in qyronSFXClips)
        {
            if (sfx.name == movementSFXIndex)
            {
                qyronAudioSource.PlayOneShot(sfx, volume);
                return;
            }
        }
        Debug.LogWarning("AudioClip " + movementSFXIndex + " not found in the audio list.");
    }
}